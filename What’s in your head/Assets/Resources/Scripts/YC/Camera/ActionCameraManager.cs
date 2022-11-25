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
        [Header("1�� ī�޶� �ӹ����� �ð�")]
        [SerializeField] [Range(0, 10)] float stayTime1 = 2f;
        [Header("2�� ī�޶� �ӹ����� �ð�")]
        [SerializeField] [Range(0, 10)] float stayTime2 = 2f;
        [Space] [Space]

        [Header("ī�޶� �� ��ȯ�� �ɸ��� �ð�")]
        [SerializeField] [Range(0, 10)] float blendingTime = 3f;

        [Space] [Space]
        [SerializeField] List <CinemachineVirtualCamera> CM_VCAMs;

        CinemachineBrain CB;
        float originBlendTime;
        PhotonView pv;

        void Start()
        {           
            pv = this.gameObject.GetComponent<PhotonView>();

            // >> : �׽�Ʈ�ÿ��� ��ٷȴٰ� ����
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

                // << : Owner�� Camera�� Full Screen���� �����Ѵ�
                if (GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient])
                    CameraManager.Instance.InitCamera((int)CharacterCamera.NELLA);
                else
                    CameraManager.Instance.InitCamera((int)CharacterCamera.STEADY);

                // �÷��̾��� ���� ī�޶� ����
                GameManager.Instance.myPlayerTF.GetComponent<CameraController>().OnOffCamera(null);

                // Owner Cinemachine Brain�� ���� �ð��� �������ش�
                if (GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient])
                    CB = CameraManager.Instance.cameras[0].GetComponent<CinemachineBrain>();
                else
                    CB = CameraManager.Instance.cameras[1].GetComponent<CinemachineBrain>();

                originBlendTime = CB.m_DefaultBlend.m_Time;

                CB.m_DefaultBlend.m_Time = blendingTime;

                // �׼� �ڷ�ƾ�� �����Ѵ�
                StartCoroutine(nameof(CameraAction));
            }
            else
            {
                // << : ������ Camera�� Half Screen���� �����Ѵ�
                if (GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient])
                    CameraManager.Instance.SetHalfScreenCor2(true);
                else
                    CameraManager.Instance.SetHalfScreenCor2(false);

                // Owner Cinemachine Brain�� ���� �ð��� �������ش�
                CB.m_DefaultBlend.m_Time = originBlendTime;

                StartCoroutine(ExitDelay());
            }
        }


        IEnumerator CameraAction()
        {
            // << : �÷��̾� ī�޶� -> 1�� ī�޶� ��ȯ��
            CM_VCAMs[0].enabled = true;
            yield return new WaitForSeconds(blendingTime);


            // << : 1�� ī�޶� Stay ��
            yield return new WaitForSeconds(stayTime1);


            // << : �÷��̾� ī�޶� -> 2�� ī�޶� ��ȯ��
            CM_VCAMs[0].enabled = false;
            CM_VCAMs[1].enabled = true;
            yield return new WaitForSeconds(blendingTime);


            // << : 2�� ī�޶� Stay ��
            yield return new WaitForSeconds(stayTime2);


            // << : 2�� ī�޶� -> �÷��̾� ī�޶� ��ȯ��
            foreach (CinemachineVirtualCamera cv in CM_VCAMs)
            {
                cv.enabled = false;
            }
            GameManager.Instance.myPlayerTF.GetComponent<CameraController>().InitSceneChange();
            yield return new WaitForSeconds(blendingTime);
            
            // << : ���� ����
            InitCamera(false);
        }

        IEnumerator ExitDelay()
        {
            // << : ���� ������
            yield return new WaitForSeconds(CameraManager.Instance._LerpTime);

            // << : ���� ����
            CameraManager.Instance.BlockCinemachineInput(false);
            GameManager.Instance.myPlayerTF.GetComponent<PlayerState>().isOutOfControl = false;
        }
    }
}
