using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
using YC.CameraManager_;
using UnityEngine.Rendering.Universal;
using KSU;
using System;

namespace YC.Camera_
{
    public class CameraController : MonoBehaviour, IPunObservable
    {
        // �÷��̾� �� ����
        PlayerController playerController;
        GameObject player;
        PlayerState playerState;
        PhotonView pv;

        // ���� ī�޶�
        public Camera mainCam { get; private set; }

        // ���� ī�޶��� �ó׸ӽ� �극��
        public CinemachineBrain cinemachineBrain { get; private set; }

        // ���� ī�޶� ����Ʈ
        List<CinemachineVirtualCameraBase> camList;

        // ����ī�޶�
        CinemachineVirtualCameraBase backCam;
        CinemachineVirtualCameraBase sholderCam;
        CinemachineVirtualCameraBase topCam;
        CinemachineVirtualCameraBase sideCam;

        CinemachineCollider backCamCol;
        CinemachineCollider sholderCamCol;

        // ���� ī�޶� enum State
        enum CamState { back, sholder };
        CamState curCam;
        CamState preCam;

        // ============  ����ȭ ���� ����  ============ //
        //[Header("[Clipping Planes Far]")]
        //[SerializeField] [Range(0, 1000)] 
        float farValue = 500;

        // ============  ī�޶� ���� ����  ============ //
        [Header("[Back View ī�޶� ���콺 ����]")]
        [SerializeField] [Range(0, 100)] float backView_MouseSensitivity = 25f;

        [Header("[Sholder View ī�޶� ���콺 ����]")]
        [SerializeField] [Range(0, 100)] float sholderView_MouseSensitivity = 10f;

        float curSholderMaxSpeedY;

        [Space]
        [Space]

        // ============  Aim View Y�� �˵� ����  ============ //
        [Header("[Sholder View Y�˵� Up ���� ��]")]
        [SerializeField] [Range(0, 1)] float sholderAxisY_MaxUp = 0.25f;

        [Header("[Sholder View Y�˵� Down ���� ��]")]
        [SerializeField] [Range(0, 1)] float sholderAxisY_MaxDown = 0.7f;

        float sholderViewMaxY;
        [Space]
        [Space]

        // ============  ���׵� �� ���� ī�޶� ��鸲  ============ //
        [Header("[���׵� ��, ī�޶� ��鸲 ���� ũ��]")]
        [SerializeField] [Range(0, 5)] float AmplitudeGain = 1f;

        [Header("[���׵� ��, ī�޶� ��鸲 ��]")]
        [SerializeField] [Range(0, 5)] float FrequebctGain = 1.5f;

        List<CinemachineBasicMultiChannelPerlin> listSholderCBMCP;
        List<CinemachineBasicMultiChannelPerlin> listBackCBMCP;

        [HideInInspector] public bool canShake = true; // �ɼ� ���׵� ��鸲 ����
        bool wasShaked = false;
        bool isShakedFade = false;

        Coroutine ShakeCoroutine;
        [Space]
        [Space]

        // ============  FOV  ============ //
        float backViewFOV = 50;
        //float sholderViewFOV = 50;
        float topViewFOV = 60;
        float sideScrollViewFOV = 80;

        // ============  ���̵�� ����  ============ //
        [Header("[���̵� ��, ī�޶� ��鸲 ���� ũ��]")]
        [SerializeField] [Range(0, 5)] float AmplitudeGainSide = 2f;

        [Header("[���̵� ��, ī�޶� ��鸲 ��]")]
        [SerializeField] [Range(0, 5)] float FrequebctGainSide = 1f;

        [Header("[���̵� ��, ī�޶� ��鸲 ���� �ð�]")]
        [SerializeField] [Range(0, 5)] float ShakeTimeSide = 2f;

        CinemachineBasicMultiChannelPerlin SideCBMCP;

        bool isSideView = false;
        List<Transform> targets;
        GameObject lookAndFollow;
        float minDis = 0; // Follow Obj�� �ּ� �Ÿ�
        float maxDis = -250; // Follow Obj�� �ִ� �Ÿ�
        float curPosZ = 0;
        [Space]
        [Space]

        // ============  �̷κ� ����  ============ //
        string mazeCamTag = "Cine_MazeCam";
        GameObject mazeCamObj;
        CinemachineVirtualCamera mazeCineCam;
        CinemachineCollider mazeCineCamCol;

        // ============  ���� ���� ������  ============ //
        [Header("[���� ��, �÷��� ������ ���� �ð�]")]
        [SerializeField] [Range(0, 3)] float platformLerpTime = 0.5f;
        [Space]
        [Space]
        Transform followObj; // >> : ������ Follow, LookAt ����
        public Transform lookatBackObj { get; private set; } //< : ���� �׼ǿ��� ���
        float lookatObjOriginY;

        float originPlayerY; // << : �÷��̾ �����ϱ� ��, ������Ʈ���� ���̰�
        //float orgLookY;
        //float orgFollowY;

        bool wasEndSet = false;
        bool isJumping = false;
        bool isAirJumpLerpEnd = false;
        bool wasAirJump = false;
        bool isRiding = false; // ���̵� ���ۺ���, �׶��� �����ñ��� true
        bool isLower = false; // �Ϲ� ������ �÷��̾ ���� ���� �� ���̺��� ���ٸ� true

