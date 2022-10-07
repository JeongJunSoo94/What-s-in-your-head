using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 시네머신 카메라 컨트롤러
/// 현재 구현 View : BackView, AinView
/// </summary>

using Photon.Pun;
using Cinemachine;

using JCW.Options.InputBindings;

namespace YC.Camera_
{
    public class CameraController : MonoBehaviour, IPunObservable
    {

         
        // Photon 

        PhotonView pv;

        // Player
        Camera mainCam;

        // Cinemachine
        CinemachineBrain cinemachineBrain;
        List<CinemachineVirtualCameraBase> camList;
        CinemachineVirtualCameraBase backCam;       
        CinemachineVirtualCameraBase wideCam;         
        CinemachineVirtualCameraBase sholderCam;            
        CinemachineVirtualCameraBase topCam;
        CinemachineVirtualCameraBase wallCam;

        enum CamState { back, wide, sholder, top, wall };
        CamState curCam;
        CamState preCam;
        float originCurVirtualCam_XAxis;
        float originCurVirtualCam_YAxis;
        float originPreVirtualCam_XAxis;
        float originPreVirtualCam_YAxis;

        Transform lookatObj;
        Transform followObj;

        bool isOriginBlending = false;

        Vector3 originCurCam_Pos = new Vector3();

        // Clone 
        CamState curCam_Clone;
        CamState blendingCam_Clone; // 블렌딩 목표 카메라
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


        void Awake()
        {
            // Photon 
            #region
            pv = GetComponent<PhotonView>();
            if (pv) pv.ObservedComponents.Add(this);
            #endregion

            //Camera Components
            #region 
            cinemachineBrain    = this.gameObject.transform.Find("Main Camera").GetComponent<CinemachineBrain>();
            mainCam             = this.gameObject.transform.Find("Main Camera").GetComponent<Camera>();

            camList     = new List<CinemachineVirtualCameraBase>();
            backCam     = this.gameObject.transform.Find("Cine_backCam").GetComponent<CinemachineVirtualCameraBase>();
            sholderCam  = this.gameObject.transform.Find("Cine_sholderCam").GetComponent<CinemachineVirtualCameraBase>();
            topCam      = GameObject.Find("Cine_topCam").GetComponent<CinemachineVirtualCameraBase>();
            camList.Add(backCam);
            camList.Add(sholderCam);
            camList.Add(topCam);
            #endregion

            // Camera Variables
            #region 
            curCam = new CamState();
            preCam = new CamState();


            curCam_Clone = new CamState();
            blendingCam_Clone = new CamState();

            #endregion
        }

        void Start()
        {      
            if(pv.IsMine)
            {           
                curCam_Clone = CamState.back;
                blendingCam_Clone = CamState.back;
                OnOffCamera(camList[(int)curCam_Clone]);
            }
            else
            {
                curCam = CamState.back;
                OnOffCamera(camList[(int)curCam]);
            }
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
            }
        }

        void SetCamera() // 플레이어 스테이트에 맞게 카메라 세팅 
        {
            if (curCam == CamState.back) 
            {
                if(ITT_KeyManager.Instance.GetKeyDown(PlayerAction.Aim)) // back View -> sholder View
                {
                    AxisState temp = backCam.GetComponent<CinemachineFreeLook>().m_XAxis;

                    curCam = CamState.sholder;

                    OnOffCamera(sholderCam);

                    sholderCam.GetComponent<CinemachineFreeLook>().m_XAxis = temp;

                    Debug.Log("Debug - Sholder View");
                }
                else if(Input.GetKeyDown(KeyCode.Alpha1)) // back View -> Top View
                {
                    curCam = CamState.top;
                    OnOffCamera(topCam);
                    Debug.Log("Debug - Top View");
                }
            }
            else if (curCam == CamState.sholder)
            {
                if(ITT_KeyManager.Instance.GetKeyDown(PlayerAction.Aim)) // sholder View -> back View
                {
                    AxisState temp = sholderCam.GetComponent<CinemachineFreeLook>().m_XAxis;

                    curCam = CamState.back;

                    OnOffCamera(backCam);

                    Debug.Log("Debug - Back View");

                    backCam.GetComponent<CinemachineFreeLook>().m_XAxis = temp;
                }
            }
            else if(curCam == CamState.top)
            {
                if(Input.GetKeyDown(KeyCode.Alpha1)) // Top View -> back View
                {
                    curCam = CamState.back;
                    OnOffCamera(backCam);

                    Debug.Log("Debug - Back View");
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
                }
                else isBlendStart_Clone = false;
            }
            else isBlending_Clone = false;
        }
        
        void SetCameraBlned_Clone() // Clone의 Cinemachine Brain과 Photon을 상황에 맞게 On Off
        {
            if(isActiveCB_Clone && !isActiveBT_Clone) // 1회 호출 : Cinemachine Brain On & Blending Taeget Off
            {
                if(curCam_Clone == CamState.back || curCam_Clone == CamState.wide || curCam_Clone == CamState.sholder)
                {
                    camList[(int)blendingCam_Clone].GetComponent<CinemachineFreeLook>().m_XAxis.Value
                                = originCurVirtualCam_XAxis;

                    camList[(int)blendingCam_Clone].GetComponent<CinemachineFreeLook>().m_YAxis.Value
                                = originPreVirtualCam_YAxis;

                    camList[(int)blendingCam_Clone].GetComponent<CinemachineFreeLook>().transform.position = originCurCam_Pos;
                }

                curCam_Clone = blendingCam_Clone;

                OnOffCamera(camList[(int)curCam_Clone]);

                isActiveBT_Clone = true;
            } 
            
            if(isBlendStart_Clone)
            {
                cinemachineBrain.enabled = true;

                if (curCam_Clone == CamState.back || curCam_Clone == CamState.wide || curCam_Clone == CamState.sholder)
                {
                    camList[(int)curCam_Clone].GetComponent<CinemachineFreeLook>().m_XAxis.Value = originPreVirtualCam_XAxis;
                    camList[(int)curCam_Clone].GetComponent<CinemachineFreeLook>().m_YAxis.Value = originPreVirtualCam_YAxis;
                }

                isActiveCB_Clone = true;
            }

            if(isOriginBlending)
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
                if (isActiveBT_Clone) isActiveBT_Clone = true;
                if (isActiveCB_Clone) isActiveCB_Clone = true;
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
                originCurVirtualCam_XAxis = (float)stream.ReceiveNext();
                originCurVirtualCam_YAxis = (float)stream.ReceiveNext();
                originPreVirtualCam_XAxis = (float)stream.ReceiveNext();
                originPreVirtualCam_YAxis = (float)stream.ReceiveNext();
                originCurCam_Pos = (Vector3)stream.ReceiveNext();

                ///remoteFOV = (float)stream.ReceiveNext();
                ///Mathf.Lerp(mainCam.fieldOfView, (float)stream.ReceiveNext(), 0.5f);
                ///mainCam.fieldOfView = (float)stream.ReceiveNext();
                /// 보간
                ///float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                ///mainCam.fieldOfView = (float)stream.ReceiveNext();
                ///mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, (float)stream.ReceiveNext(), 1f * lag);
            }
        }
    }
}
