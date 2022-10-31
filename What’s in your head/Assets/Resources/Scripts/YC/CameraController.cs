using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using System.Linq;

using Cinemachine;

using JCW.UI.Options.InputBindings;
using YC.CameraManager_;
using UnityEngine.Rendering.Universal;

using KSU;

namespace YC.Camera_
{
    public class CameraController : MonoBehaviour, IPunObservable
    {

        // Photon 
        PhotonView pv;

        // Player Cam
        //public Camera mainCam;
        public Camera mainCam { get; private set; }

        // Cinemachine
        public CinemachineBrain cinemachineBrain;
        List<CinemachineVirtualCameraBase> camList;

        CinemachineVirtualCameraBase backCam;
        CinemachineVirtualCameraBase sholderCam;
        CinemachineVirtualCameraBase topCam;

        enum CamState { back, sholder, top, };
        CamState curCam;
        CamState preCam;

        CamState curCam_Clone;

        [Header("[Back View 카메라 마우스 감도]")]
        [SerializeField] [Range(0, 100)] float backView_MouseSensitivity;
        [Header("[Back View 카메라 마우스 감도]")]
        [SerializeField] [Range(0, 100)] float sholderView_MouseSensitivity;


        PlayerController player;

        float sholderViewMaxY;
        [Header("[Sholder View Y궤도 Up 제한 값]")]
        [SerializeField] [Range(0, 1)] float sholderAxisY_MaxUp = 0.3f;
        [Header("[Sholder View Y궤도 Down 제한 값]")]
        [SerializeField] [Range(0, 1)] float sholderAxisY_MaxDown = 0.5f;


        [Header("[스테디 빔, 카메라 흔들림 진폭 크기]")]
        [SerializeField] [Range(0, 5)] float AmplitudeGain = 3f;
        [Header("[스테디 빔, 카메라 흔들림 빈도]")]
        [SerializeField] [Range(0, 5)] float FrequebctGain = 3f;
        //bool isInitCamera = false;
        List<CinemachineBasicMultiChannelPerlin> listCBMCP;

        bool wasShaked = false;
        bool isShakedFade = true;

        [Header("[넬라 시네머신 백뷰]")]
        [SerializeField] CinemachineVirtualCameraBase CineNellaBack;
        [Header("[넬라 시네머신 숄더뷰]")]
        [SerializeField] CinemachineVirtualCameraBase CineNellaSholder;
        [Header("[스테디 시네머신 백뷰]")]
        [SerializeField] CinemachineVirtualCameraBase CineSteadyBack;
        [Header("[스테디 시네머신 숄더뷰]")]
        [SerializeField] CinemachineVirtualCameraBase CineSteadySholder;


        //[Header("[일반 점프 후, 공중 점프나 공중 대쉬까지의 보간 시간]")]
        float normalJumpLerpTime = 0.2f;


        PlayerState playerState;


        float backViewFOV = 40;


        // >> : 점프시 Follow, LookAt 관련
        Transform followObj;
        Transform lookatBackObj;
        Transform lookatSholderObj;
        float lookatObjOriginY;

        

        [SerializeField] bool isJumping = false;

        float orgLookY;
        float orgFollowY;

        float originLocalFollowY;
        float originLocalLookAtY;


        public bool canShake = true; // 옵션 스테디 흔들림 여부