        bool isJumpLerp = false;
        Coroutine jumpCoroutine;

        float groundFollowY;
        float groundLookY;

        // ============  OutOfControl�� ���  ============ //
        float XSpeed;
        float YSpeed;

        // ============  �ν����� ������  ============ //

        [SerializeField] CinemachineVirtualCameraBase CineBack;
        [SerializeField] CinemachineVirtualCameraBase CineSholder;
        [SerializeField] GameObject CineLookObj_Back;
        [SerializeField] GameObject CineFollowObj_Back;
        [SerializeField] NoiseSettings NoiseProfile;
        [Header("[Aim UI]")]
        public GameObject aimUI;

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
            if (!pv.IsMine) return;

            if (playerState.isInMaze)
                return;

            if (isSideView)
            {
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

            if (playerState.isCumstomJumping)
            {
                if (isJumpLerp) return;

                FollowPlayer();
                return;
            }

            if (isJumping && !isJumpLerp)
            {
                if (!wasEndSet)
                    NormalJump_FixY();

                if (!playerState.IsAirJumping && !isLower)
                    CheckLowerPlayer();

                if ((playerState.IsAirJumping || wasAirJump) && isAirJumpLerpEnd && !playerState.IsAirDashing)
                    AirJumpPlayerFollow();

                if (wasEndSet && playerState.IsAirDashing)
                    AirDashFollow();

                if (isLower)
                    LowerPlayerFollow();
            }
            else if (!isJumping && !isJumpLerp)
            {
                SetCineObjPos();
            }
        }


        // ====================  [Awake���� �����ϴ� �ʱ�ȭ]  ==================== //

        void InitUI() // Aim UI �ν��Ͻ�ȭ  
        {
            if (!pv.IsMine) return;
            aimUI = Instantiate(aimUI);
        }

        void InitVirtualCamera() // Virtual Camera ���� �� �ʱ�ȭ  
        {
            if (!pv.IsMine) return;

            camList = new List<CinemachineVirtualCameraBase>();

            backCam = Instantiate(CineBack, Vector3.zero, Quaternion.identity).GetComponent<CinemachineVirtualCameraBase>();
            sholderCam = Instantiate(CineSholder, Vector3.zero, Quaternion.identity).GetComponent<CinemachineVirtualCameraBase>();

            backCam.GetComponent<CinemachineFreeLook>().m_Lens.FarClipPlane = farValue;
            sholderCam.GetComponent<CinemachineFreeLook>().m_Lens.FarClipPlane = farValue;

            backCamCol = backCam.GetComponent<CinemachineCollider>();
            sholderCamCol = sholderCam.GetComponent<CinemachineCollider>();

            if (this.gameObject.CompareTag("Nella"))
            {
                string layerName = "NellaCam";
                string ignoreTag = "Nella";
                backCam.gameObject.layer = LayerMask.NameToLayer(layerName);
                sholderCam.gameObject.layer = LayerMask.NameToLayer(layerName);
                backCamCol.m_IgnoreTag = ignoreTag;
                sholderCamCol.m_IgnoreTag = ignoreTag;
            }
            else
            {
                string layerName = "SteadyCam";
                string ignoreTag = "Steady";
                backCam.gameObject.layer = LayerMask.NameToLayer(layerName);
                sholderCam.gameObject.layer = LayerMask.NameToLayer(layerName);
                backCamCol.m_IgnoreTag = ignoreTag;
                sholderCamCol.m_IgnoreTag = ignoreTag;
            }

            // << : Set CinemachineCollider
            backCamCol.m_SmoothingTime = 0.01f;
            backCamCol.m_Damping = 0.1f;
            backCamCol.m_DampingWhenOccluded = 0.1f;
            backCamCol.m_Strategy = CinemachineCollider.ResolutionStrategy.PullCameraForward;

            sholderCamCol.m_SmoothingTime = 0.01f;
            sholderCamCol.m_Damping = 0.1f;
            sholderCamCol.m_DampingWhenOccluded = 0.1f;
            sholderCamCol.m_Strategy = CinemachineCollider.ResolutionStrategy.PullCameraForward;


            followObj = Instantiate(CineFollowObj_Back, player.transform.position + CineFollowObj_Back.transform.position, player.transform.rotation).GetComponent<Transform>();
            lookatBackObj = Instantiate(CineLookObj_Back, player.transform.position + CineLookObj_Back.transform.position, player.transform.rotation).GetComponent<Transform>();
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

            SetCinemachineColliderDis(1);
            // ���� ī�޶� ����Ʈ�� �־��ش�.
            camList.Add(backCam);
            camList.Add(sholderCam);

            Cine_backCam.GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView = backViewFOV;
        }

