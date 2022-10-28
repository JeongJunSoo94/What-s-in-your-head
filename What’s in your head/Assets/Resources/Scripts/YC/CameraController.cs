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
        [SerializeField] float sholderAxisY_MaxUp = 0.3f;
        [Header("[Sholder View Y궤도 Down 제한 값]")]
        [SerializeField] float sholderAxisY_MaxDown = 0.5f;


        [Header("[스테디 빔, 카메라 흔들림 진폭 크기]")]
        [SerializeField] float AmplitudeGain = 3f;
        [Header("[스테디 빔, 카메라 흔들림 빈도]")]
        [SerializeField] float FrequebctGain = 3f;
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
        float normalJumpLerpTime = 2f;


        PlayerState playerState;


        float backViewFOV = 40;


        // >> : 점프시 Follow, LookAt 관련
        Transform followObj;
        Transform lookatBackObj;
        Transform lookatSholderObj;

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

            if (isJumping)
                NormalJumpCameraSet();

            SetCamera();
            SetAimYAxis();

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
            SetSensitivity(backView_MouseSensitivity, sholderView_MouseSensitivity);


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

        public void SetSensitivity(float backSensitivity, float sholdervity)
        {
            // << : 감도 관련
            int defaulyX = 100;
            int defaultY = 1;

            if (backView_MouseSensitivity == 0) backSensitivity = 25;

            backCam.GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = defaulyX + backSensitivity * 4;
            backCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = defaultY + backSensitivity * 0.04f;

            if (sholderView_MouseSensitivity == 0) sholdervity = 25;

            sholderCam.GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = defaulyX + sholdervity * 4;
            sholderCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = defaultY + sholdervity * 0.04f;
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





        public void NormalJumpCameraInit(bool On)
        {
            if (On) // 일반 점프 시작
            {
                isJumping = true;

                orgLookY = lookatBackObj.transform.position.y;
                orgFollowY = followObj.transform.position.y;

              
                //Debug.Log("카메라 - 세팅 시작!");
            }
            else if (!On)
            {
                //Debug.Log("들어옴");
                if (playerState.IsJumping && playerState.IsAirJumping && !playerState.IsAirDashing) // 공중 점프
                {
                    if (player.transform.position.y < lookatBackObj.transform.position.y) // 룩앳 오브젝트보다 낮은 위치에서 이중 점프를 시도한다면 블락
                    {
                        isJumping = true;
                        //Debug.Log("입구컷1");
                        //Debug.Log("Player Y : " + player.transform.position.y);
                        //Debug.Log("Look Y : " + lookatBackObj.transform.position.y);
                        return;
                    }
                    //isJumping = false;
                    //StartCoroutine(NormalJumpLerp(normalJumpLerpTime));
                }
                else
                {
                    Debug.Log("Follow : " + followObj.transform.position.y);
                    Debug.Log("origin : " + orgFollowY);

                    if (followObj.transform.position.y - orgFollowY > avgDif)
                    {
                        StartCoroutine(TestCoroutine(normalJumpLerpTime));
                        return;
                    }

                    //if (playerState.IsJumping && playerState.IsAirJumping && playerState.IsAirDashing) return;

                    //Debug.Log("땅 도착시 들어오는 곳");
                    isJumping = false;
                    lookatBackObj.localPosition = new Vector3(0, originLocalLookAtY, 0);
                    followObj.localPosition = new Vector3(0, originLocalFollowY, 0);



                    
                }
            }
        }

        public void NormalJumpCameraSet()
        {
            float lookValue = orgLookY - lookatBackObj.transform.position.y;
            float followValue = orgFollowY - followObj.transform.position.y;


            lookatBackObj.position = new Vector3(lookatBackObj.transform.position.x, lookatBackObj.position.y + lookValue, lookatBackObj.transform.position.z);
            followObj.position = new Vector3(followObj.transform.position.x, followObj.position.y + followValue, followObj.transform.position.z);

        }
        float avgDif = 0.1f;

        IEnumerator TestCoroutine(float LerpTime)
        {
            float currentTime = 0;

            float initValue = orgFollowY; // 점프 당시 y 값
            float curValue = followObj.transform.position.y; // 현재 follow y 값 ( 더 높음)

            Debug.Log("코루틴 들어옴!");

            while (followObj.transform.position.y < curValue)
            {
                currentTime += Time.deltaTime;
                if (currentTime >= LerpTime) currentTime = LerpTime;

                
                float yLerp = Mathf.Lerp(initValue, curValue, currentTime / LerpTime);
                if (curValue < yLerp) yLerp = curValue;

                lookatBackObj.position = new Vector3(lookatBackObj.transform.position.x, yLerp + orgLookY, lookatBackObj.transform.position.z);

                followObj.position = new Vector3(followObj.transform.position.x, yLerp, followObj.transform.position.z);
                yield return null;
            }
            lookatBackObj.localPosition = new Vector3(0, originLocalLookAtY, 0);
            followObj.localPosition = new Vector3(0, originLocalFollowY, 0);
        }

        

        IEnumerator NormalJumpLerp(float LerpTime)
        {

            //Debug.Log("코루틴 들어옴");

            float initValue = followObj.transform.position.y;
            float currentTime = 0;

            while (player.transform.position.y > followObj.transform.position.y)
            {
                currentTime += Time.deltaTime;
                if (currentTime >= LerpTime) currentTime = LerpTime;

                float yLerp = Mathf.Lerp(initValue, player.transform.position.y, currentTime / LerpTime);
                if (player.transform.position.y < followObj.transform.position.y) yLerp = player.transform.position.y;

                lookatBackObj.position = new Vector3(lookatBackObj.transform.position.x, yLerp + orgLookY, lookatBackObj.transform.position.z);

                followObj.position = new Vector3(followObj.transform.position.x, yLerp, followObj.transform.position.z);
                yield return null;
            }
            lookatBackObj.localPosition = new Vector3(0, originLocalLookAtY, 0);
            followObj.localPosition = new Vector3(0, originLocalFollowY, 0);
            Debug.Log("Look : " + lookatBackObj.transform.position.y);
            Debug.Log("Follow : " + followObj.transform.position.y);
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

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // Photon 
        {
            //if (stream.IsWriting) {}
            //else {}

        }
    }
}
