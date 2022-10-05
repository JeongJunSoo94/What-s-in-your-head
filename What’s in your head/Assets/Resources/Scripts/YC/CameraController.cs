using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ó׸ӽ� ī�޶� ��Ʈ�ѷ�
/// ���� ���� View : BackView, AinView
/// </summary>

using Photon.Pun;
using Cinemachine;

using JCW.Options.InputBindings;

namespace YC.Camera_
{
    public class CameraController : MonoBehaviour
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
        float curVirtualCam_XAxis;
        float curVirtualCam_YAxis;
        

        Transform lookatObj;
        Transform followObj;

        bool isOriginBlending = false;

        // Clone 
        CamState curCam_Clone;
        CamState BlendingToCam_Clone; // ���� ��ǥ ī�޶�
        float PreVirtualCam_XAxis;
        float PreVirtualCam_YAxis;
        bool isBlendStart_Clone = false;
        bool isBlending_Clone = false;


        void Awake()
        {
            // Photon 
            pv = GetComponent<PhotonView>();
            if (pv) pv.ObservedComponents.Add(this);

            camList = new List<CinemachineVirtualCameraBase>();

            // Camera
            cinemachineBrain = this.gameObject.transform.Find("Main Camera").GetComponent<CinemachineBrain>();

            mainCam     = this.gameObject.transform.Find("Main Camera").GetComponent<Camera>();
            backCam     = this.gameObject.transform.Find("Cine_backCam").GetComponent<CinemachineVirtualCameraBase>();
            //wideCam     = this.gameObject.transform.Find("Cine_wideCam").GetComponent<CinemachineVirtualCameraBase>();
            sholderCam  = this.gameObject.transform.Find("Cine_sholderCam").GetComponent<CinemachineVirtualCameraBase>();
            //wallCam         = GameObject.FindGameObjectWithTag("CAM_WallView").GetComponent<CinemachineVirtualCameraBase>();
            //topCam = GameObject.FindGameObjectWithTag("CAM_TopVIew").GetComponent<CinemachineVirtualCameraBase>();

            camList.Add(backCam);
            //camList.Add(wideCam);
            camList.Add(sholderCam);
            //camList.Add(wallCam);
            //camList.Add(topCam);

            //lookatObj = this.gameObject.transform.Find("LookAtObj_Default").GetComponent<Transform>();
            //followObj = this.gameObject.transform.Find("FollowObj").GetComponent<Transform>();

            curCam = new CamState();
            preCam = new CamState();

           

        }

        void Start()
        {

            curCam = CamState.back;
            OnOffCamera(camList[(int)curCam]);

          
            
        }

        void Update()
        {
           

           
            SetCamera();
        }

        void SetCamera()
        {
            if (curCam == CamState.back && ITT_KeyManager.Instance.GetKeyDown(PlayerAction.Aim))
            {
                AxisState temp = backCam.GetComponent<CinemachineFreeLook>().m_XAxis;

                
                curCam = CamState.sholder;

                OnOffCamera(sholderCam);

                sholderCam.GetComponent<CinemachineFreeLook>().m_XAxis = temp;

                Debug.Log("Aim On!");

            }
            else if (curCam == CamState.sholder && ITT_KeyManager.Instance.GetKeyDown(PlayerAction.Aim))
            {
                AxisState temp = sholderCam.GetComponent<CinemachineFreeLook>().m_XAxis;

                
                curCam = CamState.back;
                OnOffCamera(backCam);
                Debug.Log("Aim Off!");
                

                backCam.GetComponent<CinemachineFreeLook>().m_XAxis = temp;

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

        // ================================================================================================================ //

        //void CheckCameraBlend_Clone() // Clone�� ���� ���� ������ true : �ٸ� blend 100% ������ ȸ�ͽ� ���� ���� �ʿ�
        //{
        //    isBlendStart_Clone;
        //    isBlending_Clone;

        //    if (isOriginBlending)
        //    {
        //        if (!isBlendStart_Clone && !isBlending_Clone)
        //        {
        //            isBlendStart_Clone = true;
        //            isBlending_Clone = true;
        //        }
        //        else
        //    }
        //}
    }
}