        void InitCinemachineRig() //  Rig�� �߻��ϴ� �̽��� ���� ���� Rig�� ��ũ��Ʈ�� ���� �ʱ�ȭ  
        {
            if (!pv.IsMine) return;

            float zero = 0;
            //float offSetYInitValue = 2.5f;

            CinemachineFreeLook backCine = backCam.GetComponent<CinemachineFreeLook>();
            CinemachineFreeLook sholderCine = sholderCam.GetComponent<CinemachineFreeLook>();

            if (mainCam.transform.childCount != 0) // << : Rig ���� (MainCam)
            {
                for (int i = 0 ; i < mainCam.transform.childCount ; ++i)
                {
                    Destroy(mainCam.transform.GetChild(i).gameObject);
                }
            }

            //if (backCam.transform.childCount != 0) // << : Rig ���� (BackCam)
            //{
            //    for (int i = 0; i < backCam.transform.childCount; ++i)
            //    {
            //        Destroy(backCam.transform.GetChild(i).gameObject);
            //    }
            //}

            //if (sholderCam.transform.childCount != 0) // << : Rig ����(SholderCam)
            //{
            //    for (int i = 0; i < sholderCam.transform.childCount; ++i)
            //    {
            //        Destroy(sholderCam.transform.GetChild(i).gameObject);
            //    }
            //}


            for (int i = 0 ; i < 3 ; ++i)
            {
                if (backCine.GetRig(i))
                {
                    //backCine.GetRig(i).AddCinemachineComponent<CinemachineComposer>();

                    backCine.GetRig(i).GetCinemachineComponent<CinemachineComposer>().m_HorizontalDamping = zero;
                    backCine.GetRig(i).GetCinemachineComponent<CinemachineComposer>().m_VerticalDamping = zero;

                    backCine.GetRig(i).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XDamping = zero;
                    backCine.GetRig(i).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_YDamping = zero;
                    backCine.GetRig(i).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_ZDamping = zero;
                }
                if (sholderCine.GetRig(i))
                {
                    //sholderCine.GetRig(i).AddCinemachineComponent<CinemachineComposer>();

                    sholderCine.GetRig(i).GetCinemachineComponent<CinemachineComposer>().m_HorizontalDamping = zero;
                    sholderCine.GetRig(i).GetCinemachineComponent<CinemachineComposer>().m_VerticalDamping = zero;

                    sholderCine.GetRig(i).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XDamping = zero;
                    sholderCine.GetRig(i).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_YDamping = zero;
                    sholderCine.GetRig(i).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_ZDamping = zero;
                }
            }

            //backCine.GetRig(1).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y = offSetYInitValue; // Back View Middle���� ������ ���� ���
        }

        void InitDefault()  // ��Ÿ �ʱ�ȭ  
        {
            mainCam.fieldOfView = backViewFOV;
            if (!pv.IsMine) cinemachineBrain.enabled = false;

            if (!pv.IsMine) return;

            // << : ���� ����
            Option_SetSensitivity(backView_MouseSensitivity, sholderView_MouseSensitivity);

            // << : Sholder View Y�� ���� ����
            if (sholderAxisY_MaxUp == 0) sholderAxisY_MaxUp = 0.2f;
            if (sholderAxisY_MaxDown == 0) sholderAxisY_MaxDown = 0.5f;


            InitNoiseSet();


            // << : ī�޶� State ����
            curCam = new CamState();
            preCam = new CamState();

            // << : Sholder View Max Speed�� �޾Ƶ�
            sholderViewMaxY = sholderCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxValue;

            curCam = CamState.back;
            preCam = curCam;
            OnOffCamera(camList[(int)curCam]);
        }

        void InitNoiseSet()
        {
            listSholderCBMCP = new List<CinemachineBasicMultiChannelPerlin>();
            listBackCBMCP = new List<CinemachineBasicMultiChannelPerlin>();

            if (AmplitudeGain == 0) AmplitudeGain = 1;
            if (FrequebctGain == 0) FrequebctGain = 2;

            // << : �������� ������ �������� �߰�

            // Back Cam
            if (backCam.GetComponent<CinemachineFreeLook>().GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>() == null)
            {
                for (int i = 0 ; i < 3 ; ++i)
                {
                    backCam.GetComponent<CinemachineFreeLook>().GetRig(i).AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                    backCam.GetComponent<CinemachineFreeLook>().GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_NoiseProfile
                        = NoiseProfile;
                }
            }

            for (int i = 0 ; i < 3 ; ++i)
            {
                listBackCBMCP.Add(backCam.GetComponent<CinemachineFreeLook>().GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>());
            }

            for (int i = 0 ; i < 3 ; ++i)
            {
                backCam.GetComponent<CinemachineFreeLook>().GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
                backCam.GetComponent<CinemachineFreeLook>().GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
            }

            // Sholder Cam
            if (sholderCam.GetComponent<CinemachineFreeLook>().GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>() == null)
            {
                for (int i = 0 ; i < 3 ; ++i)
                {
                    sholderCam.GetComponent<CinemachineFreeLook>().GetRig(i).AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                    sholderCam.GetComponent<CinemachineFreeLook>().GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_NoiseProfile
                        = NoiseProfile;
                }
            }

            for (int i = 0 ; i < 3 ; ++i)
            {
                listSholderCBMCP.Add(sholderCam.GetComponent<CinemachineFreeLook>().GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>());
            }

            for (int i = 0 ; i < 3 ; ++i)
            {
                sholderCam.GetComponent<CinemachineFreeLook>().GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
                sholderCam.GetComponent<CinemachineFreeLook>().GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
            }
        }

        // ====================  [Option ���� �Լ� (������)]  ==================== //

        public void Option_SetShake(bool on) // ���׵� ī�޶� ��鸲 ��� ����  
        {
            if (CameraManager.Instance.isOptionInit) // << : ���� ���� ��, �ɼǿ��� �̸� ������ ���� �ִٸ�
            {
                canShake = CameraManager.Instance.canShakeSaved;
                return;
            }

            canShake = on;
        }

