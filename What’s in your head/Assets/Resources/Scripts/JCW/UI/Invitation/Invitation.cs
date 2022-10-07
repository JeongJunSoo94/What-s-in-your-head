using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using JCW.Network;

namespace JCW.UI
{    public class Invitation : MonoBehaviour
    {
        [Header("초대자 ID 텍스트")] [SerializeField] private Text masterText;
        [Header("수락 버튼")] [SerializeField] private Button acceptButton;
        [Header("거절 버튼")] [SerializeField] private Button declineButton;
        [Header("메인 메뉴 UI")] [SerializeField] GameObject mainUI = null;
        [Header("친구와 만났을 때 열릴 UI")][SerializeField] private GameObject readyObj = null;

        private string masterID = null;

        private void Awake()
        {
            // 현재 있는 로비 방을 나간 후, 방장 방에 접속
            acceptButton.onClick.AddListener(() =>
            {
                PhotonManager.instance.gameObject.SendMessage("LetMasterMakeRoom", masterID);
                PhotonNetwork.LeaveRoom();
                StartCoroutine(nameof(WaitForRoom), masterID);
            });
            declineButton.onClick.AddListener(() =>
            {                
                this.gameObject.SetActive(false);
            });
        }
        void SetMasterName(string masterName)
        {
            masterID = masterName;
            masterText.text = "ID : " + masterName;
        }

        IEnumerator WaitForRoom(string masterName)
        {
            while (PhotonNetwork.NetworkClientState.ToString() != ClientState.JoinedLobby.ToString())
            {
                yield return new WaitForSeconds(0.5f);
            }            
            
            PhotonNetwork.JoinRoom(masterName, null);
            readyObj.SetActive(true);
            this.gameObject.SetActive(false);
            yield return null;
        }
    }
}

