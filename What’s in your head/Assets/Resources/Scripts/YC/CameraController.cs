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
        [SerializeField] CamState curCam;
        [SerializeField] CamState preCam;
        float originCurVirtualCam_XAxis;
        float originCurVirtualCam_YAxis;
        float originPreVirtualCam_XAxis;
        float originPreVirtualCam_YAxis;

        Transform lookatObj;
        Transform followObj;

        bool isOriginBlending = false;

        Vector3 originCurCam_Pos = new Vector3();

        // Clone 
        [Space]
        [Space]
        [SerializeField] CamState curCam_Clone;
        [SerializeField] CamState blendingCam_Clone; // ���� ��ǥ ī�޶�
        [SerializeField] CamState blendingPrevCam_Clone; // ���� ������ǥ ī�޶�(���� �� �ٽ� ī�޶� ����)
        [SerializeField] CamState preCam_Clone;


        bool isBlendStart_Clone = false; // Ŭ�� �ó׸ӽ��� ���尡 ���� ���� true
        bool isBlending_Clone = false;  // Ŭ�� �ó׸ӽ� ���� ������
        bool isActiveCB_Clone = false;  // Ŭ�� �ó�Ŀ�� �극�� enable ����
        bool isActiveBT_Clone = false;  // Ŭ�� �ó�Ŀ�� �극�� Ÿ�� ī�޶� set ����

        // Option
        [Tooltip("�⺻ ī�޶� ����")]
        public float defaultCameraSensitivity = 20;
        [Tooltip("���� ī�޶� ����")]
        public float sholderCameraSensitivity = 20;

        PlayerController player;

        float sholderViewMaxY;
        [SerializeField] float sholderAxisY_MaxUp;
        [SerializeField] float sholderAxisY_MaxDown;

        bool isInitCamera = false;


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
            preCam_Clone = new CamState();
            blendingCam_Clone = new CamState();


            sholderViewMaxY = sholderCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxValue;



            if (!pv.IsMine)
            {
                curCam_Clone = CamState.back;
                preCam_Clone = curCam_Clone;
                blendingCam_Clone = CamState.back;
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

        void BlockMouseControlInBlending() // ���� ���� ���콺 �Է��� ���´�
        {
            if (cinemachineBrain.IsBlending)
            {
                camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_XAxis.m_InputAxisName = "";
                camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.m_InputAxisName = "";
                camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_XAxis.m_InputAxisName = "";
                camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.m_InputAxisName = "";
            }
            else
            {
                camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_XAxis.m_InputAxisName = "Mouse X";
                camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.m_InputAxisName = "Mouse Y";
                camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_XAxis.m_InputAxisName = "Mouse X";
                camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.m_InputAxisName = "Mouse Y";
            }
        }

        void SetAimYAxis() // sholder View���� YAxis Limit ����
        {
            if (curCam == CamState.sholder)
            {
                AxisState axisY = camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis;

                if (axisY.Value < sholderAxisY_MaxUp)
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

                if (axisY.Value > sholderAxisY_MaxDown)
                {
                    if (axisY.m_InputAxisValue > 0)
                    {
                        axisY.m_MaxSpeed = sholderViewMaxY;
                    }
                    else if (axisY.m_InputAxisValue < 0)
                    {
                        axisY.m_MaxSpeed = 0;
                    }
                }
                camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis = axisY;
            }
        }

        void SetCamera() // �÷��̾� State ����ī�޶� ���� 
        {
            if (curCam == CamState.back)
            {
                //if(Input.GetMouseButtonDown(1))
                if (player.characterState.aim) // back View -> sholder View
                {
                    AxisState temp = backCam.GetComponent<CinemachineFreeLook>().m_XAxis;

                    preCam = curCam;
                    curCam = CamState.sholder;

                    OnOffCamera(sholderCam);

                    sholderCam.GetComponent<CinemachineFreeLook>().m_XAxis = temp;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha1)) // back View -> Top View
                {
                    preCam = curCam;
                    curCam = CamState.top;
                    OnOffCamera(topCam);
                }
            }
            else if (curCam == CamState.sholder)
            {
                //if (Input.GetMouseButtonDown(1))
                if (!player.characterState.aim) // sholder View -> back View
                {
                    AxisState temp = sholderCam.GetComponent<CinemachineFreeLook>().m_XAxis;

                    preCam = curCam;
                    curCam = CamState.back;

                    OnOffCamera(backCam);

                    backCam.GetComponent<CinemachineFreeLook>().m_XAxis = temp;
                }
            }
            else if (curCam == CamState.top)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1)) // Top View -> back View
                {
                    preCam = curCam;
                    curCam = CamState.back;
                    OnOffCamera(backCam);
                }
            }
        }

        void CheckStartBlend_Clone() // Owner�� ���� ī�޶� ��ȯ ���� üũ 
        {
            if (blendingPrevCam_Clone != blendingCam_Clone)
            {
                isBlendStart_Clone = true;
            }
            blendingPrevCam_Clone = blendingCam_Clone;
        }

        void SetCameraBlned_Clone() // Clone�� Cinemachine Brain�� Photon�� ��Ȳ�� �°� On Off
        {
            if (isBlendStart_Clone && !cinemachineBrain.IsBlending) // ���� ���۽�
            {
                // 1ȸ ȣ�� : Remote�� ���� ���� ī�޶� Owner ī�޶� ���� �־��� ��, Remote�� �ó׸ӽ��� �۵� ��Ų��.
                if (isActiveCB_Clone)
                {
                    if (curCam_Clone == CamState.back || curCam_Clone == CamState.wide || curCam_Clone == CamState.sholder)
                    {
                        camList[(int)blendingCam_Clone].GetComponent<CinemachineFreeLook>().m_XAxis.Value
                                    = originCurVirtualCam_XAxis;

                        camList[(int)blendingCam_Clone].GetComponent<CinemachineFreeLook>().m_YAxis.Value
                                    = originCurVirtualCam_YAxis;

                        camList[(int)blendingCam_Clone].GetComponent<CinemachineFreeLook>().transform.position = originCurCam_Pos;
                    }
                    preCam_Clone = curCam_Clone;
                    curCam_Clone = blendingCam_Clone;

                    OnOffCamera(camList[(int)curCam_Clone]);

                    isBlendStart_Clone = false;
                    isActiveCB_Clone = false;
                }

                // 1ȸ ȣ�� :  Remote�� Cinemachine Brain�� ���ְ�, Owner�� ���� ī�޶� Value ���� �޾� Remote�� ���� ī�޶� Value�� �־��ش�.
                if (isBlendStart_Clone)
                {
                    cinemachineBrain.enabled = true;

                    if (curCam_Clone == CamState.back || curCam_Clone == CamState.wide || curCam_Clone == CamState.sholder)
                    {
                        camList[(int)curCam_Clone].GetComponent<CinemachineFreeLook>().m_XAxis.Value
                                     = originPreVirtualCam_XAxis;
                        camList[(int)curCam_Clone].GetComponent<CinemachineFreeLook>().m_YAxis.Value
                                     = originPreVirtualCam_YAxis;
                    }
                    isActiveCB_Clone = true;
                }
            }
            else if (isBlendStart_Clone && cinemachineBrain.IsBlending) // ���� ���� ��ҽ�
            {
                ////preCam_Clone = curCam_Clone;
                ////curCam_Clone = blendingCam_Clone;
                //OnOffCamera(camList[(int)blendingCam_Clone]);
                //isBlendStart_Clone = false;

                // 1ȸ ȣ�� : Remote�� ���� ���� ī�޶� Owner ī�޶� ���� �־��� ��, Remote�� �ó׸ӽ��� �۵� ��Ų��.
                if (isActiveCB_Clone)
                {
                    if (curCam_Clone == CamState.back || curCam_Clone == CamState.wide || curCam_Clone == CamState.sholder)
                    {
                        camList[(int)blendingCam_Clone].GetComponent<CinemachineFreeLook>().m_XAxis.Value
                                    = originCurVirtualCam_XAxis;

                        camList[(int)blendingCam_Clone].GetComponent<CinemachineFreeLook>().m_YAxis.Value
                                    = originCurVirtualCam_YAxis;

                        camList[(int)blendingCam_Clone].GetComponent<CinemachineFreeLook>().transform.position = originCurCam_Pos;
                    }
                    preCam_Clone = curCam_Clone;
                    curCam_Clone = blendingCam_Clone;

                    OnOffCamera(camList[(int)curCam_Clone]);

                    isBlendStart_Clone = false;
                    isActiveCB_Clone = false;
                }

                // 1ȸ ȣ�� :  Remote�� Cinemachine Brain�� ���ְ�, Owner�� ���� ī�޶� Value ���� �޾� Remote�� ���� ī�޶� Value�� �־��ش�.
                if (isBlendStart_Clone)
                {
                    if (curCam_Clone == CamState.back || curCam_Clone == CamState.wide || curCam_Clone == CamState.sholder)
                    {
                        camList[(int)curCam_Clone].GetComponent<CinemachineFreeLook>().m_XAxis.Value
                                     = originPreVirtualCam_XAxis;
                        camList[(int)curCam_Clone].GetComponent<CinemachineFreeLook>().m_YAxis.Value
                                     = originPreVirtualCam_YAxis;
                    }
                    isActiveCB_Clone = true;
                }


            }
        }

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
                mainCam = GameObject.FindGameObjectWithTag("NellaCamera").GetComponent<Camera>();

                cinemachineBrain = mainCam.GetComponent<CinemachineBrain>();
                CameraManager.Instance.cameras[0] = mainCam;

            }
            else if (this.gameObject.CompareTag("Steady"))
            {
                mainCam = GameObject.FindGameObjectWithTag("SteadyCamera").GetComponent<Camera>();
                cinemachineBrain = mainCam.GetComponent<CinemachineBrain>();
                CameraManager.Instance.cameras[1] = mainCam;
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
