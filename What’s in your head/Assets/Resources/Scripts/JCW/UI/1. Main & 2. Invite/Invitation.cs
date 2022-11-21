using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using JCW.Network;
using JCW.AudioCtrl;

namespace JCW.UI
{    public class Invitation : MonoBehaviour
    {
        [Header("�ʴ��� ID �ؽ�Ʈ")] [SerializeField] private Text masterText;
        [Header("���� ��ư")] [SerializeField] private Button acceptButton;
        [Header("���� ��ư")] [SerializeField] private Button declineButton;
        [Header("ģ���� ������ �� ���� UI")][SerializeField] private GameObject readyObj = null;

        private string masterID = null;

        private void Awake()
        {
            // ���� �ִ� �κ� ���� ���� ��, ���� �濡 ����
            acceptButton.onClick.AddListener(() =>
            {
                PhotonManager.Instance.LetMasterMakeRoom(masterID);
                PhotonNetwork.LeaveRoom();
                StartCoroutine(nameof(WaitForRoom), masterID);
            });
            declineButton.onClick.AddListener(() => {
                StopAllCoroutines();
                this.gameObject.SetActive(false); 
            });
        }

        private void OnEnable()
        {
            SoundManager.Instance.PlayUI("SystemMessage");
        }

        public void SetMasterName(string masterName)
        {
            masterID = masterName;
            masterText.text = "ID : " + masterName;
        }

        IEnumerator WaitForRoom(string masterName)
        {
            while (PhotonNetwork.NetworkClientState != ClientState.JoinedLobby)
            {
                Debug.Log("���� �濡�� ������ ��");
                yield return null;
            }
            //bool isJoined = false;

            PhotonNetwork.JoinLobby();
            while (PhotonNetwork.NetworkClientState != ClientState.JoinedLobby)
            {
                Debug.Log("�κ� ���� ��");
                yield return null;
            }

            //while (!isJoined)
            //{
            //    isJoined = PhotonNetwork.JoinRoom(masterName, null);
            //    Debug.Log("�� ���� �õ� ��");
            //    yield return null;
            //}
            //Debug.Log("�� ���� �Ϸ�");
            PhotonNetwork.JoinRoom(masterName, null);
            while (PhotonNetwork.NetworkClientState != ClientState.Joined)
            {
                Debug.Log("�� ���� : " + PhotonNetwork.NetworkClientState);
                if(PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
                    PhotonNetwork.JoinLobby();
                if (PhotonNetwork.NetworkClientState == ClientState.JoinedLobby)
                    PhotonNetwork.JoinRoom(masterName, null);
                yield return null;
            }
            //PhotonNetwork.JoinRoom(masterName, null);
            readyObj.SetActive(true);
            this.gameObject.SetActive(false);
            yield break;
        }
    }
}

