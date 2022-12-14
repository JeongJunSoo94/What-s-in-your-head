using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 시네머신 카메라 컨트롤러
/// 현재 구현 View : BackView, AinView
/// </summary>

using Photon.Pun;
using Photon.Realtime;
using System.Linq;

using Cinemachine;

using JCW.UI.Options.InputBindings;
using YC.YC_CameraManager;
using UnityEngine.Rendering.Universal;

using KSU;

namespace YC.YC_Camera
{
    public class CameraControllerOrigin : MonoBehaviour, IPunObservable
    {

        // Photon 
        PhotonView pv;
        PhotonTransformView pvTransform;

        // Player Cam
        //public Camera mainCam;
        public Camera mainCam { get; private set; }

        // Cinemachine
        public CinemachineBrain cinemachineBrain;
        List<CinemachineVirtualCameraBase> camList;
        CinemachineVirtualCameraBase backCam;
        CinemachineVirtualCameraBase wideCam;
        CinemachineVirtualCameraBase sholderCam;
        CinemachineVirtualCameraBase topCam;
        CinemachineVirtualCameraBase wallCam;

        enum CamState { back, wide, sholder, top, wall };
        CamState curCam;
        CamState preCam;
        //float originCurVirtualCam_XAxis;
        //float originCurVirtualCam_YAxis;
        //float originPreVirtualCam_XAxis;
        //float originPreVirtualCam_YAxis;


        //[SerializeField] Transform lookatObj_BackView;
        //[SerializeField] Transform lookatObj_SholderView;
        //[SerializeField] Transform followObj;

        //bool isOriginBlending = false;

        //Vector3 originCurCam_Pos = new Vector3();

        // Clone 

        CamState curCam_Clone;
        //[SerializeField] CamState blendingCam_Clone; // 블렌딩 목표 카메라
        CamState blendingPrevCam_Clone; // 블렌딩 이전목표 카메라(블렌딩 중 다시 카메라 변경)
        //[SerializeField] CamState preCam_Clone;


        //bool isBlendStart_Clone = false; // 클론 시네머신의 블렌드가 시작 시점 true
        //bool isBlending_Clone = false;  // 클론 시네머신 블렌딩 중인지
        //bool isActiveCB_Clone = false;  // 클론 시네커신 브레인 enable 여부
        //bool isActiveBT_Clone = false;  // 클론 시네커신 브레인 타겟 카메라 set 여부

        // Option
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

        void Awake()
        {
            pv = GetComponent<PhotonView>();
            pvTransform = GetComponent<PhotonTransformView>();

            if (pv) pv.ObservedComponents.Add(this);

            camList = new List<CinemachineVirtualCameraBase>();

            // 리스트 인덱스 순서랑 Enum 순서 맞춰줘야 한다.
            backCam = this.gameObject.transform.Find("Cine_backCam").GetComponent<CinemachineVirtualCameraBase>();
            wideCam = this.gameObject.transform.Find("Cine_wideCam").GetComponent<CinemachineVirtualCameraBase>();
            sholderCam = this.gameObject.transform.Find("Cine_sholderCam").GetComponent<CinemachineVirtualCameraBase>();
            topCam = GameObject.Find("Cine_topCam").GetComponent<CinemachineVirtualCameraBase>();
            wallCam = this.gameObject.transform.Find("Cine_wallCam").GetComponent<CinemachineVirtualCameraBase>();

            camList.Add(backCam);
            camList.Add(wideCam);
            camList.Add(sholderCam);
            camList.Add(topCam);
            camList.Add(wallCam);

            curCam = new CamState();
            preCam = new CamState();

            curCam_Clone = new CamState();
            //preCam_Clone = new CamState();
            //blendingCam_Clone = new CamState();           


            sholderViewMaxY = sholderCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxValue;



            if (!pv.IsMine)
            {
                curCam_Clone = CamState.back;
                //preCam_Clone = curCam_Clone;
                //blendingCam_Clone = CamState.back;
                OnOffCamera(camList[(int)curCam_Clone]);
            }
            else
            {
                curCam = CamState.back;
                preCam = curCam;
                OnOffCamera(camList[(int)curCam]);
            }

            FindCamera();

            player = this.gameObject.GetComponent<PlayerController>();


            // 카메라 FOV 초기화 (해당 시네머신 카메라는 Commone Lens 설정 필요)
            if (pv.IsMine)
                mainCam.fieldOfView = camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView;
            else
                mainCam.fieldOfView = camList[(int)curCam_Clone].GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView;

            // 만약 둘이서 멀티 진행했는데 Nella Remote 블렌딩 진행중 Steady Owner의 마우스 값이 Nella Remote에게 들어온다면
            // 시야각이 카메라끼리의 블렌딩 불가, 시야각을 위한 별도 설정 필요
            if (!pv.IsMine) cinemachineBrain.enabled = false;

            listCBMCP = new List<CinemachineBasicMultiChannelPerlin>();

            InitDefault();

        }



