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

        // ���� ī�޶� enum State
        enum CamState { back, sholder, top};
        CamState curCam; 
        CamState preCam; 

        // ============  ī�޶� ���� ����  ============ //
        [Header("[Back View ī�޶� ���콺 ����]")]
        [SerializeField] [Range(0, 100)] float backView_MouseSensitivity;

        [Header("[Sholder View ī�޶� ���콺 ����]")]
        [SerializeField] [Range(0, 100)] float sholderView_MouseSensitivity;

        float curSholderMaxSpeedY;

        [Space][Space]

        // ============  Aim View Y�� �˵� ����  ============ //
        [Header("[Sholder View Y�˵� Up ���� ��]")]
        [SerializeField] [Range(0, 1)] float sholderAxisY_MaxUp = 0.3f;

        [Header("[Sholder View Y�˵� Down ���� ��]")]
        [SerializeField] [Range(0, 1)] float sholderAxisY_MaxDown = 0.5f;

        float sholderViewMaxY;
        [Space][Space]

        // ============  ���׵� �� ���� ī�޶� ��鸲  ============ //
        [Header("[���׵� ��, ī�޶� ��鸲 ���� ũ��]")]
        [SerializeField] [Range(0, 5)] float AmplitudeGain = 3f;

        [Header("[���׵� ��, ī�޶� ��鸲 ��]")]
        [SerializeField] [Range(0, 5)] float FrequebctGain = 3f;

        List<CinemachineBasicMultiChannelPerlin> listCBMCP;

        [HideInInspector] public bool canShake = true; // �ɼ� ���׵� ��鸲 ����
        bool wasShaked = false;
        bool isShakedFade = true;
        [Space] [Space]

        

        // ============  FOV  ============ //
        float backViewFOV = 50;


        // ============  ���� ���� ������  ============ //
        [Header("[���� ��, �÷��� ������ ���� �ð�]")]
        [SerializeField] [Range(0, 3)] float platformLerpTime;
        [Space] [Space]
        Transform followObj; // >> : ������ Follow, LookAt ����
        public Transform lookatBackObj { get; private set; } //< : ���� �׼ǿ��� ���
        Transform lookatSholderObj;
        float lookatObjOriginY;

        //float avgDif = 0.5f; // �Ϲ� ���� �÷��� ������ �ּ� ����

        float originPlayerY; // << : �÷��̾ �����ϱ� ��, ������Ʈ���� ���̰�
        float orgLookY;
        float orgFollowY;

        bool wasEndSet = false;
        bool isJumping = false;
        bool isLerp = false;
        bool isAirJumpLerpEnd = false;
        bool isDebugLog = true;
        bool isRiding = false; // ���̵� ���ۺ���, �׶��� �����ñ��� true;


        // ============  �ν����� ������  ============ //
        [SerializeField] CinemachineVirtualCameraBase CineNellaBack;
        [SerializeField] CinemachineVirtualCameraBase CineNellaSholder;
        [SerializeField] CinemachineVirtualCameraBase CineSteadyBack;
        [SerializeField] CinemachineVirtualCameraBase CineSteadySholder;
        [SerializeField] GameObject CineLookObj_Back;
        [SerializeField] GameObject CineFollowObj_Back;


        void Awake()  
        {
            pv = GetComponent<PhotonView>();
            if (pv) pv.ObservedComponents.Add(this);

            playerController = this.gameObject.GetComponent<PlayerController>();
            playerState = this.gameObject.GetComponent<PlayerState>();
            player = this.gameObject;

            FindCamera();
            InitVirtualCamera();
            InitDefault();
        }

        void FixedUpdate()
        {
            if (!pv.IsMine) return;

            SetCamera();
            SetAimYAxis();

            if (isRiding)
            {
                RidingCamera();
                return;
            }

            if (isJumping && !isLerp)
            {
                if (!wasEndSet)
                    NormalJump_FixY();

                if (!playerState.IsAirJumping)
                    LowerPlayerFollow();

                if (playerState.IsAirJumping && isAirJumpLerpEnd)
                    AirJumpPlayerFollow();
            }
            else if (!isJumping && !isLerp)
            {
                SetCineObjPos();
            }
           
        }



        // ====================  [Awake()���� �����ϴ� �ʱ�ȭ]  ==================== //

        void InitVirtualCamera() // Virtual Camera ���� �� �ʱ�ȭ  
        {
            if (!pv.IsMine) return;

            camList = new List<CinemachineVirtualCameraBase>();

            // �� �÷��̾ �°� ���� ī�޶� ����� �ش�.
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

            // ���� ī�޶� ���� Look �� Follow ������Ʈ�� �����Ѵ�.
            //followObj = transform.Find("Cine_followObj").gameObject.transform;
            //lookatBackObj = transform.Find("Cine_lookatObj_Back").gameObject.transform;
            //lookatObjOriginY = lookatBackObj.transform.position.y;
            //lookatSholderObj = transform.Find("Cine_lookatObj_Sholder").gameObject.transform;

            followObj = Instantiate(CineFollowObj_Back, player.transform.position + CineFollowObj_Back.transform.position, player.transform.rotation).GetComponent<Transform>();
            lookatBackObj = Instantiate(CineLookObj_Back, player.transform.position + CineLookObj_Back.transform.position, player.transform.rotation).GetComponent<Transform>();
            lookatSholderObj = transform.Find("Cine_lookatObj_Sholder").gameObject.transform;
            lookatObjOriginY = lookatBackObj.transform.position.y; ;

            backCam.GetComponent<CinemachineFreeLook>().m_Follow = followObj;
            backCam.GetComponent<CinemachineFreeLook>().m_LookAt = lookatBackObj;
            sholderCam.GetComponent<CinemachineFreeLook>().m_Follow = followObj;
            sholderCam.GetComponent<CinemachineFreeLook>().m_LookAt = lookatSholderObj;

            // ���� ī�޶� ����Ʈ�� �־��ش�.
            camList.Add(backCam);
            camList.Add(sholderCam);
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

            // << : ���׵� �� ��鸲 ����
            listCBMCP = new List<CinemachineBasicMultiChannelPerlin>();

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

            // << : ī�޶� State ����
            curCam = new CamState();
            preCam = new CamState();

            // << : Sholder View Max Speed�� �޾Ƶ�
            sholderViewMaxY = sholderCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxValue;

            curCam = CamState.back;
            preCam = curCam;
            OnOffCamera(camList[(int)curCam]);
        }


        // ====================  [Option ���� �Լ�]  ==================== //

        public void Option_SetShake(bool on) // ���׵� ī�޶� ��鸲 ��� ����  
        {
            canShake = on;
        }

        public void Option_SetSensitivity(float backSensitivity, float sholderSensitivity) // ���콺 �ΰ��� ����  
        {
            int defaulyX = 100;
            int defaultY = 1;

            if (backView_MouseSensitivity == 0) backSensitivity = 25;

            backCam.GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = defaulyX + backSensitivity * 4;
            backCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = defaultY + backSensitivity * 0.04f;

            if (sholderView_MouseSensitivity == 0) sholderSensitivity = 25;

            sholderCam.GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = defaulyX + sholderSensitivity * 4;
            sholderCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = defaultY + sholderSensitivity * 0.04f;
            curSholderMaxSpeedY = sholderCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed;
        }

        
        // ====================  [�ڶ� ���׵� ���� �Լ�]  ==================== //

        void SetCamera() // �÷��̾� State�� ���� ī�޶� ����  
        {
            if (GameManager.Instance.isTopView) return;

            if (curCam == CamState.back) // Back View -> Sholder View
            {
                if (playerController.characterState.aim) 
                {
                    AxisState preCamAxisX = backCam.GetComponent<CinemachineFreeLook>().m_XAxis;

                    preCam = curCam;
                    curCam = CamState.sholder;

                    if (camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value <= sholderAxisY_MaxUp)
                    {                     
                        sholderCam.GetComponent<CinemachineFreeLook>().m_YAxis.Value
                            = sholderAxisY_MaxUp;                       
                    }
                    else if (camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value >= sholderAxisY_MaxDown)
                    {                     
                        sholderCam.GetComponent<CinemachineFreeLook>().m_YAxis.Value
                            = sholderAxisY_MaxDown;
                    }

                    camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_XAxis.Value
                                    = camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_XAxis.Value;         

                    OnOffCamera(sholderCam);

                    sholderCam.GetComponent<CinemachineFreeLook>().m_XAxis = preCamAxisX;
                }
            }
            else if (curCam == CamState.sholder) // Sholder View -> Back View
            {
                if (!playerController.characterState.aim) 
                {
                    AxisState preCamAxisX = sholderCam.GetComponent<CinemachineFreeLook>().m_XAxis;

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

                    OnOffCamera(backCam);

                    backCam.GetComponent<CinemachineFreeLook>().m_XAxis = preCamAxisX;
                }
            }
        }

        void SetAimYAxis() // Sholder View���� YAxis Limit ����  
        {
            if (curCam == CamState.sholder)
            {
                AxisState axisY = camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis;

                if (axisY.Value <= sholderAxisY_MaxUp) // Ŀ���� Max ���� �Ѿ
                {
                    // axisY.m_InputAxisValue : Ŀ�� ��(1) ~ �Ʒ�(0)
                    // axisY.Value : Ŀ�� ��(-) ~ �Ʒ� (+)

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

        public void SetDefenseMode() // ���潺 ��� �� ����  
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

        void OnOffCamera(CinemachineVirtualCameraBase curCam) // �Ű������� ���� ī�޶� �ܿ� �ٸ� ī�޶�� Off  
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

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // ���� SerializeView  
        {
            //if (stream.IsWriting) {}
            //else {}
        }


        // ====================  [���� ���� �Լ�]  ==================== //

        void SetCineObjPos() // Look�� Follow�� x, y�� ������Ʈ
        {
            if (isDebugLog) Debug.Log("ȣ�� : ���� ��� ���� - x, z ������Ʈ��");

            followObj.transform.position = player.transform.position;

            Vector3 LookPos = new Vector3(player.transform.position.x,
                                            player.transform.position.y + CineLookObj_Back.transform.position.y,
                                            player.transform.position.z);

            lookatBackObj.transform.position = LookPos;
        }

        public void JumpInit(bool On) // �÷��̾� SMB���� ȣ��  
        {
            if (On) // �Ϲ� ���� ����
            {
                // �÷��� ���� ��, ������ �õ��ߴٸ� 
                if (isLerp)
                {
                    if (isDebugLog) Debug.Log("ȣ�� - �÷��� ������ �Ϲ����� �õ�! ");
                    StopAllCoroutines();
                    isLerp = false;
                    float lerpTime = 120;
                    StartCoroutine(AirJumpLerp(lerpTime));
                    return;
                }

                isJumping = true;
                wasEndSet = false;
                isAirJumpLerpEnd = false;

                // << : ���� ���۽� ������Ʈ���� ������ ����
                orgLookY = lookatBackObj.transform.position.y;
                orgFollowY = followObj.transform.position.y;
                originPlayerY = player.transform.position.y;

                if (isDebugLog) Debug.Log("ȣ�� - �Ϲ����� ����");
            }
            else if (!On) // ���� ���� or ����� �׼� ����
            {
                if (isDebugLog) Debug.Log("ȣ�� - ����");
                isJumping = false;

                if (playerState.IsGrounded)
                {
                    isRiding = false;
                }

                if (wasEndSet)
                {
                    if (isDebugLog) Debug.Log("ȣ�� - �̹� ���õǾ����� ����");
                    return;
                }

                if (player.transform.position.y > originPlayerY)  // �Ϲ� ������ ����ǰ�, �÷����� �ö�������, ���� ���̸�ŭ ����
                {
                    if (isLerp)
                    {
                        StopAllCoroutines();
                        isLerp = false;
                        if (isDebugLog) Debug.Log("ȣ�� - ���� �ڷ�ƾ ����, ���� �ڷ�ƾ ����");
                    }

                    if (isDebugLog) Debug.Log("ȣ�� - �Ϲ� �÷��� ����, ���� ����");
                    StartCoroutine(LerpAfter(platformLerpTime));
                }
                else
                {
                    if (isDebugLog) Debug.Log("ȣ�� - �Ϲ� ����, ������� ����");
                }


                //float curDif = Mathf.Abs(player.transform.position.y - originPlayerY);  // ���� ���۽� �÷��̾� ���̿�, ���� �÷��̾��� ���� ���� ���Ѵ�

                //if (curDif > avgDif) // �Ϲ� ������ ����ǰ�, �÷����� �ö�������, ���� ���̸�ŭ ����
                //{
                //    if (isLerp)
                //    {
                //        StopAllCoroutines();
                //        Debug.Log("ȣ�� - ���� �ڷ�ƾ ����, ���� �ڷ�ƾ ����");
                //    }

                //    Debug.Log("ȣ�� - �Ϲ� �÷��� ����, ���� ����");
                //    StartCoroutine(LerpAfter(platformLerpTime));
                //    return;
                //}
                //else // �ƹ��� �̺�Ʈ ���� �Ϲ����� ����
                //{
                //    Debug.Log("ȣ�� - ���� �� �ƹ��� �̺�Ʈ ���� ����");
                //    lookatBackObj.position = new Vector3(lookatBackObj.transform.position.x, player.transform.position.y + lookatObjOriginY, lookatBackObj.transform.position.z);
                //    followObj.position = new Vector3(followObj.transform.position.x, player.transform.position.y, followObj.transform.position.z);

                //}
            }
        }

        public void NormalJump_FixY() // �÷��̾ �Ϲ� ���� ���� ��, �ܺ� ������Ʈ�� Y���� ������Ų��  
        {
            if (isDebugLog) Debug.Log("ȣ�� - �Ϲ� ���� Y�� ������");

            lookatBackObj.position =
                        new Vector3(player.transform.position.x,
                                    orgLookY,
                                    player.transform.position.z);
            followObj.position =
                        new Vector3(player.transform.position.x,
                                    orgFollowY,
                                    player.transform.position.z);
        }

        void LowerPlayerFollow() // �Ϲ� ���� �� �÷��̾, ���� ���۽� ��ġ���� �Ʒ��� �ִٸ� ī�޶� �÷��̾ �Ѵ´�  
        {
            if (player.transform.position.y < followObj.transform.position.y) // ���� �÷��̾��� ���̰�, followObj ���̺��� ���ٸ�
            {
                wasEndSet = true;

                if (isDebugLog) Debug.Log("ȣ�� - �Ϲ� ���� ��, �÷��̾� ��ġ ���󰡴� ��");
                lookatBackObj.position
                            = new Vector3(player.transform.position.x,
                                        player.transform.position.y + lookatObjOriginY,
                                        player.transform.position.z);

                followObj.position
                            = new Vector3(player.transform.position.x,
                                        player.transform.position.y,
                                        player.transform.position.z);
            }
        }

        void AirJumpPlayerFollow() // ���� ���� ���� �� �÷��̾� ��ġ ����
        {
            if (isDebugLog) Debug.Log("ȣ�� - ���� ���� ���� ��, �÷��̾� ��ġ ���󰡴� ��");

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
            if (isDebugLog) Debug.Log("ȣ�� - ���� ���� ����");

            wasEndSet = true;

            if (isLerp)
            {
                StopAllCoroutines();
                isLerp = false;
                if (isDebugLog) Debug.Log("ȣ�� - ���� �ڷ�ƾ ����, ���� �ڷ�ƾ ����");
            }
            float lerpTime = 120;
            StartCoroutine(AirJumpLerp(lerpTime));
        }

        IEnumerator AirJumpLerp(float LerpTime) // ���� ���� ����  
        {
            if (isDebugLog) Debug.Log("ȣ�� - ���� ���� �ڷ�ƾ ����");

            float initYpos = followObj.transform.position.y; // ���� ���۽� �÷��̾� Y ��
            float lerpYpos = initYpos;  // ������ �̷������ �ִ� y ��

            float currentTime = 0;

            wasEndSet = true;
            isLerp = true;

            while (lerpYpos < player.transform.position.y - 0.05f)
            {
                //if (isDebugLog) Debug.Log("ȣ�� - �������� �ڷ�ƾ Lerp ������");

                float curPlayerYpos = player.transform.position.y;
                float curFollowYpos = followObj.transform.position.y;

                currentTime += Time.deltaTime;
                if (currentTime >= LerpTime) currentTime = LerpTime;

                float t = currentTime / LerpTime;
                t = Mathf.Sin(t * Mathf.PI * 0.5f);


                lerpYpos = Mathf.Lerp(curFollowYpos, curPlayerYpos, t);
                if (lerpYpos > curPlayerYpos) lerpYpos = curPlayerYpos;


                lookatBackObj.position = new Vector3(player.transform.position.x, lerpYpos + lookatObjOriginY, player.transform.position.z);
                followObj.position = new Vector3(player.transform.position.x, lerpYpos, player.transform.position.z);

                yield return null;
            }

            lookatBackObj.position = new Vector3(player.transform.position.x, player.transform.position.y + lookatObjOriginY, player.transform.position.z);
            followObj.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
            isLerp = false;
            isAirJumpLerpEnd = true;
            if (isDebugLog) Debug.Log("ȣ�� - �������� �ڷ�ƾ Lerp ����");
        }

        IEnumerator LerpAfter(float LerpTime) // �Ϲ� ���� ����  
        {
            if (isDebugLog) Debug.Log("ȣ�� - �Ϲ� �ڷ�ƾ Lerp ����");
            isLerp = true;

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
                    //if (isDebugLog) Debug.Log("ȣ�� - �Ϲ� �ڷ�ƾ Lerp ������");

                    float curPlayerYpos = player.transform.position.y;

                    currentTime += Time.deltaTime;
                    if (currentTime >= LerpTime) currentTime = LerpTime;

                    float t = currentTime / LerpTime;
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);

                    lerpYpos = Mathf.Lerp(initYpos, curPlayerYpos, t);
                    if (lerpYpos > curPlayerYpos) lerpYpos = curPlayerYpos;

                    lookatBackObj.position = new Vector3(player.transform.position.x, lerpYpos + lookatObjOriginY, player.transform.position.z);
                    followObj.position = new Vector3(player.transform.position.x, lerpYpos, player.transform.position.z);

                    yield return null;
                }
            }
            else
            {
                lerpYpos = initYpos;

                while (lerpYpos > player.transform.position.y)
                {
                    //if (isDebugLog) Debug.Log("ȣ�� - �ڷ�ƾ Lerp ������");

                    float curPlayerYpos = player.transform.position.y;

                    currentTime += Time.deltaTime;
                    if (currentTime >= LerpTime) currentTime = LerpTime;

                    float t = currentTime / LerpTime;
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);

                    lerpYpos = Mathf.Lerp(initYpos, curPlayerYpos, t);
                    if (lerpYpos < curPlayerYpos) lerpYpos = curPlayerYpos;

                    lookatBackObj.position = new Vector3(player.transform.position.x, lookatObjOriginY + lerpYpos, player.transform.position.z);
                    followObj.position = new Vector3(player.transform.position.x, lerpYpos, player.transform.position.z);

                    yield return null;
                }
            }

            lookatBackObj.position = new Vector3(player.transform.position.x, player.transform.position.y + lookatObjOriginY, player.transform.position.z);
            followObj.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
            isLerp = false;
            if (isDebugLog) Debug.Log("ȣ�� - �Ϲ� �ڷ�ƾ Lerp ����");
        }

        public void RidingInit() // ���̵�(����, ����)����� ������ SMB���� �ش� �Լ��� ȣ���Ѵ�  (���� ����)
        {
            if (isLerp)
            {
                if (isDebugLog) Debug.Log("ȣ�� - ���� �ڷ�ƾ ����");
                StopAllCoroutines();
                isLerp = false;
            }

            if (isDebugLog) Debug.Log("ȣ�� - ���̵� ����======================================");
            isRiding = true;           
        }

        void RidingCamera() // ���̵� ���ۺ���, '������ ��'���� �÷��̾ �Ѿư���  
        {
            if (isDebugLog) Debug.Log("ȣ�� - ���̵� ī�޶� ������======================================");

            lookatBackObj.position
                            = new Vector3(player.transform.position.x,
                                        player.transform.position.y + lookatObjOriginY,
                                        player.transform.position.z);
            followObj.position
                        = new Vector3(player.transform.position.x,
                                    player.transform.position.y,
                                    player.transform.position.z);
        }


        // ====================  [���׵� ���� �Լ�]  ==================== //

        public void SetSteadyBeam(bool isLock) // ���׵� �� ����, ī�޶� Lock (Aim Attack State���� ȣ��)  
        {
            if (GameManager.Instance.isTopView || !pv.IsMine) return;

            if (isLock)
            {
                camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_XAxis.m_InputAxisName = "";
                camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.m_InputAxisName = "";

                // << : ȸ���ϸ鼭 �� ���� ��� ���� ����
                camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_XAxis.m_InputAxisValue = 0;
            }
            else
            {
                camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_XAxis.m_InputAxisName = "Mouse X";
                camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.m_InputAxisName = "Mouse Y";
            }
        }

        public void ShakeCamera(bool isShake) // ���׵� �� ����, ī�޶� ��鸲 (MagnifyingGlass���� ����޽����� ȣ��)  
        {
            if (!canShake) return;

            if (isShake)
            {
                if (wasShaked) return;

                foreach (CinemachineBasicMultiChannelPerlin CBMCP in listCBMCP)
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

        IEnumerator ShakeFadeOut(float LerpTime) // ���׵� �� �����, ��鸲 ���̵� �ƿ�  
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

        void InitShakeFade() // ���׵� �� ����� ī�޶� �����, �ٷ� ��鸲 0���� �ʱ�ȭ  
        {
            float initialVlaue = 0;

            foreach (CinemachineBasicMultiChannelPerlin CBMCP in listCBMCP)
            {
                CBMCP.m_AmplitudeGain = initialVlaue;
                CBMCP.m_FrequencyGain = initialVlaue;
            }
            isShakedFade = false;
        }   
    }
}
