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
        [SerializeField] CamState blendingCam_Clone; // 블렌딩 목표 카메라
        [SerializeField] CamState blendingPrevCam_Clone; // 블렌딩 이전목표 카메라(블렌딩 중 다시 카메라 변경)
        [SerializeField] CamState preCam_Clone;


        bool isBlendStart_Clone = false; // 클론 시네머신의 블렌드가 시작 시점 true
        bool isBlending_Clone = false;  // 클론 시네머신 블렌딩 중인지
        bool isActiveCB_Clone = false;  // 클론 시네커신 브레인 enable 여부
        bool isActiveBT_Clone = false;  // 클론 시네커신 브레인 타겟 카메라 set 여부

        // Option
        [Tooltip("기본 카메라 감도")]
        public float defaultCameraSensitivity = 20;
        [Tooltip("조준 카메라 감도")]
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


            // 카메라 FOV 초기화 (해당 시네머신 카메라는 Commone Lens 설정 필요)
            if (pv.IsMine)
                mainCam.fieldOfView = camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView;
            else
                mainCam.fieldOfView = camList[(int)curCam_Clone].GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView;

            // 만약 둘이서 멀티 진행했는데 Nella Remote 블렌딩 진행중 Steady Owner의 마우스 값이 Nella Remote에게 들어온다면
            // 시야각이 카메라끼리의 블렌딩 불가, 시야각을 위한 별도 설정 필요
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

        void BlockMouseControlInBlending() // 블렌딩 도중 마우스 입력을 막는다
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

        void SetAimYAxis() // sholder View에서 YAxis Limit 설정
        {
            if (curCam == CamState.sholder)
            {
                AxisState axisY = camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis;

                if (axisY.Value < sholderAxisY_MaxUp)
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

        void SetCamera() // 플레이어 State 따라카메라 세팅 
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

        void CheckStartBlend_Clone() // Owner의 가상 카메라 전환 여부 체크 
        {
            if (blendingPrevCam_Clone != blendingCam_Clone)
            {
                isBlendStart_Clone = true;
            }
            blendingPrevCam_Clone = blendingCam_Clone;
        }

        void SetCameraBlned_Clone() // Clone의 Cinemachine Brain과 Photon을 상황에 맞게 On Off
        {
            if (isBlendStart_Clone && !cinemachineBrain.IsBlending) // 블렌딩 시작시
            {
                // 1회 호출 : Remote의 현재 블렌딩 카메라에 Owner 카메라 값을 넣어준 뒤, Remote의 시네머신을 작동 시킨다.
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

                // 1회 호출 :  Remote의 Cinemachine Brain을 켜주고, Owner의 이전 카메라 Value 값을 받아 Remote의 이전 카메라 Value에 넣어준다.
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
            else if (isBlendStart_Clone && cinemachineBrain.IsBlending) // 블렌딩 도중 취소시
            {
                ////preCam_Clone = curCam_Clone;
                ////curCam_Clone = blendingCam_Clone;
                //OnOffCamera(camList[(int)blendingCam_Clone]);
                //isBlendStart_Clone = false;

                // 1회 호출 : Remote의 현재 블렌딩 카메라에 Owner 카메라 값을 넣어준 뒤, Remote의 시네머신을 작동 시킨다.
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

                // 1회 호출 :  Remote의 Cinemachine Brain을 켜주고, Owner의 이전 카메라 Value 값을 받아 Remote의 이전 카메라 Value에 넣어준다.
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