        void Awake()
        {
            pv = GetComponent<PhotonView>();

            if (pv) pv.ObservedComponents.Add(this);

            camList = new List<CinemachineVirtualCameraBase>();

            InitInstantiate();

            curCam = new CamState();
            preCam = new CamState();



            if (!pv.IsMine)
            {
                //curCam_Clone = CamState.back;
                //OnOffCamera(camList[(int)curCam_Clone]);
            }
            else
            {
                sholderViewMaxY = sholderCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxValue;

                curCam = CamState.back;
                preCam = curCam;
                OnOffCamera(camList[(int)curCam]);
            }

            FindCamera();

            player = this.gameObject.GetComponent<PlayerController>();


            // 카메라 FOV 초기화 (해당 시네머신 카메라는 Commone Lens 설정 필요)
            //if (pv.IsMine)
            //    mainCam.fieldOfView = camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView;
            //else
            //    mainCam.fieldOfView = camList[(int)curCam_Clone].GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView;

            mainCam.fieldOfView = backViewFOV;

            // 만약 둘이서 멀티 진행했는데 Nella Remote 블렌딩 진행중 Steady Owner의 마우스 값이 Nella Remote에게 들어온다면
            // 시야각이 카메라끼리의 블렌딩 불가, 시야각을 위한 별도 설정 필요
            if (!pv.IsMine) cinemachineBrain.enabled = false;

            listCBMCP = new List<CinemachineBasicMultiChannelPerlin>();

            InitDefault();

            playerState = this.gameObject.GetComponent<PlayerState>();

        }


        void FixedUpdate()
        {
            if (!pv.IsMine) return;

            if (isJumping && !isAirJump)
                NormalJumpCameraSet();

            SetCamera();
            SetAimYAxis();

            //if (!playerState.IsGrounded && !timer)
            //{
            //    StartCoroutine(Timer());
            //}
            if (player.characterState.IsJumping)
                DetectPlayerInScreen();

            if (player.characterState.IsJumping && isAirJump && endCor)
                AirJumpCameraSet();

            //Debug.Log(followObj.transform.position.y * 100);
        }

        bool wasLerped = false;
        bool timer = false;



        bool endCor = false;
        bool isCor = false;

        bool isJumpAfterSet = false;

        bool isJumpingMin = false;
        void DetectPlayerInScreen()
        {
            
            if (isJumpAfterSet) return;

            if (player.transform.position.y < followObj.transform.position.y) // 현재 플레이어의 높이가, 최초 점프 시작 높이보다 낮다면
            {
                //Debug.Log("호출!");

                lookatBackObj.position = new Vector3(lookatBackObj.transform.position.x, player.transform.position.y + lookatObjOriginY, lookatBackObj.transform.position.z);
                followObj.position = new Vector3(followObj.transform.position.x, player.transform.position.y, followObj.transform.position.z);
                isJumpAfterSet = true;
                isJumpingMin = true;
            }
        }


        bool isAirJump = false;
        void AirJumpCameraSet()
        {
            //Debug.Log("호출 - 공중 점프 코루틴 후 카메라 세팅");
            lookatBackObj.position = new Vector3(lookatBackObj.transform.position.x, player.transform.position.y + lookatObjOriginY, lookatBackObj.transform.position.z);
            followObj.position = new Vector3(followObj.transform.position.x, player.transform.position.y, followObj.transform.position.z);
        }



        public void Option_SetShake(bool on)
        {
            canShake = on;
        }