        void FixedUpdate()
        {
            //BlockMouseControlInBlending();

            if (!pv.IsMine)
            {
                //CheckStartBlend_Clone();

                //if (!isBlendStart_Clone && cinemachineBrain.enabled && !cinemachineBrain.IsBlending)
                //{
                //    cinemachineBrain.enabled = false;
                //}

                //SetCameraBlned_Clone();               
            }
            else
            {
                //BlockMouseControlInBlending();
                SetCamera();
                SetAimYAxis();
            }
        }

       
        void InitDefault()
        {    
            // backView_MouseSensitivity, sholderView_MouseSensitivity 두 변수는 1~100의 값을 갖는다 (찬우형 게임매니저로 부터 받아옴)
            // 시네머신에서 최적의 마우스 스피드는 X : 100 ~ 300, Y : 1 ~ 3 정도이다.
            // 감도 변수를 시네머신에 맞게 변환하여 세팅해준다       

            // << : 감도 관련
            int defaulyX = 100;
            int defaultY = 1;

            if (backView_MouseSensitivity == 0) backView_MouseSensitivity = 25;

            backCam.GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = defaulyX + backView_MouseSensitivity * 4;
            backCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = defaultY + backView_MouseSensitivity * 0.04f;

            if (sholderView_MouseSensitivity == 0) sholderView_MouseSensitivity = 25;

            sholderCam.GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = defaulyX + sholderView_MouseSensitivity * 4;
            sholderCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = defaultY + sholderView_MouseSensitivity * 0.04f;

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


        //void BlockMouseControlInBlending() // 블렌딩 도중 마우스 입력을 막는다
        //{
        //    if (cinemachineBrain.IsBlending)
        //    {
        //        camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_XAxis.m_InputAxisName = "";
        //        camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.m_InputAxisName = "";
        //        camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_XAxis.m_InputAxisName = "";
        //        camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.m_InputAxisName = "";
        //    }
        //    else
        //    {
        //        camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_XAxis.m_InputAxisName = "Mouse X";
        //        camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.m_InputAxisName = "Mouse Y";
        //        camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_XAxis.m_InputAxisName = "Mouse X";
        //        camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.m_InputAxisName = "Mouse Y";
        //    }
        //}

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
                    
                    // << : 수정
                    camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value = camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value;

                    //if(!cinemachineBrain.IsBlending)
                    //    camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value = 0.5f;

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

                    // << : 수정
                    camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value = camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value;

                    //if (!cinemachineBrain.IsBlending)
                    //    camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value = 0.5f;

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
            Debug.Log("들어옴!");
            
            if(pv.IsMine)
            {
                preCam = curCam;
                curCam = CamState.top;

                //Debug.Log("로그 확인");
                OnOffCamera(topCam);

                camList[(int)curCam].GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = 60;
            }
            else
            {
                mainCam.fieldOfView = 60;
            }
            

            //Debug.Log("디펜스 모드 세팅 - 카메라 컨트롤러!");

        }

