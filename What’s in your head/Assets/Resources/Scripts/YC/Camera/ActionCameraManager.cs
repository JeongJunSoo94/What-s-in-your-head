using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YC.CameraManager_;
using Cinemachine;

namespace YC.Camera_
{
    public class ActionCameraManager : MonoBehaviour
    {
        [Header("1번 카메라 머무르는 시간")]
        [SerializeField] [Range(0, 10)] float stayTime1 = 2f;
        [Header("2번 카메라 머무르는 시간")]
        [SerializeField] [Range(0, 10)] float stayTime2 = 2f;
        [Space] [Space]

        [Header("카메라 간 전환에 걸리는 시간")]
        [SerializeField] [Range(0, 10)] float blendingTime = 3f;

        [Space] [Space]
        [SerializeField] List <CinemachineVirtualCamera> CM_VCAMs;

        CinemachineBrain CB;
        float originBlendTime;
        PhotonView pv;

        void Start()
        {           
            pv = this.gameObject.GetComponent<PhotonView>();

            // >> : 테스트시에만 기다렸다가 시작
            StartCoroutine(nameof(WaitForPlayers));

            //if (pv.IsMine)
            //    pv.RPC(nameof(InitCamera), RpcTarget.AllViaServer, true);
        }

        IEnumerator WaitForPlayers()
        {
            yield return new WaitUntil(() => GameManager.Instance.GetCharOnScene(true) && GameManager.Instance.GetCharOnScene(false));

            yield return new WaitForSeconds(0.2f);

            if (pv.IsMine)
                pv.RPC(nameof(InitCamera), RpcTarget.AllViaServer, true);
            
            yield break;
        }


        [PunRPC]
        void InitCamera(bool enter)
        {
            if (enter)
            {
                CameraManager.Instance.BlockCinemachineInput(enter);
                GameManager.Instance.myPlayerTF.GetComponent<PlayerState>().isOutOfControl = enter;

                // << : Owner의 Camera를 Full Screen으로 세팅한다
                if (GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient])
                    CameraManager.Instance.InitCamera((int)CharacterCamera.NELLA);
                else
                    CameraManager.Instance.InitCamera((int)CharacterCamera.STEADY);

                // 플레이어의 가상 카메라를 끈다
                GameManager.Instance.myPlayerTF.GetComponent<CameraController>().OnOffCamera(null);

                // Owner Cinemachine Brain의 블렌딩 시간을 설정해준다
                if (GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient])
                    CB = CameraManager.Instance.cameras[0].GetComponent<CinemachineBrain>();
                else
                    CB = CameraManager.Instance.cameras[1].GetComponent<CinemachineBrain>();

                originBlendTime = CB.m_DefaultBlend.m_Time;

                CB.m_DefaultBlend.m_Time = blendingTime;

                // 액션 코루틴을 실행한다
                StartCoroutine(nameof(CameraAction));
            }
            else
            {
                // << : 각각의 Camera를 Half Screen으로 세팅한다
                if (GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient])
                    CameraManager.Instance.SetHalfScreenCor2(true);
                else
                    CameraManager.Instance.SetHalfScreenCor2(false);

                // Owner Cinemachine Brain의 블렌딩 시간을 설정해준다
                CB.m_DefaultBlend.m_Time = originBlendTime;

                StartCoroutine(ExitDelay());
            }
        }


        IEnumerator CameraAction()
        {
            // << : 플레이어 카메라 -> 1번 카메라 전환중
            CM_VCAMs[0].enabled = true;
            yield return new WaitForSeconds(blendingTime);


            // << : 1번 카메라 Stay 중
            yield return new WaitForSeconds(stayTime1);


            // << : 플레이어 카메라 -> 2번 카메라 전환중
            CM_VCAMs[0].enabled = false;
            CM_VCAMs[1].enabled = true;
            yield return new WaitForSeconds(blendingTime);


            // << : 2번 카메라 Stay 중
            yield return new WaitForSeconds(stayTime2);


            // << : 2번 카메라 -> 플레이어 카메라 전환중
            foreach (CinemachineVirtualCamera cv in CM_VCAMs)
            {
                cv.enabled = false;
            }
            GameManager.Instance.myPlayerTF.GetComponent<CameraController>().InitSceneChange();
            yield return new WaitForSeconds(blendingTime);
            
            // << : 분할 시작
            InitCamera(false);
        }

        IEnumerator ExitDelay()
        {
            // << : 분할 진행중
            yield return new WaitForSeconds(CameraManager.Instance._LerpTime);

            // << : 게임 시작
            CameraManager.Instance.BlockCinemachineInput(false);
            GameManager.Instance.myPlayerTF.GetComponent<PlayerState>().isOutOfControl = false;
        }
    }
}
