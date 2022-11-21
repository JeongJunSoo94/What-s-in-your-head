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
        [Header("초대자 ID 텍스트")] [SerializeField] private Text masterText;
        [Header("수락 버튼")] [SerializeField] private Button acceptButton;
        [Header("거절 버튼")] [SerializeField] private Button declineButton;
        [Header("친구와 만났을 때 열릴 UI")][SerializeField] private GameObject readyObj = null;

        private string masterID = null;

        private void Awake()
        {
            // 현재 있는 로비 방을 나간 후, 방장 방에 접속
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
                Debug.Log("기존 방에서 나가는 중");
                yield return null;
            }
            //bool isJoined = false;

            PhotonNetwork.JoinLobby();
            while (PhotonNetwork.NetworkClientState != ClientState.JoinedLobby)
            {
                Debug.Log("로비 접속 중");
                yield return null;
            }

            //while (!isJoined)
            //{
            //    isJoined = PhotonNetwork.JoinRoom(masterName, null);
            //    Debug.Log("방 접속 시도 중");
            //    yield return null;
            //}
            //Debug.Log("방 접속 완료");
            PhotonNetwork.JoinRoom(masterName, null);
            while (PhotonNetwork.NetworkClientState != ClientState.Joined)
            {
                Debug.Log("현 상태 : " + PhotonNetwork.NetworkClientState);
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

