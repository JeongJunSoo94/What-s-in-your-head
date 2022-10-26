using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ó׸ӽ� ī�޶� ��Ʈ�ѷ�
/// ���� ���� View : BackView, AinView
/// </summary>

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
        //[SerializeField] CamState blendingCam_Clone; // ���� ��ǥ ī�޶�
        CamState blendingPrevCam_Clone; // ���� ������ǥ ī�޶�(���� �� �ٽ� ī�޶� ����)
        //[SerializeField] CamState preCam_Clone;


        //bool isBlendStart_Clone = false; // Ŭ�� �ó׸ӽ��� ���尡 ���� ���� true
        //bool isBlending_Clone = false;  // Ŭ�� �ó׸ӽ� ���� ������
        //bool isActiveCB_Clone = false;  // Ŭ�� �ó�Ŀ�� �극�� enable ����
        //bool isActiveBT_Clone = false;  // Ŭ�� �ó�Ŀ�� �극�� Ÿ�� ī�޶� set ����

        // Option
        [Header("[Back View ī�޶� ���콺 ����]")]
        [SerializeField] [Range(0, 100)] float backView_MouseSensitivity;
        [Header("[Back View ī�޶� ���콺 ����]")]
        [SerializeField] [Range(0, 100)] float sholderView_MouseSensitivity;


        PlayerController player;

        float sholderViewMaxY;
        [Header("[Sholder View Y�˵� Up ���� ��]")]
        [SerializeField] float sholderAxisY_MaxUp = 0.3f;
        [Header("[Sholder View Y�˵� Down ���� ��]")]
        [SerializeField] float sholderAxisY_MaxDown = 0.5f;


        [Header("[���׵� ��, ī�޶� ��鸲 ���� ũ��]")]
        [SerializeField] float AmplitudeGain = 3f;
        [Header("[���׵� ��, ī�޶� ��鸲 ��]")]
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

            // ����Ʈ �ε��� ������ Enum ���� ������� �Ѵ�.
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


            // ī�޶� FOV �ʱ�ȭ (�ش� �ó׸ӽ� ī�޶�� Commone Lens ���� �ʿ�)
            if (pv.IsMine)
                mainCam.fieldOfView = camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView;
            else
                mainCam.fieldOfView = camList[(int)curCam_Clone].GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView;

            // ���� ���̼� ��Ƽ �����ߴµ� Nella Remote ���� ������ Steady Owner�� ���콺 ���� Nella Remote���� ���´ٸ�
            // �þ߰��� ī�޶󳢸��� ���� �Ұ�, �þ߰��� ���� ���� ���� �ʿ�
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
            // backView_MouseSensitivity, sholderView_MouseSensitivity �� ������ 1~100�� ���� ���´� (������ ���ӸŴ����� ���� �޾ƿ�)
            // �ó׸ӽſ��� ������ ���콺 ���ǵ�� X : 100 ~ 300, Y : 1 ~ 3 �����̴�.
            // ���� ������ �ó׸ӽſ� �°� ��ȯ�Ͽ� �������ش�       

            // << : ���� ����
            int defaulyX = 100;
            int defaultY = 1;

            if (backView_MouseSensitivity == 0) backView_MouseSensitivity = 25;

            backCam.GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = defaulyX + backView_MouseSensitivity * 4;
            backCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = defaultY + backView_MouseSensitivity * 0.04f;

            if (sholderView_MouseSensitivity == 0) sholderView_MouseSensitivity = 25;

            sholderCam.GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = defaulyX + sholderView_MouseSensitivity * 4;
            sholderCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = defaultY + sholderView_MouseSensitivity * 0.04f;

            // << : ����� Y�� ����
            if (sholderAxisY_MaxUp == 0) sholderAxisY_MaxUp = 0.2f;
            if (sholderAxisY_MaxDown == 0) sholderAxisY_MaxDown = 0.5f;


            // << : ���׵� �� ��鸲 ���� ����

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


        //void BlockMouseControlInBlending() // ���� ���� ���콺 �Է��� ���´�
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

        void SetAimYAxis() // sholder View���� YAxis Limit ����.
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
                    axisY.m_MaxSpeed = 
                        sholderCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = 1 + backView_MouseSensitivity * 0.02f;
                }
                camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis = axisY;
            }
        }

        void SetCamera() // �÷��̾� State ����ī�޶� ���� 
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
                    
                    // << : ����
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

                    // << : ����
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
            Debug.Log("����!");
            
            if(pv.IsMine)
            {
                preCam = curCam;
                curCam = CamState.top;

                //Debug.Log("�α� Ȯ��");
                OnOffCamera(topCam);

                camList[(int)curCam].GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = 60;
            }
            else
            {
                mainCam.fieldOfView = 60;
            }
            

            //Debug.Log("���潺 ��� ���� - ī�޶� ��Ʈ�ѷ�!");

        }

        // << : ���׵� �� ����, ���׵� Aim Attack State���� ȣ��
        public void SetSteadyBeam(bool isLock)
        {
            // << : ž���� ����(�ؼ���)
            if (GameManager.Instance.isTopView) return;

            if(isLock)
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


        public void ShakeCamera(bool isShake) // MagnifyingGlass ���� ����޽����� ȣ��
        {
            // if(�ɼǿ��� ��鸲 �����ϸ�) return;

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


        //void CheckStartBlend_Clone() // Owner�� ���� ī�޶� ��ȯ ���� üũ 
        //{
        //    if (blendingPrevCam_Clone != blendingCam_Clone)
        //    {
        //        isBlendStart_Clone = true;
        //    }
        //    blendingPrevCam_Clone = blendingCam_Clone;
        //}

        //void SetCameraBlned_Clone() // Clone�� Cinemachine Brain�� Photon�� ��Ȳ�� �°� On Off
        //{
        //    if (isBlendStart_Clone && !cinemachineBrain.IsBlending) // ���� ���۽�
        //    {
        //        // 1ȸ ȣ�� : Remote�� ���� ���� ī�޶� Owner ī�޶� ���� �־��� ��, Remote�� �ó׸ӽ��� �۵� ��Ų��.
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

        //        // 1ȸ ȣ�� :  Remote�� Cinemachine Brain�� ���ְ�, Owner�� ���� ī�޶� Value ���� �޾� Remote�� ���� ī�޶� Value�� �־��ش�.
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
        //    else if (isBlendStart_Clone && cinemachineBrain.IsBlending) // ���� ���� ��ҽ�
        //    {
        //        ////preCam_Clone = curCam_Clone;
        //        ////curCam_Clone = blendingCam_Clone;
        //        //OnOffCamera(camList[(int)blendingCam_Clone]);
        //        //isBlendStart_Clone = false;

        //        // 1ȸ ȣ�� : Remote�� ���� ���� ī�޶� Owner ī�޶� ���� �־��� ��, Remote�� �ó׸ӽ��� �۵� ��Ų��.
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

        //        // 1ȸ ȣ�� :  Remote�� Cinemachine Brain�� ���ְ�, Owner�� ���� ī�޶� Value ���� �޾� Remote�� ���� ī�޶� Value�� �־��ش�.
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

        void OnOffCamera(CinemachineVirtualCameraBase curCam) // �Ű������� ���� ī�޶� �ܿ� �ٸ� ī�޶�� ��
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

        public Camera FindCamera() // ĳ���Ϳ� ���� �ڱ� ī�޶� ã�� ����
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