        public void Option_SetSensitivity(float _backSensitivity, float _sholderSensitivity) // ���콺 �ΰ��� ����  
        {
            float backSensitivity;
            float sholderSensitivity;

            if (CameraManager.Instance.isOptionInit) // << : ���� ���� ��, �ɼǿ��� �̸� ������ ���� �ִٸ�
            {
                backSensitivity = CameraManager.Instance.backSensitivitySaved;
                sholderSensitivity = CameraManager.Instance.sholderSensitivitySaved;
            }
            else
            {
                backSensitivity = _backSensitivity;
                sholderSensitivity = _sholderSensitivity;
            }

            int defaulyX = 200;
            int defaultY = 1;

            backCam.GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = defaulyX + backSensitivity * 4;
            backCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = defaultY + backSensitivity * 0.04f;

            sholderCam.GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = defaulyX + sholderSensitivity * 4;
            sholderCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = defaultY + sholderSensitivity * 0.04f;

            curSholderMaxSpeedY = sholderCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed;
        }

        public void BlockCinemachineInput(bool block)  // pause�� ī�޶� ��ǲ�� ���´� (ī�޶� �Ŵ��� ���ؼ� ȣ��)  
        {
            //if (block)
            //{
            //    cinemachineBrain.enabled = false;
            //}
            //else
            //{
            //    cinemachineBrain.enabled = true;
            //}

            if (isSideView || GameManager.Instance.isTopView) return;

            CinemachineFreeLook CF = camList[(int)curCam].GetComponent<CinemachineFreeLook>();

            if (block)
            {
                XSpeed = CF.m_XAxis.m_MaxSpeed;
                YSpeed = CF.m_YAxis.m_MaxSpeed;

                CF.m_XAxis.m_MaxSpeed = 0;
                CF.m_YAxis.m_MaxSpeed = 0;
            }
            else
            {
                CF.m_XAxis.m_MaxSpeed = XSpeed;
                CF.m_YAxis.m_MaxSpeed = YSpeed;
            }
        }


        // ====================  [�ڶ� ���׵� ���� �Լ�]  ==================== //

        void SetCamera() // �÷��̾� State�� ���� ī�޶� ����  
        {
            if (GameManager.Instance.isTopView) return;

            if (curCam == CamState.back) // Back View -> Sholder View
            {
                if (playerController.characterState.aim)
                {
                    preCam = curCam;
                    curCam = CamState.sholder;

                    CinemachineFreeLook preBackCF = camList[(int)preCam].GetComponent<CinemachineFreeLook>();
                    CinemachineFreeLook curBackCF = camList[(int)curCam].GetComponent<CinemachineFreeLook>();

                    CinemachineFreeLook sholderCF = sholderCam.GetComponent<CinemachineFreeLook>();

                    // << Y Value�� �����Ѵ�.

                    if (preBackCF.m_YAxis.Value <= sholderAxisY_MaxUp ||
                        curBackCF.m_YAxis.Value <= sholderAxisY_MaxUp)
                    {
                        sholderCF.m_YAxis.Value = sholderAxisY_MaxUp;
                    }
                    else if (preBackCF.m_YAxis.Value >= sholderAxisY_MaxDown
                        || curBackCF.m_YAxis.Value >= sholderAxisY_MaxDown)
                    {
                        sholderCF.m_YAxis.Value = sholderAxisY_MaxDown;
                    }
                    else
                    {
                        curBackCF.m_YAxis.Value = preBackCF.m_YAxis.Value;
                    }

                    curBackCF.m_XAxis.Value = preBackCF.m_XAxis.Value;

                    OnOffCamera(camList[(int)curCam]);
                }
            }
            else if (curCam == CamState.sholder) // Sholder View -> Back View
            {
                if (!playerController.characterState.aim)
                {
                    preCam = curCam;
                    curCam = CamState.back;

                    camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_XAxis.Value
                                    = camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_XAxis.Value;
                    camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value
                                      = camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value;

                    if (isShakedFade)
                    {
                        InitShakeFade();
                    }

                    OnOffCamera(camList[(int)curCam]);
                }
            }
        }