        void InitInstantiate()
        {
            if (!pv.IsMine) return;

            if (this.gameObject.CompareTag("Nella"))
            {
                backCam = Instantiate(CineNellaBack, Vector3.zero, Quaternion.identity).GetComponent<CinemachineVirtualCameraBase>();
                sholderCam = Instantiate(CineNellaSholder, Vector3.zero, Quaternion.identity).GetComponent<CinemachineVirtualCameraBase>();

            }
            else
            {
                backCam = Instantiate(CineSteadyBack, Vector3.zero, Quaternion.identity).GetComponent<CinemachineVirtualCameraBase>();
                sholderCam = Instantiate(CineSteadySholder, Vector3.zero, Quaternion.identity).GetComponent<CinemachineVirtualCameraBase>();
              
            }

            followObj = transform.Find("Cine_followObj").gameObject.transform;
            lookatBackObj = transform.Find("Cine_lookatObj_Back").gameObject.transform;
            lookatObjOriginY = lookatBackObj.transform.position.y;
            lookatSholderObj = transform.Find("Cine_lookatObj_Sholder").gameObject.transform;

       

            backCam.GetComponent<CinemachineFreeLook>().m_Follow = followObj;
            backCam.GetComponent<CinemachineFreeLook>().m_LookAt = lookatBackObj;

            sholderCam.GetComponent<CinemachineFreeLook>().m_Follow = followObj;
            sholderCam.GetComponent<CinemachineFreeLook>().m_LookAt = lookatSholderObj;

            originLocalFollowY = followObj.transform.localPosition.y;
            originLocalLookAtY = lookatBackObj.transform.localPosition.y;


            camList.Add(backCam);
            camList.Add(sholderCam);
        }
        void InitDefault()
        {
            if (!pv.IsMine) return;
            // backView_MouseSensitivity, sholderView_MouseSensitivity 두 변수는 1~100의 값을 갖는다 (찬우형 게임매니저로 부터 받아옴)
            // 시네머신에서 최적의 마우스 스피드는 X : 100 ~ 300, Y : 1 ~ 3 정도이다.
            // 감도 변수를 시네머신에 맞게 변환하여 세팅해준다       

            // << : 감도 관련
            Option_SetSensitivity(backView_MouseSensitivity, sholderView_MouseSensitivity);


            // << : 숄더뷰 Y축 제한
            if (sholderAxisY_MaxUp == 0) sholderAxisY_MaxUp = 0.2f;
            if (sholderAxisY_MaxDown == 0) sholderAxisY_MaxDown = 0.5f;


            // << : 스테디 빔 흔들림 변수 설정

            if (AmplitudeGain == 0) AmplitudeGain = 1;
            if (FrequebctGain == 0) FrequebctGain = 2;

            if (this.gameObject.CompareTag("Steady"))
            {
                for (int i = 0; i < 3; ++i)
                {
                    listCBMCP.Add(sholderCam.GetComponent<CinemachineFreeLook>().GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>());
                }

                for (int i = 0; i < 3; ++i)
                {
                    sholderCam.GetComponent<CinemachineFreeLook>().GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
                    sholderCam.GetComponent<CinemachineFreeLook>().GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
                }
            }
        }

        public void Option_SetSensitivity(float backSensitivity, float sholderSensitivity)
        {
            // << : 감도 관련
            int defaulyX = 100;
            int defaultY = 1;

            if (backView_MouseSensitivity == 0) backSensitivity = 25;

            backCam.GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = defaulyX + backSensitivity * 4;
            backCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = defaultY + backSensitivity * 0.04f;

            if (sholderView_MouseSensitivity == 0) sholderSensitivity = 25;

            sholderCam.GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = defaulyX + sholderSensitivity * 4;
            sholderCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = defaultY + sholderSensitivity * 0.04f;
        } // 감도 관련 매니저에서 호출 하도록

        void SetAimYAxis() // sholder View에서 YAxis Limit 설정.
        {
            if (curCam == CamState.sholder)
            {
                AxisState axisY = camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis;

                if (axisY.Value <= sholderAxisY_MaxUp) // 커서가 Max 위로 넘어감
                {
                    // axisY.m_InputAxisValue : 커서 위(1) ~ 아래(0)
                    // axisY.Value : 커서 위(-) ~ 아래 (+)

                    if (axisY.m_InputAxisValue > 0)
                    {
                        axisY.m_MaxSpeed = 0;
                    }
                    else if (axisY.m_InputAxisValue < 0)
                    {
                        axisY.m_MaxSpeed = sholderViewMaxY;

                    }
                }
                else if (axisY.Value >= sholderAxisY_MaxDown) // 커서가 Min 밑으로 내려감.
                {
                    if (axisY.m_InputAxisValue > 0)
                    {
                        axisY.m_MaxSpeed = sholderAxisY_MaxDown;

                    }
                    else if (axisY.m_InputAxisValue < 0)
                    {
                        axisY.m_MaxSpeed = 0;
                    }
                }
                else
                {
                    axisY.m_MaxSpeed =
                        sholderCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = 1 + backView_MouseSensitivity * 0.02f;
                }
                camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis = axisY;
            }
        }