        // << : 스테디 빔 사용시, 스테디 Aim Attack State에서 호출
        public void SetSteadyBeam(bool isLock)
        {
            // << : 탑뷰라면 리턴(준수형)
            if (GameManager.Instance.isTopView) return;

            if(isLock)
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


        public void ShakeCamera(bool isShake) // MagnifyingGlass 에서 센드메시지로 호출
        {
            // if(옵션에서 흔들림 해제하면) return;

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


        //void CheckStartBlend_Clone() // Owner의 가상 카메라 전환 여부 체크 
        //{
        //    if (blendingPrevCam_Clone != blendingCam_Clone)
        //    {
        //        isBlendStart_Clone = true;
        //    }
        //    blendingPrevCam_Clone = blendingCam_Clone;
        //}

        //void SetCameraBlned_Clone() // Clone의 Cinemachine Brain과 Photon을 상황에 맞게 On Off
        //{
        //    if (isBlendStart_Clone && !cinemachineBrain.IsBlending) // 블렌딩 시작시
        //    {
        //        // 1회 호출 : Remote의 현재 블렌딩 카메라에 Owner 카메라 값을 넣어준 뒤, Remote의 시네머신을 작동 시킨다.
        //        if (isActiveCB_Clone)
        //        {
        //            if (curCam_Clone == CamState.back || curCam_Clone == CamState.wide || curCam_Clone == CamState.sholder)
        //            {
        //                camList[(int)blendingCam_Clone].GetComponent<CinemachineFreeLook>().m_XAxis.Value
        //                            = originCurVirtualCam_XAxis;

        //                camList[(int)blendingCam_Clone].GetComponent<CinemachineFreeLook>().m_YAxis.Value
        //                            = originCurVirtualCam_YAxis;

        //                camList[(int)blendingCam_Clone].GetComponent<CinemachineFreeLook>().transform.position = originCurCam_Pos;
        //            }
        //            preCam_Clone = curCam_Clone;
        //            curCam_Clone = blendingCam_Clone;

        //            OnOffCamera(camList[(int)curCam_Clone]);

        //            isBlendStart_Clone = false;
        //            isActiveCB_Clone = false;
        //        }

        //        // 1회 호출 :  Remote의 Cinemachine Brain을 켜주고, Owner의 이전 카메라 Value 값을 받아 Remote의 이전 카메라 Value에 넣어준다.
        //        if (isBlendStart_Clone)
        //        {
        //            cinemachineBrain.enabled = true;

        //            if (curCam_Clone == CamState.back || curCam_Clone == CamState.wide || curCam_Clone == CamState.sholder)
        //            {
        //                camList[(int)curCam_Clone].GetComponent<CinemachineFreeLook>().m_XAxis.Value
        //                             = originPreVirtualCam_XAxis;
        //                camList[(int)curCam_Clone].GetComponent<CinemachineFreeLook>().m_YAxis.Value
        //                             = originPreVirtualCam_YAxis;
        //            }
        //            isActiveCB_Clone = true;
        //        }
        //    }
        //    else if (isBlendStart_Clone && cinemachineBrain.IsBlending) // 블렌딩 도중 취소시
        //    {
        //        ////preCam_Clone = curCam_Clone;
        //        ////curCam_Clone = blendingCam_Clone;
        //        //OnOffCamera(camList[(int)blendingCam_Clone]);
        //        //isBlendStart_Clone = false;

        //        // 1회 호출 : Remote의 현재 블렌딩 카메라에 Owner 카메라 값을 넣어준 뒤, Remote의 시네머신을 작동 시킨다.
        //        if (isActiveCB_Clone)
        //        {
        //            if (curCam_Clone == CamState.back || curCam_Clone == CamState.wide || curCam_Clone == CamState.sholder)
        //            {
        //                camList[(int)blendingCam_Clone].GetComponent<CinemachineFreeLook>().m_XAxis.Value
        //                            = originCurVirtualCam_XAxis;

        //                camList[(int)blendingCam_Clone].GetComponent<CinemachineFreeLook>().m_YAxis.Value
        //                            = originCurVirtualCam_YAxis;

        //                camList[(int)blendingCam_Clone].GetComponent<CinemachineFreeLook>().transform.position = originCurCam_Pos;
        //            }
        //            preCam_Clone = curCam_Clone;
        //            curCam_Clone = blendingCam_Clone;

        //            OnOffCamera(camList[(int)curCam_Clone]);

        //            isBlendStart_Clone = false;
        //            isActiveCB_Clone = false;
        //        }

        //        // 1회 호출 :  Remote의 Cinemachine Brain을 켜주고, Owner의 이전 카메라 Value 값을 받아 Remote의 이전 카메라 Value에 넣어준다.
        //        if (isBlendStart_Clone)
        //        {
        //            if (curCam_Clone == CamState.back || curCam_Clone == CamState.wide || curCam_Clone == CamState.sholder)
        //            {
        //                camList[(int)curCam_Clone].GetComponent<CinemachineFreeLook>().m_XAxis.Value
        //                             = originPreVirtualCam_XAxis;
        //                camList[(int)curCam_Clone].GetComponent<CinemachineFreeLook>().m_YAxis.Value
        //                             = originPreVirtualCam_YAxis;
        //            }
        //            isActiveCB_Clone = true;
        //        }


        //    }
        //}

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
                //mainCam = GameObject.FindGameObjectWithTag("NellaCamera").GetComponent<Camera>();
                //mainCam.GetComponent<UniversalAdditionalCameraData>().SetRenderer(0);

                //cinemachineBrain = mainCam.GetComponent<CinemachineBrain>();
                //CameraManager.Instance.cameras[0] = mainCam;

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
                //mainCam = GameObject.FindGameObjectWithTag("SteadyCamera").GetComponent<Camera>();
                //mainCam.GetComponent<UniversalAdditionalCameraData>().SetRenderer(1);
                //cinemachineBrain = mainCam.GetComponent<CinemachineBrain>();
                //CameraManager.Instance.cameras[1] = mainCam;
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
            //if (stream.IsWriting)
            //{
            //    stream.SendNext(cinemachineBrain.IsBlending);
            //    stream.SendNext(preCam);
            //    stream.SendNext(curCam);
            //    stream.SendNext(camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_XAxis.Value);
            //    stream.SendNext(camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value);
            //    stream.SendNext(camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_XAxis.Value);
            //    stream.SendNext(camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value);
            //    stream.SendNext(camList[(int)curCam].GetComponent<CinemachineFreeLook>().transform.position);
            //}
            //else
            //{
            //    isOriginBlending = (bool)stream.ReceiveNext();
            //    preCam_Clone = (CamState)stream.ReceiveNext();
            //    blendingCam_Clone = (CamState)stream.ReceiveNext();
            //    originPreVirtualCam_XAxis = (float)stream.ReceiveNext();
            //    originPreVirtualCam_YAxis = (float)stream.ReceiveNext();
            //    originCurVirtualCam_XAxis = (float)stream.ReceiveNext();
            //    originCurVirtualCam_YAxis = (float)stream.ReceiveNext();
            //    originCurCam_Pos = (Vector3)stream.ReceiveNext();
            //}
        }
    }
}