        void SetAimYAxis() // Sholder View���� YAxis Limit ���� 
        {
            if (curCam == CamState.sholder)
            {
                AxisState axisY = camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis;

                if (axisY.Value == 0)
                    axisY.Value = sholderAxisY_MaxUp;
                else if (axisY.Value == 1)
                    axisY.Value = sholderAxisY_MaxDown;

                if (axisY.Value <= sholderAxisY_MaxUp) // Ŀ���� Max ���� �Ѿ
                {
                    // axisY.m_InputAxisValue : Ŀ�� ��(1) ~ �Ʒ�(0)
                    // axisY.Value : Ŀ�� ��(-) ~ �Ʒ� (+)

                    axisY.Value = sholderAxisY_MaxUp;

                    if (axisY.m_InputAxisValue > 0)
                    {
                        axisY.m_MaxSpeed = 0;
                    }
                    else if (axisY.m_InputAxisValue < 0)
                    {
                        axisY.m_MaxSpeed = sholderViewMaxY;

                    }
                }
                else if (axisY.Value >= sholderAxisY_MaxDown) // Ŀ���� Min ������ ������.
                {
                    axisY.Value = sholderAxisY_MaxDown;

                    if (axisY.m_InputAxisValue > 0)
                    {
                        axisY.m_MaxSpeed = sholderViewMaxY;

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

        public void OnOffCamera(CinemachineVirtualCameraBase curCam) // �Ű������� ���� ī�޶� �ܿ� �ٸ� ī�޶�� Off (���ڰ� null�̶�� ��� ����ī�޶� ����)  
        {
            foreach (CinemachineVirtualCameraBase cam in camList)
            {
                if (curCam && (cam == curCam)) // << : null üũ
                {
                    curCam.enabled = true;
                    continue;
                }
                cam.enabled = false;
            }
        }

        public Camera FindCamera() // �ڱ� Main Camera�� ����  
        {
            if (this.gameObject.CompareTag("Nella"))
            {
                if (!mainCam)
                {
                    mainCam = GameObject.FindGameObjectWithTag("NellaCamera").GetComponent<Camera>();
                    mainCam.GetComponent<UniversalAdditionalCameraData>().SetRenderer(0);

                    cinemachineBrain = mainCam.GetComponent<CinemachineBrain>();
                    CameraManager.Instance.cameras[0] = mainCam;
                }
                return mainCam;
            }
            else if (this.gameObject.CompareTag("Steady"))
            {
                if (!mainCam)
                {
                    mainCam = GameObject.FindGameObjectWithTag("SteadyCamera").GetComponent<Camera>();
                    mainCam.GetComponent<UniversalAdditionalCameraData>().SetRenderer(1);
                    cinemachineBrain = mainCam.GetComponent<CinemachineBrain>();
                    CameraManager.Instance.cameras[1] = mainCam;
                }
                return mainCam;
            }
            return mainCam;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // ���� SerializeView  
        {
            //if (stream.IsWriting) {}
            //else {}
        }

        public void InitSceneChange() // Scene�� ����� �� �ٽ� ī�޶� ������ (BackView)�� ����  
        {
            if (!pv.IsMine) return;

            //if (GameManager.Instance.isTopView || GameManager.Instance.isSideView || playerState.isInMaze) return;

            if (playerState.isInMaze)
            {
                mazeCineCam.enabled = false;
                mazeCineCamCol.enabled = false;
            }

            float defaultAxisValue = 0.5f;

            preCam = CamState.back;
            curCam = CamState.back;

            camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_XAxis.Value = defaultAxisValue;
            camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value = defaultAxisValue;

            mainCam.fieldOfView = backViewFOV;
            camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView = backViewFOV;

            OnOffCamera(backCam);
        }

        public void SetCinemachineColliderDis(float dis)
        {
            if(!pv.IsMine) return;

            backCam.GetComponent<CinemachineCollider>().m_DistanceLimit = dis;
            sholderCam.GetComponent<CinemachineCollider>().m_DistanceLimit = dis;

        }

        // ====================  [Top View �Լ�]  ==================== //

        public void SetDefenseMode() // ���潺 ��� ����  
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


        // ====================  [Maze View �Լ�]  ==================== //
        public void SetMazeMode(bool enter, bool isExit) // �̷� ��� ����  
        {
            if (!pv.IsMine) return;

            if (enter) // �̷� �����
            {
                OnOffCamera(null);

                if (!mazeCamObj)
                {
                    GameObject[] mazeCamObjs;
                    mazeCamObjs = GameObject.FindGameObjectsWithTag(mazeCamTag);

                    foreach (GameObject cam in mazeCamObjs)
                    {
                        if (cam.layer == 0)
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
            else // �̷� �����
            {
                if (!isExit)
                {
                    backCam.GetComponent<CinemachineFreeLook>().m_XAxis.Value = -(backCam.GetComponent<CinemachineFreeLook>().m_XAxis.Value);
                }

                mazeCineCam.enabled = false;
                mazeCineCamCol.enabled = false;

                curCam = CamState.back;
                OnOffCamera(camList[(int)curCam]);
            }
        }

        // ====================  [Side View �Լ�]  ==================== //  

        public void SetSideScrollMode() // ���̵�� ��� ����  
        {
            isSideView = true;

            if (pv.IsMine)
            {
                // ���� ī�޶� ���� �� �ʱ�ȭ
                sideCam = GameObject.FindWithTag("Cine_SideScrollCam").GetComponent<CinemachineVirtualCameraBase>();
                sideCam.GetComponent<CinemachineVirtualCamera>().enabled = true;
                sideCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = sideScrollViewFOV;

                lookAndFollow = new GameObject();
                lookAndFollow.name = "Cine_SideScrollObj";
                lookAndFollow.transform.position
                    = new Vector3(lookAndFollow.transform.position.x, sideCam.transform.position.y, 0);
                sideCam.Follow = lookAndFollow.transform;
                sideCam.LookAt = lookAndFollow.transform;

                OnOffCamera(null);

                // Ÿ�� ���� 
                targets = new List<Transform>();
                targets.Add(GameObject.FindWithTag("Nella").GetComponent<Transform>());
                targets.Add(GameObject.FindWithTag("Steady").GetComponent<Transform>());
            }
            else
            {
                mainCam.fieldOfView = sideScrollViewFOV;
            }
        }

        public void LerpPlatformHeight_Cor(float LerpTime, float height) // �÷��� ���� ���� ����  
        {
            if (pv.IsMine)
                jumpCoroutine = StartCoroutine(LerpPlatformHeight(LerpTime, height));
        }

        IEnumerator LerpPlatformHeight(float LerpTime, float height)
        {
            float initYpos = lookAndFollow.transform.position.y;
            float lerpYpos = initYpos;
            float targetYpos = initYpos + height;

            float currentTime = 0;

            while (lerpYpos < targetYpos)
            {
                lerpYpos = lookAndFollow.transform.position.y;

                currentTime += Time.fixedDeltaTime;
                if (currentTime >= LerpTime) currentTime = LerpTime;

                float t = currentTime / LerpTime;
                t = Mathf.Sin(t * Mathf.PI * 0.5f);

                lerpYpos = Mathf.Lerp(lerpYpos, targetYpos, t);
                if (lerpYpos > targetYpos) lerpYpos = targetYpos;

                lookAndFollow.transform.position = new Vector3(lookAndFollow.transform.position.x, lerpYpos, lookAndFollow.transform.position.z);
                //lookAndFollow.transform.position = new Vector3(lookAndFollow.transform.position.x, lerpYpos, curPosZ);
                yield return new WaitForFixedUpdate();
            }
        }

        void MoveInSideView() // ���̺� �� ī�޶� �̵�  
        {
            if (!targets[0] || !targets[1]) return;

            Vector3 centerPoint = GetCenterPoint();

            //lookAndFollow.transform.position = new Vector3(centerPoint.x, lookAndFollow.transform.position.y, centerPoint.z);
            lookAndFollow.transform.position = new Vector3(centerPoint.x, lookAndFollow.transform.position.y, curPosZ);
        }

        void ZoomInSideView() // ���̵� �� ī�޶� ��  
        {
            float newZoom = Mathf.Lerp(minDis, maxDis, (GetDistance() / 2000));
            curPosZ = newZoom;
        }

        float GetDistance() // �� �÷��̾� �� �Ÿ��� ���Ѵ�  
        {
            if (!targets[0] || !targets[1]) return 0;

            var bounds = new Bounds(targets[0].position, Vector3.zero); // �ڶ��� ���͸� ���������ϴ� ������ ����
            bounds.Encapsulate(targets[1].position); // �� �����ڰ� ���׵� �����ϵ��� �Ѵ�

            return bounds.size.x;
        }

        Vector3 GetCenterPoint() // �� �÷��̾� ���� ������ ���� ���Ѵ�  
        {
            // if, ���� ����ִ� �÷��̾ �Ѹ�
            // ����, �� �Ѹ��� ������         

            var bounds = new Bounds(targets[0].position, Vector3.zero); // �ڶ��� ���͸� ���������ϴ� ������ ����
            bounds.Encapsulate(targets[1].position); // �� �����ڰ� ���׵� �����ϵ��� �Ѵ�

            return bounds.center;
        }

        public void ShakeCameraInSideView() // �÷��̾� �����, ī�޶� ��鸲 ���� (�ܺο��� ����޽����� ȣ��)  
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

        IEnumerator ShakingCameraInSideView() // �����ð� ���� ī�޶� ��鸲  
        {
            SideCBMCP.m_AmplitudeGain = AmplitudeGainSide;
            SideCBMCP.m_FrequencyGain = FrequebctGainSide;

            yield return new WaitForSeconds(ShakeTimeSide);

            float fadeLerpTime = 0.5f;
            StartCoroutine(ShakeCameraSideFadeOutInSideView(fadeLerpTime));
        }

        IEnumerator ShakeCameraSideFadeOutInSideView(float LerpTime) // ���� �ð� ��, ��鸲 ���̵� �ƿ�  
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


        // ====================  [���� ���� �Լ�]  ==================== //

        void FollowPlayer()  // �ܼ��� �÷��̾ ���󰡱⸸ 
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
        }

        void SetCineObjPos() // Look�� Follow�� x, y�� ������Ʈ  (�׶��� ����)  
        {
            followObj.transform.position = player.transform.position;

            Vector3 LookPos = new Vector3(player.transform.position.x,
                                            player.transform.position.y + CineLookObj_Back.transform.position.y,
                                            player.transform.position.z);

            lookatBackObj.transform.position = LookPos;

            // << : Test
            groundFollowY = player.transform.position.y;
            groundLookY = player.transform.position.y + CineLookObj_Back.transform.position.y;

        }

        public void JumpInit(bool On) // �÷��̾� SMB���� ȣ��  
        {
            if (On) // �Ϲ� ���� ����
            {
                // �÷��� ���� ��, ������ �õ��ߴٸ� 
                if (isJumpLerp)
                {
                    StopCoroutine(jumpCoroutine);
                    isJumpLerp = false;
                    float lerpTime = 120;
                    jumpCoroutine = StartCoroutine(AirJumpLerp(lerpTime));
                    return;
                }

                isJumping = true;
                wasEndSet = false;
                wasAirJump = false;
                isAirJumpLerpEnd = false;
                isLower = false;

                // << : ���� ���۽� ������Ʈ���� ������ ����
                //orgLookY = lookatBackObj.transform.position.y;
                //orgFollowY = followObj.transform.position.y;
                originPlayerY = player.transform.position.y;
            }
            else if (!On) // ���� ���� or ����� �׼� ����
            {
                isJumping = false;

                if (playerState.IsGrounded)
                {
                    isRiding = false;
                }

                if (wasEndSet)
                {
                    return;
                }

                if (player.transform.position.y > originPlayerY)  // �Ϲ� ������ ����ǰ�, �÷����� �ö�������, ���� ���̸�ŭ ����
                {
                    if (isJumpLerp)
                    {
                        StopCoroutine(jumpCoroutine);
                        isJumpLerp = false;
                    }
                    jumpCoroutine = StartCoroutine(LerpAfter(platformLerpTime));
                }
            }
        }

        void NormalJump_FixY() // �÷��̾ �Ϲ� ���� ���� ��, �ܺ� ������Ʈ�� Y���� ������Ų��  
        {
            //lookatBackObj.position =
            //            new Vector3(player.transform.position.x,
            //                        orgLookY,
            //                        player.transform.position.z);
            //followObj.position =
            //            new Vector3(player.transform.position.x,
            //                        orgFollowY,
            //                        player.transform.position.z);

            // << : Test

            lookatBackObj.position =
                        new Vector3(player.transform.position.x,
                                    groundLookY,
                                    player.transform.position.z);
            followObj.position =
                        new Vector3(player.transform.position.x,
                                    groundFollowY,
                                    player.transform.position.z);
        }

        void AirDashFollow() // �Ϲ� ������ ���� �뽬����, �÷��̾� ��ġ�� ����  
        {

            lookatBackObj.position
                            = new Vector3(player.transform.position.x,
                                        player.transform.position.y + lookatObjOriginY,
                                        player.transform.position.z);

            followObj.position
                        = new Vector3(player.transform.position.x,
                                    player.transform.position.y,
                                    player.transform.position.z);
        }

        void CheckLowerPlayer() // ���� ���� �� ���̺���, ���� ������ ���̰� ���ٸ� True  
        {
            if (playerState.IsAirDashing) return;

            if (player.transform.position.y < followObj.transform.position.y - 0.5f) // 0.5�ϸ� ������ �� ����, -0.5 �� �ϸ� ���� ������ �Ϲ� �뽬 ������
            {
                wasEndSet = true;

                isLower = true;
            }
        }

        void LowerPlayerFollow() // �� �Լ��� ���� isLower�� true��� �÷��̾ �Ѵ´�  
        {
            lookatBackObj.position
                        = new Vector3(player.transform.position.x,
                                    player.transform.position.y + lookatObjOriginY,
                                    player.transform.position.z);

            followObj.position
                        = new Vector3(player.transform.position.x,
                                    player.transform.position.y,
                                    player.transform.position.z);
        }

        void AirJumpPlayerFollow() // ���� ���� ���� �� �÷��̾� ��ġ ����  
        {
            lookatBackObj.position
                            = new Vector3(player.transform.position.x,
                                        player.transform.position.y + lookatObjOriginY,
                                        player.transform.position.z);
            followObj.position
                        = new Vector3(player.transform.position.x,
                                    player.transform.position.y,
                                    player.transform.position.z);
        }

        public void AirJumpStart() // �������� ����  
        {
            wasEndSet = true;
            wasAirJump = true;

            if (isJumpLerp)
            {
                StopCoroutine(jumpCoroutine);
                isJumpLerp = false;
            }
            float lerpTime = 120;
            jumpCoroutine = StartCoroutine(AirJumpLerp(lerpTime));
        }

        IEnumerator AirJumpLerp(float LerpTime) // ���� ���� ����  
        {
            float initYpos = followObj.transform.position.y; // ���� FollowObj�� Y ��
            float lerpYpos = initYpos;  // ������ �̷������ �ִ� y ��

            float currentTime = 0;

            wasEndSet = true;
            isJumpLerp = true;

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
                yield return new WaitForFixedUpdate();
            }

            lookatBackObj.position = new Vector3(player.transform.position.x, player.transform.position.y + lookatObjOriginY, player.transform.position.z);
            followObj.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
            isJumpLerp = false;
            isAirJumpLerpEnd = true;
        }

        IEnumerator LerpAfter(float LerpTime) // �Ϲ� ���� ����  
        {
            isJumpLerp = true;

            float currentTime = 0;

            float initYpos = originPlayerY; // ���� ���۽� �÷��̾� Y ��
            float lerpYpos;  // ������ �̷������ �ִ� y ��

            float delayTime = 0.1f;
            yield return new WaitForSeconds(delayTime);

            wasEndSet = true;

            if (initYpos < player.transform.position.y)
            {
                lerpYpos = initYpos;
                while (lerpYpos < player.transform.position.y)
                {
                    //Debug.Log(lookatBackObj.position.y);

                    float curPlayerYpos = player.transform.position.y;

                    currentTime += Time.fixedDeltaTime;
                    if (currentTime >= LerpTime) currentTime = LerpTime;

                    float t = currentTime / LerpTime;
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);

                    lerpYpos = Mathf.Lerp(initYpos, curPlayerYpos, t);
                    if (lerpYpos > curPlayerYpos) lerpYpos = curPlayerYpos;

                    lookatBackObj.position = new Vector3(player.transform.position.x, lerpYpos + lookatObjOriginY, player.transform.position.z);
                    followObj.position = new Vector3(player.transform.position.x, lerpYpos, player.transform.position.z);
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

                    yield return new WaitForFixedUpdate();
                }
            }

            lookatBackObj.position = new Vector3(player.transform.position.x, player.transform.position.y + lookatObjOriginY, player.transform.position.z);
            followObj.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
            isJumpLerp = false;
        }

        public void RidingInit() // ���̵�(����, ����)����� ������ SMB���� �ش� �Լ��� ȣ���Ѵ� (���� ����)  
        {
            if (playerState.IsJumping && !playerState.IsAirJumping)
            {
                float lerpTime = 120;
                jumpCoroutine = StartCoroutine(AirJumpLerp(lerpTime));
            }
            isRiding = true;
        }

        void RidingCamera() // ���̵� ���ۺ���, '������ ��'���� �÷��̾ �Ѿư���  
        {
            if (isJumpLerp) return;

            lookatBackObj.position
                        = new Vector3(player.transform.position.x,
                                    player.transform.position.y + lookatObjOriginY,
                                    player.transform.position.z);
            followObj.position
                        = new Vector3(player.transform.position.x,
                                    player.transform.position.y,
                                    player.transform.position.z);
        }

        // ====================  [Shake �Լ�]  ==================== //

        public void SetSteadyBeam(bool isLock) // ���׵� �� ����, ī�޶� Lock (Aim Attack State���� ȣ��)  
        {
            if (GameManager.Instance.isTopView || !pv.IsMine) return;

            CinemachineFreeLook cam = camList[(int)curCam].GetComponent<CinemachineFreeLook>();

            if (isLock)
            {
                cam.m_XAxis.m_InputAxisName = "";
                cam.m_YAxis.m_InputAxisName = "";

                // << : ȸ���ϸ鼭 �� ���� ��� ���� ���� �ذ�
                cam.m_XAxis.m_InputAxisValue = 0;
                cam.m_YAxis.m_InputAxisValue = 0;
            }
            else
            {
                cam.m_XAxis.m_InputAxisName = "Mouse X";
                cam.m_YAxis.m_InputAxisName = "Mouse Y";
            }
        }

        public void ShakeCamera(bool isShake) // ī�޶� ��鸲 ����
        {
            if (!canShake) return;

            if (isShake)
            {
                //if (wasShaked)
                //{
                //    return;
                //}

                if (isShakedFade)
                {
                    InitShakeFade();
                }


                if (curCam == CamState.back)
                {
                    foreach (CinemachineBasicMultiChannelPerlin CBMCP in listBackCBMCP)
                    {
                        CBMCP.m_AmplitudeGain = AmplitudeGain;
                        CBMCP.m_FrequencyGain = FrequebctGain;
                    }
                }
                else if (curCam == CamState.sholder)
                {

                    foreach (CinemachineBasicMultiChannelPerlin CBMCP in listSholderCBMCP)
                    {
                        CBMCP.m_AmplitudeGain = AmplitudeGain;
                        CBMCP.m_FrequencyGain = FrequebctGain;
                    }
                }
                //wasShaked = true;

            }
            else
            {
                //if (!wasShaked) return;

                //wasShaked = false;

                isShakedFade = true;

                float fadeLerpTime = 0.3f;

                ShakeCoroutine = StartCoroutine(ShakeFadeOut(fadeLerpTime));
            }
        }

        IEnumerator ShakeFadeOut(float LerpTime) // ��鸲 �����, ��鸲 ���̵� �ƿ�  
        {
            float initialVlaue = 0;

            if (curCam == CamState.back)
                initialVlaue = listBackCBMCP[0].m_AmplitudeGain;
            else if (curCam == CamState.sholder)
                initialVlaue = listSholderCBMCP[0].m_AmplitudeGain;

            float currentTime = 0;

            while (initialVlaue > 0)
            {
                initialVlaue -= Time.deltaTime * (AmplitudeGain);

                currentTime += Time.deltaTime;
                if (currentTime >= LerpTime) currentTime = LerpTime;

                float curValue = Mathf.Lerp(initialVlaue, 0, currentTime / LerpTime);

                if (curValue < 0)
                    curValue = 0;

                if (curCam == CamState.back)
                {
                    foreach (CinemachineBasicMultiChannelPerlin CBMCP in listBackCBMCP)
                    {
                        CBMCP.m_AmplitudeGain = curValue;
                        CBMCP.m_FrequencyGain = curValue;
                    }
                }
                else if (curCam == CamState.sholder)
                {
                    foreach (CinemachineBasicMultiChannelPerlin CBMCP in listSholderCBMCP)
                    {
                        CBMCP.m_AmplitudeGain = curValue;
                        CBMCP.m_FrequencyGain = curValue;
                    }
                }

                yield return null;
            }

            isShakedFade = false;
        }

        public void InitShakeFade() // �ٷ� ��鸲 0���� �ʱ�ȭ  
        {
            if (!isShakedFade) return;

            StopCoroutine(ShakeCoroutine);

            float initialVlaue = 0;

            foreach (CinemachineBasicMultiChannelPerlin CBMCP in listBackCBMCP)
            {
                CBMCP.m_AmplitudeGain = initialVlaue;
                CBMCP.m_FrequencyGain = initialVlaue;
            }

            foreach (CinemachineBasicMultiChannelPerlin CBMCP in listSholderCBMCP)
            {
                CBMCP.m_AmplitudeGain = initialVlaue;
                CBMCP.m_FrequencyGain = initialVlaue;
            }
            isShakedFade = false;
        }
    }
}