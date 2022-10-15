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

namespace YC.Camera_
{
    public class CameraController : MonoBehaviour, IPunObservable
    {

        // Photon 
        PhotonView pv;

        // Player
        public Camera mainCam;

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
        float PreVirtualCam_XAxis;
        float PreVirtualCam_YAxis;
        bool isBlendStart_Clone = false; // 클론 시네머신의 블렌드가 시작 시점 true
        bool isBlending_Clone = false;  // 클론 시네머신 블렌딩 중인지
        bool isActiveCB_Clone = false;  // 클론 시네커신 브레인 enable 여부
        bool isActiveBT_Clone = false;  // 클론 시네커신 브레인 타겟 카메라 set 여부

        // Option
        [Tooltip("기본 카메라 감도")]
        public float defaultCameraSensitivity = 20;
        [Tooltip("조준 카메라 감도")]
        public float sholderCameraSensitivity = 20;


        //


        void Awake()
        {
            // Photon 
            pv = GetComponent<PhotonView>();
            if (pv) pv.ObservedComponents.Add(this);

            //Camera Components
            // << :
            //mainCam             = this.gameObject.transform.Find("Main Camera").GetComponent<Camera>();
            //cinemachineBrain    = mainCam.GetComponent<CinemachineBrain>();
            // >> :

            camList = new List<CinemachineVirtualCameraBase>();

            // 리스트 인덱스 순서랑 Enum 순서 맞춰줘야 한다.
            backCam     = this.gameObject.transform.Find("Cine_backCam").GetComponent<CinemachineVirtualCameraBase>();
            wideCam     = this.gameObject.transform.Find("Cine_wideCam").GetComponent<CinemachineVirtualCameraBase>();
            sholderCam  = this.gameObject.transform.Find("Cine_sholderCam").GetComponent<CinemachineVirtualCameraBase>();
            topCam      = GameObject.Find("Cine_topCam").GetComponent<CinemachineVirtualCameraBase>();
            wallCam     = this.gameObject.transform.Find("Cine_wallCam").GetComponent<CinemachineVirtualCameraBase>();
   

            

            camList.Add(backCam);
            camList.Add(wideCam);
            camList.Add(sholderCam);
            camList.Add(topCam);
            camList.Add(wallCam);

            // Camera Variables
            curCam = new CamState();
            preCam = new CamState();


            curCam_Clone = new CamState();
            blendingCam_Clone = new CamState();

            // =====================================================

            if (!pv.IsMine)
            {
                curCam_Clone = CamState.back;
                blendingCam_Clone = CamState.back;
                OnOffCamera(camList[(int)curCam_Clone]);
            }
            else
            {
                curCam = CamState.back;
                preCam = CamState.back;
                OnOffCamera(camList[(int)curCam]);
            }


            FindCamera();



        }

        void Start()
        {
            // >> :
            //if (this.gameObject.name == "Nella(Clone)") CameraManager.Instance.cameras[0] = mainCam;
            //else if (this.gameObject.name == "Steady(Clone)") CameraManager.Instance.cameras[1] = mainCam;
            // << :

            // >> :
            //if (this.gameObject.name == "Nella(Clone)")
            //{

            //    CameraManager.Instance.cameras[0] = mainCam;
            //}
            //else if (this.gameObject.name == "Steady(Clone)")
            //{
            //    CameraManager.Instance.cameras[1] = mainCam;
            //}
            // << :

        }

        void Update()
        {
            if (!pv.IsMine)
            {
                CheckStartBlend_Clone(); // Clone의 Blend 시작 시점 체크
                SetCameraBlned_Clone();
            }

            if (pv.IsMine)
            {
                SetCamera();

                SetAimYAxis();
            }    
        }
       
        void SetAimYAxis() // sholder View에서 YAxis Limit 설정
        {
            if(curCam == CamState.sholder)
            {
                AxisState axisY = camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis;

                if (axisY.Value < 0.2f)
                {
                    axisY.Value = 0.2f;
                    axisY.m_InputAxisValue = 0;
                }

                if (axisY.Value > 0.8f)
                {
                    axisY.Value = 0.8f;
                    axisY.m_InputAxisValue = 0;
                }

                camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis = axisY;

                //Debug.Log("Debug - CameraController : " + camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value);
            }
        }

        void SetCamera() // 플레이어 스테이트에 맞게 카메라 세팅 
        {
            if (curCam == CamState.back) 
            {
                if(KeyManager.Instance.GetKey(PlayerAction.Aim)) // back View -> sholder View
                {
                    AxisState temp = backCam.GetComponent<CinemachineFreeLook>().m_XAxis;

                    preCam = curCam;
                    curCam = CamState.sholder;

                    OnOffCamera(sholderCam);

                    sholderCam.GetComponent<CinemachineFreeLook>().m_XAxis = temp;
                }
                else if(Input.GetKeyDown(KeyCode.Alpha1)) // back View -> Top View
                {
                    preCam = curCam;
                    curCam = CamState.top;
                    OnOffCamera(topCam);
                }
            }
            else if (curCam == CamState.sholder)
            {
                if(!KeyManager.Instance.GetKey(PlayerAction.Aim)) // sholder View -> back View
                {
                    AxisState temp = sholderCam.GetComponent<CinemachineFreeLook>().m_XAxis;

                    preCam = curCam;
                    curCam = CamState.back;

                    OnOffCamera(backCam);

                    backCam.GetComponent<CinemachineFreeLook>().m_XAxis = temp;
                }
            }
            else if(curCam == CamState.top)
            {
                if(Input.GetKeyDown(KeyCode.Alpha1)) // Top View -> back View
                {
                    preCam = curCam;
                    curCam = CamState.back;
                    OnOffCamera(backCam);
                }
            }
        } 