        void SetCamera() // 플레이어 State 따라카메라 세팅 
        {
            if (GameManager.Instance.isTopView)
                return;


            if (curCam == CamState.back)
            {
                if (player.characterState.aim) // back View -> sholder View
                {
                    AxisState temp = backCam.GetComponent<CinemachineFreeLook>().m_XAxis;

                    preCam = curCam;
                    curCam = CamState.sholder;

                    if (camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value < sholderAxisY_MaxUp)
                        camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value = sholderAxisY_MaxUp;

                    if (camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value > sholderAxisY_MaxDown)
                        camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value = sholderAxisY_MaxDown;


                    camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_XAxis.Value = camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_XAxis.Value;
                    camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value = camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value;
                    OnOffCamera(sholderCam);

                    sholderCam.GetComponent<CinemachineFreeLook>().m_XAxis = temp;
                }
            }
            else if (curCam == CamState.sholder)
            {
                if (!player.characterState.aim) // sholder View -> back View
                {
                    AxisState temp = sholderCam.GetComponent<CinemachineFreeLook>().m_XAxis;

                    preCam = curCam;
                    curCam = CamState.back;

                    camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_XAxis.Value = camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_XAxis.Value;
                    camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value = camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value;

                    if (isShakedFade)
                    {
                        StopCoroutine("ShakeFadeOut");
                        InitShakeFade();
                    }

                    OnOffCamera(backCam);

                    backCam.GetComponent<CinemachineFreeLook>().m_XAxis = temp;
                }
            }
        }

        public void SetDefenseMode()
        {
            if (pv.IsMine)
            {
                topCam = GameObject.FindWithTag("Cine_DefenseCam").GetComponent<CinemachineVirtualCameraBase>();
                topCam.GetComponent<CinemachineVirtualCamera>().enabled = true;
                camList.Add(topCam);

                preCam = curCam;
                curCam = CamState.top;

                OnOffCamera(topCam);

                camList[(int)curCam].GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = 60;
            }
            else
            {
                mainCam.fieldOfView = 60;
            }
        }

        // << : 스테디 빔 사용시, 스테디 Aim Attack State에서 호출
        public void SetSteadyBeam(bool isLock)
        {
            // << : 탑뷰라면 리턴(준수형)
            if (GameManager.Instance.isTopView) return;

            if (!pv.IsMine) return;

            if (isLock)
            {
                camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_XAxis.m_InputAxisName = "";
                camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.m_InputAxisName = "";

                // << : 회전하면서 빔 사용시 삥삥 도는 문제
                camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_XAxis.m_InputAxisValue = 0;
            }
            else
            {
                camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_XAxis.m_InputAxisName = "Mouse X";
                camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.m_InputAxisName = "Mouse Y";
            }
        }



        float avgDif = 0.5f;
        float totDif = 0;
        float originPlayerY; // 플레이어가 점프하기 전 Y 포지션


        
       
        
        public void NormalJumpCameraInit(bool On)
        {
            
            if (On) // 일반 점프 시작
            {
                isJumping = true;
                isJumpAfterSet = false;
                isJumpingMin = false;
                isAirJump = false;
                orgLookY = lookatBackObj.transform.position.y;
                orgFollowY = followObj.transform.position.y;
                originPlayerY = player.transform.position.y;

                //Debug.Log("카메라 - 일반점프 시작");

            }
            else if (!On && isJumping)
            {
                if (isJumpAfterSet) return;
                

                //Debug.Log("들어옴");
                if (playerState.IsJumping && playerState.IsAirJumping && !playerState.IsAirDashing) // 공중 점프
                {
                    Debug.Log("호출 - 공중 점프 시작 ==============================================");
                    if (player.transform.position.y < lookatBackObj.transform.position.y) // 룩앳 오브젝트보다 낮은 위치에서 이중 점프를 시도한다면 블락
                    {
                        
                        //Debug.Log("입구컷1");
                        //Debug.Log("Player Y : " + player.transform.position.y);
                        //Debug.Log("Look Y : " + lookatBackObj.transform.position.y);
                        return;
                    }
                    isAirJump = true;
                    isJumping = false;
                    
                    float lerpTime = 2f;
                    StartCoroutine(LerpAfter(lerpTime));
                }
                else
                {
                    
                    //Debug.Log("Follow : " + followObj.transform.position.y);
                    //Debug.Log("origin : " + orgFollowY);
                    //Debug.Log("카메라 - 일반점프 종료");

                    float curDif = Mathf.Abs(player.transform.position.y - originPlayerY);  // 점프 시작 Y위치와, 점프 종료 Y위치의 차


                    if (curDif > avgDif)
                    {
                        isJumping = false;
                        totDif = 0;

                        float lerpTime = 0.7f;
                        if (!isJumpAfterSet)
                        {
                            StartCoroutine(LerpAfter(lerpTime));
                        }
                      
                        return;
                    }
                    else
                    {
                        lookatBackObj.position = new Vector3(lookatBackObj.transform.position.x, player.transform.position.y + lookatObjOriginY, lookatBackObj.transform.position.z);
                        followObj.position = new Vector3(followObj.transform.position.x, player.transform.position.y, followObj.transform.position.z);
                        isJumping = false;
                    }    
                    //if (playerState.IsJumping && playerState.IsAirJumping && playerState.IsAirDashing) return;
                    //lookatBackObj.localPosition = new Vector3(0, originLocalLookAtY, 0);
                    //followObj.localPosition = new Vector3(0, originLocalFollowY, 0);
                }
            }

            
        }
       

