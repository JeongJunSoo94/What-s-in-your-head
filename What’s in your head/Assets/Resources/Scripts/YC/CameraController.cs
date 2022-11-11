using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
using YC.CameraManager_;
using UnityEngine.Rendering.Universal;
using KSU;
using System;
using System.Reflection;

namespace YC.Camera_
{
    public class CameraController : MonoBehaviour, IPunObservable
    {      
        // 플레이어 및 포톤
        PlayerController playerController;
        GameObject player;
        PlayerState playerState;
        PhotonView pv;

        // 메인 카메라
        public Camera mainCam { get; private set; }

        // 메인 카메라의 시네머신 브레인
        public CinemachineBrain cinemachineBrain { get; private set; }

        // 가상 카메라 리스트
        List<CinemachineVirtualCameraBase> camList;

        // 가상카메라
        CinemachineVirtualCameraBase backCam;
        CinemachineVirtualCameraBase sholderCam;
        CinemachineVirtualCameraBase topCam;
        CinemachineVirtualCameraBase sideCam;

        // 가상 카메라 enum State
        enum CamState { back, sholder};
        CamState curCam; 
        CamState preCam; 

        // ============  카메라 감도 설정  ============ //
        [Header("[Back View 카메라 마우스 감도]")]
        [SerializeField] [Range(0, 100)] float backView_MouseSensitivity = 25f;

        [Header("[Sholder View 카메라 마우스 감도]")]
        [SerializeField] [Range(0, 100)] float sholderView_MouseSensitivity = 10f;

        float curSholderMaxSpeedY;

        [Space][Space]

        // ============  Aim View Y축 궤도 제한  ============ //
        [Header("[Sholder View Y궤도 Up 제한 값]")]
        [SerializeField] [Range(0, 1)] float sholderAxisY_MaxUp = 0.25f;

        [Header("[Sholder View Y궤도 Down 제한 값]")]
        [SerializeField] [Range(0, 1)] float sholderAxisY_MaxDown = 0.5f;

        float sholderViewMaxY;
        [Space][Space]

        // ============  스테디 빔 사용시 카메라 흔들림  ============ //
        [Header("[스테디 빔, 카메라 흔들림 진폭 크기]")]
        [SerializeField] [Range(0, 5)] float AmplitudeGain = 1f;

        [Header("[스테디 빔, 카메라 흔들림 빈도]")]
        [SerializeField] [Range(0, 5)] float FrequebctGain = 1.5f;

        List<CinemachineBasicMultiChannelPerlin> listSteadyCBMCP;

        [HideInInspector] public bool canShake = true; // 옵션 스테디 흔들림 여부
        bool wasShaked = false;
        bool isShakedFade = false;
        [Space] [Space]

        // ============  FOV  ============ //
        float backViewFOV = 50;
        //float sholderViewFOV = 50;
        float topViewFOV = 60;
        float sideScrollViewFOV = 60;

        // ============  사이드뷰 변수  ============ //
        [Header("[사이드 뷰, 카메라 흔들림 진폭 크기]")]
        [SerializeField] [Range(0, 5)] float AmplitudeGainSide = 2f;

        [Header("[사이드 뷰, 카메라 흔들림 빈도]")]
        [SerializeField] [Range(0, 5)] float FrequebctGainSide = 1f;

        [Header("[사이드 뷰, 카메라 흔들림 지속 시간]")]
        [SerializeField] [Range(0, 5)] float ShakeTimeSide = 2f;

        CinemachineBasicMultiChannelPerlin SideCBMCP;

        bool isSideView = false;
        List<Transform> targets;
        GameObject lookAndFollow;
        float minZoom;
        float maxZoom;
        [Space] [Space]

        // ============  미로뷰 변수  ============ //
        string mazeCamTag = "Cine_MazeCam";
        GameObject mazeCamObj;
        CinemachineVirtualCamera mazeCineCam;
        CinemachineCollider mazeCineCamCol;

        // ============  점프 보간 변수들  ============ //
        [Header("[점프 후, 플랫폼 착지시 보간 시간]")]
        [SerializeField] [Range(0, 3)] float platformLerpTime = 0.7f;
        [Space] [Space]
        Transform followObj; // >> : 점프시 Follow, LookAt 관련
        public Transform lookatBackObj { get; private set; } //< : 레일 액션에서 사용
        float lookatObjOriginY;

        float originPlayerY; // << : 플레이어가 점프하기 전, 오브젝트들의 높이값
        float orgLookY;
        float orgFollowY;

        bool wasEndSet = false;
        bool isJumping = false;
        bool isLerp = false;
        bool isAirJumpLerpEnd = false;
        bool wasAirJump = false;
        bool isRiding = false; // 라이딩 시작부터, 그라운드 착지시까지 true
        bool isLower = false; // 일반 점프후 플레이어가 점프 시작 전 높이보다 낮다면 true


        // ============  인스펙터 프리팹  ============ //

        [SerializeField] CinemachineVirtualCameraBase CineBack;
        [SerializeField] CinemachineVirtualCameraBase CineSholder;
        [SerializeField] GameObject CineLookObj_Back;
        [SerializeField] GameObject CineFollowObj_Back;
        [SerializeField] NoiseSettings NoiseProfile;
        [Header("[Aim UI]")] 
        public GameObject aimUI;