        void CheckStartBlend_Clone() // Clone의 블렌드 시작 시점에 true : 다만 blend 100% 이전에 회귀시 버그 수정 필요
        {
            if (isOriginBlending)
            {
                if (!isBlendStart_Clone && !isBlending_Clone)
                {
                    isBlendStart_Clone = true;
                    isBlending_Clone = true;

                    //Debug.Log("Debug : (0) Clone Blend Start! : 서버로부터 블렌딩 시작 여부 받음");
                }
                else isBlendStart_Clone = false;
            }
            else isBlending_Clone = false;
        }
        
        void SetCameraBlned_Clone() // Clone의 Cinemachine Brain과 Photon을 상황에 맞게 On Off
        {

            if (isActiveCB_Clone && !isActiveBT_Clone) // 1회 호출 : Cinemachine Brain On & Blending Taeget Off
            {
                if(curCam_Clone == CamState.back || curCam_Clone == CamState.wide || curCam_Clone == CamState.sholder)
                {
                    camList[(int)blendingCam_Clone].GetComponent<CinemachineFreeLook>().m_XAxis.Value
                                = originCurVirtualCam_XAxis;

                    camList[(int)blendingCam_Clone].GetComponent<CinemachineFreeLook>().m_YAxis.Value
                                = originCurVirtualCam_YAxis;

                    camList[(int)blendingCam_Clone].GetComponent<CinemachineFreeLook>().transform.position = originCurCam_Pos;
                }

                curCam_Clone = blendingCam_Clone;

                OnOffCamera(camList[(int)curCam_Clone]);

                isActiveBT_Clone = true;

                //Debug.Log("Debug : (2) Clone start Blend! to : " + camList[(int)curCam_Clone].name);
            }

            if (isBlendStart_Clone)
            {
                cinemachineBrain.enabled = true;

                if (curCam_Clone == CamState.back || curCam_Clone == CamState.wide || curCam_Clone == CamState.sholder)
                {
                    camList[(int)curCam_Clone].GetComponent<CinemachineFreeLook>().m_XAxis.Value = originPreVirtualCam_XAxis;
                    camList[(int)curCam_Clone].GetComponent<CinemachineFreeLook>().m_YAxis.Value = originPreVirtualCam_YAxis;
                }

                isActiveCB_Clone = true;

                //Debug.Log("Debug : (1) Clone start Blend! from : " + camList[(int)curCam_Clone].name);

            }

            if (isOriginBlending)
            {
                cinemachineBrain.enabled = true;
            }
            else if(cinemachineBrain.IsBlending)
            {
                cinemachineBrain.enabled = true;
            }
            else
            {
                cinemachineBrain.enabled = false;
                if (isActiveBT_Clone) isActiveBT_Clone = false;
                if (isActiveCB_Clone) isActiveCB_Clone = false;
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
      
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // Photon 
        {
            if (stream.IsWriting)
            {
                stream.SendNext(cinemachineBrain.IsBlending);
                stream.SendNext(curCam);
                stream.SendNext(camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_XAxis.Value);
                stream.SendNext(camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value);
                stream.SendNext(camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_XAxis.Value);
                stream.SendNext(camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value);
                stream.SendNext(camList[(int)curCam].GetComponent<CinemachineFreeLook>().transform.position);
            }
            else
            {
                isOriginBlending = (bool)stream.ReceiveNext();
                blendingCam_Clone = (CamState)stream.ReceiveNext();
                originPreVirtualCam_XAxis = (float)stream.ReceiveNext();
                originPreVirtualCam_YAxis = (float)stream.ReceiveNext();
                originCurVirtualCam_XAxis = (float)stream.ReceiveNext();
                originCurVirtualCam_YAxis = (float)stream.ReceiveNext();
                originCurCam_Pos = (Vector3)stream.ReceiveNext();         
            }
        }


        public Camera FindCamera()
        {
            if (this.gameObject.name == "Nella(Clone)")
            {
                mainCam = GameObject.FindGameObjectWithTag("NellaCamera").GetComponent<Camera>();

                if (mainCam) Debug.Log("Nella OK");
                else Debug.Log("NULL");

                cinemachineBrain = mainCam.GetComponent<CinemachineBrain>();
                CameraManager.Instance.cameras[0] = mainCam;

            }
            else if (this.gameObject.name == "Steady(Clone)")
            {
                mainCam = GameObject.FindGameObjectWithTag("SteadyCamera").GetComponent<Camera>();

                if (mainCam) Debug.Log("Steady OK");
                else Debug.Log("NULL");

                cinemachineBrain = mainCam.GetComponent<CinemachineBrain>();
                CameraManager.Instance.cameras[1] = mainCam;
            }

            //gameObject.transform.GetChild(16).gameObject.SetActive(true);
            gameObject.transform.GetChild(16).gameObject.GetComponent<Canvas>().worldCamera = mainCam;
            gameObject.transform.GetChild(16).gameObject.GetComponent<Canvas>().planeDistance = 1;


            return mainCam;
        }

    }
}