        public void NormalJumpCameraSet()
        {
            //Debug.Log("호출 - 일반 점프 세팅");
            if (isJumpingMin || isAirJump) return;

            float lookValue = orgLookY - lookatBackObj.transform.position.y;
            float followValue = orgFollowY - followObj.transform.position.y;

            lookatBackObj.position = 
                        new Vector3(lookatBackObj.transform.position.x, 
                                    lookatBackObj.position.y + lookValue, 
                                    lookatBackObj.transform.position.z);
            followObj.position = 
                        new Vector3(followObj.transform.position.x, 
                                    followObj.position.y + followValue, 
                                    followObj.transform.position.z);
        }
        

        IEnumerator LerpJump(float LerpTime)
        {
            Debug.Log("호출 - 보간 코루틴! ==============================================");
            Debug.Log("호출 확인용 : " + followObj.position.y * 100);
            float currentTime = 0;

            float initYpos = originPlayerY; // 점프 시작시 플레이어 Y 값
            float lerpYpos;  // 보간이 이루어지고 있는 y 값
            endCor = false;

            float delayTime = 0.1f;
            yield return new WaitForSeconds(delayTime);

           
       
            lerpYpos = initYpos;
            while (lerpYpos < player.transform.position.y)
            {
                float curPlayerYpos = player.transform.position.y;

                currentTime += Time.deltaTime;
                if (currentTime >= LerpTime) currentTime = LerpTime;

                // << : 보간 수정 - SmoothStep
                float t = currentTime / LerpTime;
                t = Mathf.Sin(t * Mathf.PI * 0.5f);

                lerpYpos = Mathf.Lerp(initYpos, curPlayerYpos, t);
                if (lerpYpos > curPlayerYpos) lerpYpos = curPlayerYpos;


                lookatBackObj.position = new Vector3(lookatBackObj.transform.position.x, lerpYpos + lookatObjOriginY, lookatBackObj.transform.position.z);
                followObj.position = new Vector3(followObj.transform.position.x, lerpYpos, followObj.transform.position.z);

                yield return null;
            }
            lookatBackObj.position = new Vector3(lookatBackObj.transform.position.x, player.transform.position.y + lookatObjOriginY, lookatBackObj.transform.position.z);
            followObj.position = new Vector3(followObj.transform.position.x, player.transform.position.y, followObj.transform.position.z);
                  
            endCor = true;
            isJumpAfterSet = true;
        }
     