        // ============  디버그 로그  ============ //
        bool isDebugLog = false;
        void DebugLogToggle()
        {
            //var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            //var type = assembly.GetType("UnityEditor.LogEntries");
            //var method = type.GetMethod("Clear");
            //method.Invoke(new object(), null);

            //isDebugLog = !isDebugLog;
        }


        void Awake()  
        {
            pv = GetComponent<PhotonView>();
            if (pv) pv.ObservedComponents.Add(this);

            playerController = this.gameObject.GetComponent<PlayerController>();
            playerState = this.gameObject.GetComponent<PlayerState>();
            player = this.gameObject;

            FindCamera();

            InitVirtualCamera();
            InitCinemachineRig();
            InitDefault();
            InitUI();
        }


        void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.R))
                DebugLogToggle();

            if (isDebugLog) Debug.Log("FixedUpdate");
           
            if (!pv.IsMine) return;

            if (playerState.isInMaze)
                return;

            if (isSideView)
            {
                //if (Input.GetKeyDown(KeyCode.Keypad9))
                //    ShakeCameraInSideView();

                MoveInSideView();
                ZoomInSideView();
                return;
            }

            SetCamera();
            SetAimYAxis();

            if (isRiding)
            {
                RidingCamera();
                return;
            }

            if(playerState.isCumstomJumping)
            {
                if (isLerp) return;

                FollowPlayer();
                return;
            }

            if (isJumping && !isLerp)
            {
                if (!wasEndSet)
                    NormalJump_FixY();

                if (!playerState.IsAirJumping && !isLower)
                    CheckLowerPlayer();

                if ((playerState.IsAirJumping || wasAirJump) && isAirJumpLerpEnd && !playerState.IsAirDashing)
                    AirJumpPlayerFollow();

                if (wasEndSet && playerState.IsAirDashing)
                    AirDashFollow();

                if (isLower) LowerPlayerFollow();
            }
            else if (!isJumping && !isLerp)
            {
                SetCineObjPos();
            }
        }


        // ====================  [Awake에서 진행하는 초기화]  ==================== //

        void InitUI()  
        {
            if (!pv.IsMine) return;
                aimUI = Instantiate(aimUI);
        }

        void InitVirtualCamera() // Virtual Camera 생성 및 초기화  
        {           
            if (!pv.IsMine) return;

            camList = new List<CinemachineVirtualCameraBase>();

            backCam = Instantiate(CineBack, Vector3.zero, Quaternion.identity).GetComponent<CinemachineVirtualCameraBase>();
            sholderCam = Instantiate(CineSholder, Vector3.zero, Quaternion.identity).GetComponent<CinemachineVirtualCameraBase>();

            if(this.gameObject.CompareTag("Nella"))
            {
                string layerName = "NellaCam";
                string ignoreTag = "Nella";
                backCam.gameObject.layer = LayerMask.NameToLayer(layerName);
                sholderCam.gameObject.layer = LayerMask.NameToLayer(layerName);
                backCam.GetComponent<CinemachineCollider>().m_IgnoreTag = ignoreTag;
                sholderCam.GetComponent<CinemachineCollider>().m_IgnoreTag = ignoreTag;
            }
            else
            {
                string layerName = "SteadyCam";
                string ignoreTag = "Steady";
                backCam.gameObject.layer = LayerMask.NameToLayer(layerName);
                sholderCam.gameObject.layer = LayerMask.NameToLayer(layerName);
                backCam.GetComponent<CinemachineCollider>().m_IgnoreTag = ignoreTag;
                sholderCam.GetComponent<CinemachineCollider>().m_IgnoreTag = ignoreTag;
            }

            followObj = Instantiate(CineFollowObj_Back, player.transform.position + CineFollowObj_Back.transform.position, player.transform.rotation).GetComponent<Transform>();
            lookatBackObj = Instantiate(CineLookObj_Back, player.transform.position + CineLookObj_Back.transform.position, player.transform.rotation).GetComponent<Transform>();
            //lookatSholderObj = transform.Find("Cine_lookatObj_Sholder").gameObject.transform;
            lookatObjOriginY = CineLookObj_Back.transform.position.y;

            CinemachineFreeLook Cine_backCam = backCam.GetComponent<CinemachineFreeLook>();
            CinemachineFreeLook Cine_sholderCam = sholderCam.GetComponent<CinemachineFreeLook>();

            Cine_backCam.m_Follow = followObj;
            Cine_backCam.m_LookAt = lookatBackObj;
            Cine_backCam.m_XAxis.Value = 0.5f;
            Cine_backCam.m_YAxis.Value = 0.5f;

            Cine_sholderCam.m_Follow = followObj;
            Cine_sholderCam.m_LookAt = lookatBackObj;
            Cine_sholderCam.m_XAxis.Value = 0.3f;
            Cine_sholderCam.m_YAxis.Value = 0.3f;

            // 가상 카메라 리스트에 넣어준다.
            camList.Add(backCam);
            camList.Add(sholderCam);

            Cine_backCam.GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView = backViewFOV;
        }

        void InitCinemachineRig() //  Rig로 발생하는 이슈를 막기 위해 Rig를 스크립트로 강제 초기화  
        {
            if (!pv.IsMine) return;

            float zero = 0;
            float offSetYInitValue = 2.5f;

            CinemachineFreeLook backCine = backCam.GetComponent<CinemachineFreeLook>();
            CinemachineFreeLook sholderCine = sholderCam.GetComponent<CinemachineFreeLook>();

            if (mainCam.transform.childCount != 0) // << : Rig 삭제 (MainCam)
            {
                for (int i = 0; i < mainCam.transform.childCount; ++i)
                {
                    Destroy(mainCam.transform.GetChild(i).gameObject);
                }
            }

            //if (backCam.transform.childCount != 0) // << : Rig 삭제 (BackCam)
            //{
            //    for (int i = 0; i < backCam.transform.childCount; ++i)
            //    {
            //        Destroy(backCam.transform.GetChild(i).gameObject);
            //    }
            //}

            //if (sholderCam.transform.childCount != 0) // << : Rig 삭제(SholderCam)
            //{
            //    for (int i = 0; i < sholderCam.transform.childCount; ++i)
            //    {
            //        Destroy(sholderCam.transform.GetChild(i).gameObject);
            //    }
            //}


            for (int i = 0; i < 3; ++i)
            {            
                if(backCine.GetRig(i))
                {
                    //backCine.GetRig(i).AddCinemachineComponent<CinemachineComposer>();
                
                    backCine.GetRig(i).GetCinemachineComponent<CinemachineComposer>().m_HorizontalDamping = zero;
                    backCine.GetRig(i).GetCinemachineComponent<CinemachineComposer>().m_VerticalDamping = zero;

                    backCine.GetRig(i).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XDamping = zero;
                    backCine.GetRig(i).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_YDamping = zero;
                    backCine.GetRig(i).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_ZDamping = zero;
                }
                if(sholderCine.GetRig(i))
                {
                    //sholderCine.GetRig(i).AddCinemachineComponent<CinemachineComposer>();

                    sholderCine.GetRig(i).GetCinemachineComponent<CinemachineComposer>().m_HorizontalDamping = zero;
                    sholderCine.GetRig(i).GetCinemachineComponent<CinemachineComposer>().m_VerticalDamping = zero;

                    sholderCine.GetRig(i).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XDamping = zero;
                    sholderCine.GetRig(i).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_YDamping = zero;
                    sholderCine.GetRig(i).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_ZDamping = zero;
                }
            }

            backCine.GetRig(1).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y = offSetYInitValue; // Back View Middle에서 포지션 내려 잡기
        }

        void InitDefault()  // 기타 초기화  
        {
            mainCam.fieldOfView = backViewFOV;
            if (!pv.IsMine) cinemachineBrain.enabled = false;

            if (!pv.IsMine) return;
      
            // << : 감도 세팅
            Option_SetSensitivity(backView_MouseSensitivity, sholderView_MouseSensitivity);

            // << : Sholder View Y축 제한 적용
            if (sholderAxisY_MaxUp == 0) sholderAxisY_MaxUp = 0.2f;
            if (sholderAxisY_MaxDown == 0) sholderAxisY_MaxDown = 0.5f;

            // << : 스테디 빔 흔들림 설정
            if (this.gameObject.CompareTag("Steady"))
            {
                listSteadyCBMCP = new List<CinemachineBasicMultiChannelPerlin>();

                if (AmplitudeGain == 0) AmplitudeGain = 1;
                if (FrequebctGain == 0) FrequebctGain = 2;
                
                // << : 동적으로 노이즈 프로파일 추가
                if(sholderCam.GetComponent<CinemachineFreeLook>().GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>() == null)
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        sholderCam.GetComponent<CinemachineFreeLook>().GetRig(i).AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                        sholderCam.GetComponent<CinemachineFreeLook>().GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_NoiseProfile 
                            = NoiseProfile;
                    }
                }

                for (int i = 0; i < 3; ++i)
                {
                    listSteadyCBMCP.Add(sholderCam.GetComponent<CinemachineFreeLook>().GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>());
                }

                for (int i = 0; i < 3; ++i)
                {
                    sholderCam.GetComponent<CinemachineFreeLook>().GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
                    sholderCam.GetComponent<CinemachineFreeLook>().GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
                }
            }

            // << : 카메라 State 세팅
            curCam = new CamState();
            preCam = new CamState();

            // << : Sholder View Max Speed를 받아둠
            sholderViewMaxY = sholderCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxValue;

            curCam = CamState.back;
            preCam = curCam;
            OnOffCamera(camList[(int)curCam]);
        }
     
        // ====================  [Option 관련 함수 (찬우형)]  ==================== //

        public void Option_SetShake(bool on) // 스테디 카메라 흔들림 사용 여부  
        {
            canShake = on;
        }

        public void Option_SetSensitivity(float backSensitivity, float sholderSensitivity) // 마우q스 민감도 설정  
        {
            int defaulyX = 200;
            int defaultY = 1;

            if (backView_MouseSensitivity == 0) backSensitivity = 25;

            backCam.GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = defaulyX + backSensitivity * 4;
            backCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = defaultY + backSensitivity * 0.04f;

            if (sholderView_MouseSensitivity == 0) sholderSensitivity = 25;

            sholderCam.GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = defaulyX + sholderSensitivity * 4;
            sholderCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = defaultY + sholderSensitivity * 0.04f;
            curSholderMaxSpeedY = sholderCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed;
        }


        // ====================  [넬라 스테디 공용 함수]  ==================== //
      
        void SetCamera() // 플레이어 State에 따라 카메라 세팅  
        {
            if (GameManager.Instance.isTopView) return;

            if (curCam == CamState.back) // Back View -> Sholder View
            {
                if (playerController.characterState.aim) 
                {
                    //AxisState preCamAxisX = backCam.GetComponent<CinemachineFreeLook>().m_XAxis;

                    preCam = curCam;
                    curCam = CamState.sholder;

                    if (camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value <= sholderAxisY_MaxUp ||
                        camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value <= sholderAxisY_MaxUp)
                    {
                        sholderCam.GetComponent<CinemachineFreeLook>().m_YAxis.Value
                            = sholderAxisY_MaxUp;
                    }
                    else if (camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value >= sholderAxisY_MaxDown
                        || camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value >= sholderAxisY_MaxDown)
                    {
                        sholderCam.GetComponent<CinemachineFreeLook>().m_YAxis.Value
                            = sholderAxisY_MaxDown;
                    }
                    else
                    {
                        camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value
                                        = camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value;
                    }

                    camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_XAxis.Value
                                    = camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_XAxis.Value;

                    OnOffCamera(camList[(int)curCam]);

                    //sholderCam.GetComponent<CinemachineFreeLook>().m_XAxis = preCamAxisX;                   
                }
            }
            else if (curCam == CamState.sholder) // Sholder View -> Back View
            {
                if (!playerController.characterState.aim)
                {
                    //AxisState preCamAxisX = sholderCam.GetComponent<CinemachineFreeLook>().m_XAxis;
                   
                    preCam = curCam;
                    curCam = CamState.back;

                    camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_XAxis.Value
                                    = camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_XAxis.Value;
                    camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value
                                      = camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value;

                    if (isShakedFade)
                    {
                        StopCoroutine("ShakeFadeOut");
                        InitShakeFade();
                    }

                    OnOffCamera(camList[(int)curCam]);

                    //backCam.GetComponent<CinemachineFreeLook>().m_XAxis = preCamAxisX;
                }
            }
        }

        void SetAimYAxis() // Sholder View에서 YAxis Limit 설정 
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
                    axisY.m_MaxSpeed = curSholderMaxSpeedY;
                }
                camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis = axisY;
            }
        }

        void OnOffCamera(CinemachineVirtualCameraBase curCam) // 매개변수로 받은 카메라 외에 다른 카메라는 Off (인자가 null이라면 모든 가상카메라를 끈다)  
        {
            foreach (CinemachineVirtualCameraBase cam in camList)
            {
                if (curCam && (cam == curCam)) // << : null 체크
                {
                    curCam.enabled = true;
                    continue;
                }
                cam.enabled = false;
            }
        }

        public Camera FindCamera() // 자기 Main Camera를 리턴  
        {
            if (this.gameObject.CompareTag("Nella"))
            {
                if (!mainCam)
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

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // 포톤 SerializeView  
        {
            //if (stream.IsWriting) {}
            //else {}
        }

        public void InitSceneChange() // Scene이 변경될 시 다시 카메라를 원상태 (BackView)로 복귀  
        {
            if (!pv.IsMine) return;

            float defaultAxisValue = 0.5f;

            preCam = CamState.back;
            curCam = CamState.back;

            camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_XAxis.Value = defaultAxisValue;
            camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value = defaultAxisValue;

            mainCam.fieldOfView = backViewFOV;
            camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView = backViewFOV;

            OnOffCamera(backCam);
        }


        // ====================  [Top View 함수]  ==================== //

        public void SetDefenseMode() // 디펜스 모드 설정  
        {
            if (pv.IsMine)
            {
                topCam = GameObject.FindWithTag("Cine_DefenseCam").GetComponent<CinemachineVirtualCameraBase>();
                topCam.GetComponent<CinemachineVirtualCamera>().enabled = true;

                OnOffCamera(null);

                topCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = topViewFOV;
            }
            else
            {
                mainCam.fieldOfView = topViewFOV;
            }
        }


        // ====================  [Maze View 함수]  ==================== //
        public void SetMazeMode(bool enter, bool isExit) // 미로 모드 설정  
        {
            if(enter) // 미로 입장시
            {
                OnOffCamera(null);

                if(!mazeCamObj)
                {
                    GameObject[] mazeCamObjs;
                    mazeCamObjs = GameObject.FindGameObjectsWithTag(mazeCamTag);

                    foreach (GameObject cam in mazeCamObjs)
                    {
                        if(cam.layer == 0)
                        {
                            mazeCamObj = GameObject.FindGameObjectWithTag(mazeCamTag);
                        }
                    }

                    mazeCineCam = mazeCamObj.GetComponent<CinemachineVirtualCamera>();
                    mazeCineCamCol = mazeCamObj.GetComponent<CinemachineCollider>();

                    mazeCineCam.LookAt = player.transform;
                    mazeCineCam.Follow = player.transform;

                    if (player.CompareTag("Nella"))
                    {
                        string layerName = "NellaCam";
                        string ignoreTag = "Nella";
                        mazeCamObj.gameObject.layer = LayerMask.NameToLayer(layerName);
                        mazeCineCamCol.m_IgnoreTag = ignoreTag;
                    }
                    else if (player.CompareTag("Steady"))
                    {
                        string layerName = "SteadyCam";
                        string ignoreTag = "Steady";
                        mazeCamObj.gameObject.layer = LayerMask.NameToLayer(layerName);
                        mazeCineCamCol.m_IgnoreTag = ignoreTag;
                    }
                }

                mazeCineCam.enabled = true;
                mazeCineCamCol.enabled = true;
            }
            else // 미로 퇴장시
            {
                if(!isExit)
                {
                    backCam.GetComponent<CinemachineFreeLook>().m_XAxis.Value = -(backCam.GetComponent<CinemachineFreeLook>().m_XAxis.Value);
                }
               
                mazeCineCam.enabled = false;
                mazeCineCamCol.enabled = false;

                curCam = CamState.back;
                OnOffCamera(camList[(int)curCam]);
            }        
        }

        // ====================  [Side View 함수]  ==================== //

        public void SetSideScrollMode() // 사이드뷰 모드 설정  
        {
            isSideView = true;

            if (pv.IsMine)
            {
                // 가상 카메라 생성 및 초기화
                sideCam = GameObject.FindWithTag("Cine_SideScrollCam").GetComponent<CinemachineVirtualCameraBase>();
                sideCam.GetComponent<CinemachineVirtualCamera>().enabled = true;
                sideCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = sideScrollViewFOV;

                lookAndFollow = new GameObject();
                lookAndFollow.name = "Cine_SideScrollObj";
                lookAndFollow.transform.position
                    = new Vector3(lookAndFollow.transform.position.x, sideCam.transform.position.y, lookAndFollow.transform.position.z);
                sideCam.Follow = lookAndFollow.transform;
                sideCam.LookAt = lookAndFollow.transform;


                OnOffCamera(null);

                // 타겟 설정 
                targets = new List<Transform>();
                targets.Add(GameObject.FindWithTag("Nella").GetComponent<Transform>());
                targets.Add(GameObject.FindWithTag("Steady").GetComponent<Transform>());

                minZoom = sideScrollViewFOV + 15f; // 시야각 최대 넓음
                maxZoom = sideScrollViewFOV; // 시야각 최대 좁음 = 최초 시야각 값
            }
            else
            {
                mainCam.fieldOfView = sideScrollViewFOV;
            }
        }

        void MoveInSideView() // 사이브 뷰 카메라 이동  
        {
            // 만약 한명이 죽었다면 바로 리턴

            Vector3 centerPoint = GetCenterPoint();

            lookAndFollow.transform.position = new Vector3(centerPoint.x, lookAndFollow.transform.position.y, centerPoint.z);
        }

        void ZoomInSideView() // 사이드 뷰 카메라 줌  
        {
            float newZoom = Mathf.Lerp(maxZoom, minZoom, (GetDistance() / minZoom));

            sideCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = newZoom;
        }

        float GetDistance() // 두 플레이어 간 거리를 구한다  
        {
            // if, 현재 살아있는 플레이어가 한명
            // 리턴, 그 한명의 포지션

            var bounds = new Bounds(targets[0].position, Vector3.zero); // 넬라의 센터를 기준으로하는 경계상자 생성
            bounds.Encapsulate(targets[1].position); // 위 경계상자가 스테디를 포함하도록 한다

            return bounds.size.x;
        }

        Vector3 GetCenterPoint() // 두 플레이어 사이 포지션 값을 구한다  
        {
            // if, 현재 살아있는 플레이어가 한명
            // 리턴, 그 한명의 포지션

            var bounds = new Bounds(targets[0].position, Vector3.zero); // 넬라의 센터를 기준으로하는 경계상자 생성
            bounds.Encapsulate(targets[1].position); // 위 경계상자가 스테디를 포함하도록 한다

            return bounds.center;
        }

        //float GetCenterPoint_() 
        //{
        //    float middlePos = 

        //    return bounds.center;
        //}

        public void ShakeCameraInSideView() // 플레이어 사망시, 카메라 흔들림 세팅 (외부에서 센드메시지로 호출)  
        {
            if (!canShake) return;

            if (!SideCBMCP)
            {
                SideCBMCP = new CinemachineBasicMultiChannelPerlin();

                if (AmplitudeGainSide == 0) AmplitudeGainSide = 1;
                if (AmplitudeGainSide == 0) AmplitudeGainSide = 2;

                SideCBMCP = sideCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                sideCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
                sideCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
            }

            StartCoroutine(ShakingCameraInSideView());
        }

        IEnumerator ShakingCameraInSideView() // 일정시간 동안 카메라 흔들림  
        {
            SideCBMCP.m_AmplitudeGain = AmplitudeGainSide;
            SideCBMCP.m_FrequencyGain = FrequebctGainSide;
          
            yield return new WaitForSeconds(ShakeTimeSide);

            float fadeLerpTime = 0.5f;
            StartCoroutine(ShakeCameraSideFadeOutInSideView(fadeLerpTime));
        }

        IEnumerator ShakeCameraSideFadeOutInSideView(float LerpTime) // 일정 시간 뒤, 흔들림 페이드 아웃  
        {
            float initialVlaue = SideCBMCP.m_AmplitudeGain;
            float currentTime = 0;

            while (initialVlaue > 0)
            {
                initialVlaue -= Time.deltaTime * (AmplitudeGainSide);

                currentTime += Time.deltaTime;
                if (currentTime >= LerpTime) currentTime = LerpTime;

                float curValue = Mathf.Lerp(initialVlaue, 0, currentTime / LerpTime);

                if (curValue < 0)
                    curValue = 0;

                SideCBMCP.m_AmplitudeGain = curValue;
                SideCBMCP.m_FrequencyGain = curValue;

                yield return null;
            }
        }


        // ====================  [점프 보간 함수]  ==================== //

        void FollowPlayer()  // 단순히 플레이어를 따라가기만 
        {
            Vector3 PlayerPos = player.transform.position;

            lookatBackObj.position
                            = new Vector3(PlayerPos.x,
                                        PlayerPos.y + lookatObjOriginY,
                                        PlayerPos.z);
            followObj.position
                        = new Vector3(PlayerPos.x,
                                    PlayerPos.y,
                                    PlayerPos.z);

            if (!wasEndSet) wasEndSet = true;

            if (isDebugLog) Debug.Log("호출 : 단순 플레이어 따라가는 중");
        }

        void SetCineObjPos() // Look과 Follow의 x, y값 업데이트  
        {
            if (isDebugLog) Debug.Log("호출 : 점프 대기 상태 - x, z 업데이트중");

            followObj.transform.position = player.transform.position;

            Vector3 LookPos = new Vector3(player.transform.position.x,
                                            player.transform.position.y + CineLookObj_Back.transform.position.y,
                                            player.transform.position.z);

            lookatBackObj.transform.position = LookPos;
        }
        
        public void JumpInit(bool On) // 플레이어 SMB에서 호출  
        {
            if (On) // 일반 점프 시작
            {
                // 플랫폼 보간 중, 점프를 시도했다면 
                if (isLerp)
                {
                    if (isDebugLog) Debug.Log("호출 - 플랫폼 보간중 일반점프 시도! ");
                    StopAllCoroutines();
                    isLerp = false;
                    float lerpTime = 120;
                    StartCoroutine(AirJumpLerp(lerpTime));
                    return;
                }

                isJumping = true;
                wasEndSet = false;
                wasAirJump = false;
                isAirJumpLerpEnd = false;
                isLower = false;

                // << : 점프 시작시 오브젝트들의 포지션 저장
                orgLookY = lookatBackObj.transform.position.y;
                orgFollowY = followObj.transform.position.y;
                originPlayerY = player.transform.position.y;

                if (isDebugLog) Debug.Log("호출 - 일반점프 시작");
            }
            else if (!On) // 땅에 착지 or 스페셜 액션 종료
            {
                if (isDebugLog) Debug.Log("호출 - 착지");
                isJumping = false;


                if (playerState.IsGrounded)
                {
                    isRiding = false;
                }

                if (wasEndSet)
                {
                    if (isDebugLog) Debug.Log("호출 - 이미 세팅되었으니 리턴");
                    return;
                }

                if (player.transform.position.y > originPlayerY)  // 일반 점프가 종료되고, 플랫폼에 올라탔을시, 높이 차이만큼 보간
                {
                    if (isLerp)
                    {
                        StopAllCoroutines();
                        isLerp = false;
                        if (isDebugLog) Debug.Log("호출 - 이중 코루틴 방지, 기존 코루틴 종료");
                    }

                    if (isDebugLog) Debug.Log("호출 - 일반 플랫폼 착지, 보간 시작");
                    StartCoroutine(LerpAfter(platformLerpTime));
                }
                else
                {
                    if (isDebugLog) Debug.Log("호출 - 일반 착지, 변경사항 없음");
                }


                //float curDif = Mathf.Abs(player.transform.position.y - originPlayerY);  // 점프 시작시 플레이어 높이와, 현재 플레이어의 높이 차를 구한다

                //if (curDif > avgDif) // 일반 점프가 종료되고, 플랫폼에 올라탔을시, 높이 차이만큼 보간
                //{
                //    if (isLerp)
                //    {
                //        StopAllCoroutines();
                //        Debug.Log("호출 - 이중 코루틴 방지, 기존 코루틴 종료");
                //    }

                //    Debug.Log("호출 - 일반 플랫폼 착지, 보간 시작");
                //    StartCoroutine(LerpAfter(platformLerpTime));
                //    return;
                //}
                //else // 아무런 이벤트 없이 일반점프 종료
                //{
                //    Debug.Log("호출 - 점프 후 아무런 이벤트 없이 착지");
                //    lookatBackObj.position = new Vector3(lookatBackObj.transform.position.x, player.transform.position.y + lookatObjOriginY, lookatBackObj.transform.position.z);
                //    followObj.position = new Vector3(followObj.transform.position.x, player.transform.position.y, followObj.transform.position.z);

                //}
            }
        }

        void NormalJump_FixY() // 플레이어가 일반 점프 중일 때, 외부 오브젝트의 Y값을 고정시킨다  
        {
            if (isDebugLog) Debug.Log("호출 - 일반 점프 Y값 고정중");

            lookatBackObj.position =
                        new Vector3(player.transform.position.x,
                                    orgLookY,
                                    player.transform.position.z);
            followObj.position =
                        new Vector3(player.transform.position.x,
                                    orgFollowY,
                                    player.transform.position.z);
        }

        void AirDashFollow() // 일반 점프후 에어 대쉬에서, 플레이어 위치를 따라감  
        {
            lookatBackObj.position
                            = new Vector3(player.transform.position.x,
                                        player.transform.position.y + lookatObjOriginY,
                                        player.transform.position.z);

            followObj.position
                        = new Vector3(player.transform.position.x,
                                    player.transform.position.y,
                                    player.transform.position.z);

            if (isDebugLog) Debug.Log("호출 - 일반 점프 후 에어 대쉬, 플레이어 위치 따라가는 중");
        }

        void CheckLowerPlayer() // 점프 시작 때 높이보다, 점프 종료후 높이가 낮다면 True  
        {
            if (playerState.IsAirDashing) return;
           
            if (player.transform.position.y < followObj.transform.position.y - 0.5f) // 0.5하면 떨어질 때 오류, -0.5 안 하면 점프 종료후 일반 대쉬 못따라감
            {
                wasEndSet = true;

                isLower = true;
                if (isDebugLog) Debug.Log("호출 - 일반 점프후, 플레이어 높이가 시작 높이보다 낮음 체크");
            }
        }

        void LowerPlayerFollow() // 위 함수를 통해 isLower가 true라면 플레이어를 쫓는다  
        {
            if (isDebugLog) Debug.Log("호출 - 일반 점프 후, 플레이어 위치 따라가는 중");

            lookatBackObj.position
                        = new Vector3(player.transform.position.x,
                                    player.transform.position.y + lookatObjOriginY,
                                    player.transform.position.z);

            followObj.position
                        = new Vector3(player.transform.position.x,
                                    player.transform.position.y,
                                    player.transform.position.z);
        }

        void AirJumpPlayerFollow() // 공중 점프 보간 후 플레이어 위치 따라감  
        {
            if (isDebugLog) Debug.Log("호출 - 공중 점프 보간 후, 플레이어 위치 따라가는 중");

            lookatBackObj.position
                            = new Vector3(player.transform.position.x,
                                        player.transform.position.y + lookatObjOriginY,
                                        player.transform.position.z);
            followObj.position
                        = new Vector3(player.transform.position.x,
                                    player.transform.position.y,
                                    player.transform.position.z);
        }

        public void AirJumpStart() // 공중점프 시작  
        {
            if (isDebugLog) Debug.Log("호출 - 공중 점프 시작");

            wasEndSet = true;
            wasAirJump = true;

            if (isLerp)
            {
                StopAllCoroutines();
                isLerp = false;
                if (isDebugLog) Debug.Log("호출 - 이중 코루틴 방지, 기존 코루틴 종료");
            }
            float lerpTime = 120;
            StartCoroutine(AirJumpLerp(lerpTime));
        }

        IEnumerator AirJumpLerp(float LerpTime) // 공중 점프 보간  
        {
            if (isDebugLog) Debug.Log("호출 - 공중 점프 코루틴 시작");

            float initYpos = followObj.transform.position.y; // 현재 FollowObj의 Y 값
            float lerpYpos = initYpos;  // 보간이 이루어지고 있는 y 값

            float currentTime = 0;

            wasEndSet = true;
            isLerp = true;

            while (lerpYpos < player.transform.position.y - 0.05f)
            {
                float curFollowYpos = followObj.transform.position.y;
                float curPlayerYpos = player.transform.position.y;

                currentTime += Time.fixedDeltaTime;
                if (currentTime >= LerpTime) currentTime = LerpTime;

                float t = currentTime / LerpTime;
                t = Mathf.Sin(t * Mathf.PI * 0.5f);

                lerpYpos = Mathf.Lerp(curFollowYpos, curPlayerYpos, t);
                if (lerpYpos > curPlayerYpos) lerpYpos = curPlayerYpos;

                lookatBackObj.position = new Vector3(player.transform.position.x, lerpYpos + lookatObjOriginY, player.transform.position.z);
                followObj.position = new Vector3(player.transform.position.x, lerpYpos, player.transform.position.z);
                if (isDebugLog) Debug.Log("호출 - 공중 점프 코루틴 Lerp 진행중");
                yield return new WaitForFixedUpdate();
            }

            lookatBackObj.position = new Vector3(player.transform.position.x, player.transform.position.y + lookatObjOriginY, player.transform.position.z);
            followObj.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
            isLerp = false;
            isAirJumpLerpEnd = true;
            if (isDebugLog) Debug.Log("호출 - 공중점프 코루틴 Lerp 종료");
        }

        IEnumerator LerpAfter(float LerpTime) // 일반 점프 보간  
        {
            if (isDebugLog) Debug.Log("호출 - 일반 코루틴 Lerp 시작");
            isLerp = true;

            float currentTime = 0;

            float initYpos = originPlayerY; // 점프 시작시 플레이어 Y 값
            float lerpYpos;  // 보간이 이루어지고 있는 y 값

            float delayTime = 0.1f;
            yield return new WaitForSeconds(delayTime);

            wasEndSet = true;

            if (initYpos < player.transform.position.y)
            {
                lerpYpos = initYpos;
                while (lerpYpos < player.transform.position.y)
                {

                    float curPlayerYpos = player.transform.position.y;

                    currentTime += Time.fixedDeltaTime;
                    if (currentTime >= LerpTime) currentTime = LerpTime;

                    float t = currentTime / LerpTime;
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);

                    lerpYpos = Mathf.Lerp(initYpos, curPlayerYpos, t);
                    if (lerpYpos > curPlayerYpos) lerpYpos = curPlayerYpos;

                    lookatBackObj.position = new Vector3(player.transform.position.x, lerpYpos + lookatObjOriginY, player.transform.position.z);
                    followObj.position = new Vector3(player.transform.position.x, lerpYpos, player.transform.position.z);
                    if (isDebugLog) Debug.Log("호출 - 일반 코루틴 Lerp 진행중");
                    yield return new WaitForFixedUpdate();
                }
            }
            else
            {
                lerpYpos = initYpos;

                while (lerpYpos > player.transform.position.y)
                {

                    float curPlayerYpos = player.transform.position.y;

                    currentTime += Time.fixedDeltaTime;
                    if (currentTime >= LerpTime) currentTime = LerpTime;

                    float t = currentTime / LerpTime;
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);

                    lerpYpos = Mathf.Lerp(initYpos, curPlayerYpos, t);
                    if (lerpYpos < curPlayerYpos) lerpYpos = curPlayerYpos;

                    lookatBackObj.position = new Vector3(player.transform.position.x, lookatObjOriginY + lerpYpos, player.transform.position.z);
                    followObj.position = new Vector3(player.transform.position.x, lerpYpos, player.transform.position.z);
                    if (isDebugLog) Debug.Log("호출 - 일반 코루틴 Lerp 진행중");

                    yield return new WaitForFixedUpdate();
                }
            }

            lookatBackObj.position = new Vector3(player.transform.position.x, player.transform.position.y + lookatObjOriginY, player.transform.position.z);
            followObj.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
            isLerp = false;
            if (isDebugLog) Debug.Log("호출 - 일반 코루틴 Lerp 종료");
        }

        public void RidingInit() // 라이딩(로프, 레일)진행시 각각의 SMB에서 해당 함수를 호출한다 (갈고리 제외)  
        {
            if (isLerp)
            {
                if (isDebugLog) Debug.Log("호출 - 이전 코루틴 중지");
                StopAllCoroutines();
                isLerp = false;
            }

            if (isDebugLog) Debug.Log("호출 - 라이딩 시작======================================");
            isRiding = true;           
        }

        void RidingCamera() // 라이딩 시작부터, '착지할 때'까지 플레이어를 쫓아간다  
        {
            if (isDebugLog) Debug.Log("호출 - 라이딩 카메라 진행중======================================");

            lookatBackObj.position
                            = new Vector3(player.transform.position.x,
                                        player.transform.position.y + lookatObjOriginY,
                                        player.transform.position.z);
            followObj.position
                        = new Vector3(player.transform.position.x,
                                    player.transform.position.y,
                                    player.transform.position.z);
        }


        // ====================  [스테디 전용 함수]  ==================== //

        public void SetSteadyBeam(bool isLock) // 스테디 빔 사용시, 카메라 Lock (Aim Attack State에서 호출)  
        {
            if (GameManager.Instance.isTopView || !pv.IsMine) return;

            CinemachineFreeLook cam = camList[(int)curCam].GetComponent<CinemachineFreeLook>();

            if (isLock)
            {
                cam.m_XAxis.m_InputAxisName = "";
                cam.m_YAxis.m_InputAxisName = "";

                // << : 회전하면서 빔 사용시 삥삥 도는 문제 해결
                cam.m_XAxis.m_InputAxisValue = 0;  
                cam.m_YAxis.m_InputAxisValue = 0;
            }
            else
            {
                cam.m_XAxis.m_InputAxisName = "Mouse X";
                cam.m_YAxis.m_InputAxisName = "Mouse Y";
            }
        }

        public void ShakeCamera(bool isShake) // 스테디 빔 사용시, 카메라 흔들림 (MagnifyingGlass에서 센드메시지로 호출)  
        {
            if (!canShake) return;

            if (isShake)
            {
                if (wasShaked) return;

                foreach (CinemachineBasicMultiChannelPerlin CBMCP in listSteadyCBMCP)
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

        IEnumerator ShakeFadeOut(float LerpTime) // 스테디 빔 종료시, 흔들림 페이드 아웃  
        {
            float initialVlaue = listSteadyCBMCP[0].m_AmplitudeGain;
            float currentTime = 0;

            while (initialVlaue > 0)
            {
                initialVlaue -= Time.deltaTime * (AmplitudeGain);

                currentTime += Time.deltaTime;
                if (currentTime >= LerpTime) currentTime = LerpTime;

                float curValue = Mathf.Lerp(initialVlaue, 0, currentTime / LerpTime);

                if (curValue < 0)
                    curValue = 0;

                foreach (CinemachineBasicMultiChannelPerlin CBMCP in listSteadyCBMCP)
                {
                    CBMCP.m_AmplitudeGain = curValue;
                    CBMCP.m_FrequencyGain = curValue;
                }

                yield return null;
            }
            isShakedFade = false;
        }

        void InitShakeFade() // 스테디 빔 사용중 카메라 변경시, 바로 흔들림 0으로 초기화  
        {
            float initialVlaue = 0;

            foreach (CinemachineBasicMultiChannelPerlin CBMCP in listSteadyCBMCP)
            {
                CBMCP.m_AmplitudeGain = initialVlaue;
                CBMCP.m_FrequencyGain = initialVlaue;
            }
            isShakedFade = false;
        }   
    }
}