        IEnumerator LerpAfter(float LerpTime) // 점프가 이루어진 후, 점프전 Player Pos의 Y값과 점프 후 Player Pos 의 Y값이 지정한 오차를 넘을시, 오차에 비례한 시간동안 보간이 이루어진다
        {
            Debug.Log("호출 - 보간 코루틴! ==============================================");
            Debug.Log("호출 확인용 : " + followObj.position.y * 100);
            float currentTime = 0;

            float initYpos = originPlayerY; // 점프 시작시 플레이어 Y 값
            float lerpYpos;  // 보간이 이루어지고 있는 y 값
            endCor = false;

            float delayTime = 0.1f;
            yield return new WaitForSeconds(delayTime);
            
            if (initYpos < player.transform.position.y)
            {
                //Debug.Log("카메라 - 상승 점프 보간 시작!");
                //Debug.Log("카메라 - 점프 전 플레이어 포지션 : " + initYpos);
                //Debug.Log("카메라 - 점프 후 플레이어 포지션 : " + player.transform.position.y);
                //Debug.Log("======================================================");
                //Debug.Log("카메라 - 보간 전 포지션 look : " + lookatBackObj.position.y);
                //Debug.Log("카메라 - 보간 전 포지션 follow : " + followObj.position.y);
                //Debug.Log("======================================================");

                lerpYpos = initYpos;
                while (lerpYpos < player.transform.position.y)
                {
                    float curPlayerYpos = player.transform.position.y;

                    currentTime += Time.deltaTime;
                    if (currentTime >= LerpTime) currentTime = LerpTime;

                    // << : 보간 수정 - SmoothStep
                    float t = currentTime / LerpTime;
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);

                    lerpYpos = Mathf.Lerp(initYpos, curPlayerYpos, t);
                    if (lerpYpos > curPlayerYpos) lerpYpos = curPlayerYpos;

                    //Debug.Log(lerpYpos);

                    lookatBackObj.position = new Vector3(lookatBackObj.transform.position.x, lerpYpos + lookatObjOriginY, lookatBackObj.transform.position.z);
                    followObj.position = new Vector3(followObj.transform.position.x, lerpYpos, followObj.transform.position.z);

                    yield return null;
                }
                lookatBackObj.position = new Vector3(lookatBackObj.transform.position.x, player.transform.position.y + lookatObjOriginY, lookatBackObj.transform.position.z);
                followObj.position = new Vector3(followObj.transform.position.x, player.transform.position.y, followObj.transform.position.z);
                //Debug.Log("======================================================");
                //Debug.Log("카메라 - 보간 후 포지션 look : " + lookatBackObj.position.y);
                //Debug.Log("카메라 - 보간 후 포지션 follow : " + followObj.position.y);

            }   
            else
            {
                //Debug.Log("카메라 - 하강 점프 보간 시작!");
                //Debug.Log("카메라 - 점프 전 플레이어 포지션 : " + initYpos);
                //Debug.Log("카메라 - 점프 후 플레이어 포지션 : " + player.transform.position.y);
                //Debug.Log("======================================================");
                //Debug.Log("카메라 - 보간 전 포지션 look : " + lookatBackObj.position.y);
                //Debug.Log("카메라 - 보간 전 포지션 follow : " + followObj.position.y);
                //Debug.Log("======================================================");
                                
                lerpYpos = initYpos;
                while (lerpYpos > player.transform.position.y)
                {
                    float curPlayerYpos = player.transform.position.y;

                    currentTime += Time.deltaTime;
                    if (currentTime >= LerpTime) currentTime = LerpTime;

                    // << : 보간 수정 - SmoothStep
                    float t = currentTime / LerpTime;
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);

                    lerpYpos = Mathf.Lerp(initYpos, curPlayerYpos, t);
                    if (lerpYpos < curPlayerYpos) lerpYpos = curPlayerYpos;

                    //Debug.Log(lerpYpos);
                    lookatBackObj.position = new Vector3(lookatBackObj.transform.position.x, lookatObjOriginY + lerpYpos, lookatBackObj.transform.position.z);
                    followObj.position = new Vector3(followObj.transform.position.x, lerpYpos, followObj.transform.position.z);

                    //Debug.Log("카메라 - 보간 중 포지션 look : " + lookatBackObj.position.y);
                    //Debug.Log("카메라 - 보간 중 포지션 follow : " + followObj.position.y);
                    yield return null;
                }
                lookatBackObj.position = new Vector3(lookatBackObj.transform.position.x, player.transform.position.y + lookatObjOriginY, lookatBackObj.transform.position.z);
                followObj.position = new Vector3(followObj.transform.position.x, player.transform.position.y, followObj.transform.position.z);
                //Debug.Log("======================================================");
                //Debug.Log("카메라 - 보간 후 포지션 look : " + lookatBackObj.position.y);
                //Debug.Log("카메라 - 보간 후 포지션 follow : " + followObj.position.y);

            }
            endCor = true;
            isJumpAfterSet = true;
        }

        

        public void ShakeCamera(bool isShake) // MagnifyingGlass 에서 센드메시지로 호출
        {
            if (!canShake) return;

            if (isShake)
            {
                if (wasShaked) return;

                foreach(CinemachineBasicMultiChannelPerlin CBMCP in listCBMCP)
                {
                    CBMCP.m_AmplitudeGain = AmplitudeGain;
                    CBMCP.m_FrequencyGain = FrequebctGain;
                }
                wasShaked = true;
            }
            else
            {              
                if (!wasShaked) return;

                wasShaked = false;

                isShakedFade = true;

                float fadeLerpTime = 0.3f;

                StartCoroutine(ShakeFadeOut(fadeLerpTime));
            }          
        }

        IEnumerator ShakeFadeOut(float LerpTime)
        {
            float initialVlaue = listCBMCP[0].m_AmplitudeGain;
            float currentTime = 0;

            while (initialVlaue > 0)
            {
                initialVlaue -= Time.deltaTime * (AmplitudeGain);

                currentTime += Time.deltaTime;
                if (currentTime >= LerpTime) currentTime = LerpTime;

                float curValue = Mathf.Lerp(initialVlaue, 0, currentTime / LerpTime);

                if (curValue < 0)
                    curValue = 0;

                foreach (CinemachineBasicMultiChannelPerlin CBMCP in listCBMCP)
                {
                    CBMCP.m_AmplitudeGain = curValue;
                    CBMCP.m_FrequencyGain = curValue;
                }

                yield return null;
            }
            isShakedFade = false;
        }

        void InitShakeFade()
        {
            float initialVlaue = 0;

            foreach (CinemachineBasicMultiChannelPerlin CBMCP in listCBMCP)
            {
                CBMCP.m_AmplitudeGain = initialVlaue;
                CBMCP.m_FrequencyGain = initialVlaue;
            }
            isShakedFade = false;
        }

        void OnOffCamera(CinemachineVirtualCameraBase curCam) // 매개변수로 받은 카메라 외에 다른 카메라는 끔
        {
            foreach (CinemachineVirtualCameraBase cam in camList)
            {
                if (cam == curCam)
                {
                    curCam.enabled = true;
                    continue;
                }
                cam.enabled = false;
            }
        }

        public Camera FindCamera() // 캐릭터에 따라 자기 카메라를 찾아 세팅
        {
            if (this.gameObject.CompareTag("Nella"))
            {           
                if(!mainCam)
                {
                    mainCam = GameObject.FindGameObjectWithTag("NellaCamera").GetComponent<Camera>();
                    mainCam.GetComponent<UniversalAdditionalCameraData>().SetRenderer(0);

                    cinemachineBrain = mainCam.GetComponent<CinemachineBrain>();
                    CameraManager.Instance.cameras[0] = mainCam;
                    return mainCam;
                }
                else
                {
                    return mainCam;
                }
            }
            else if (this.gameObject.CompareTag("Steady"))
            {            
                if (!mainCam)
                {
                    mainCam = GameObject.FindGameObjectWithTag("SteadyCamera").GetComponent<Camera>();
                    mainCam.GetComponent<UniversalAdditionalCameraData>().SetRenderer(1);
                    cinemachineBrain = mainCam.GetComponent<CinemachineBrain>();
                    CameraManager.Instance.cameras[1] = mainCam;
                    return mainCam;
                }
                else
                {
                    return mainCam;
                }
            }
            return mainCam;
        }     

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // Photon q
        {
            //if (stream.IsWriting) {}
            //else {}
        }
    }
}
