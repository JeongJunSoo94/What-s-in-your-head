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
        [Header("�ʴ��� ID �ؽ�Ʈ")] [SerializeField] private Text masterText;
        [Header("���� ��ư")] [SerializeField] private Button acceptButton;
        [Header("���� ��ư")] [SerializeField] private Button declineButton;
        [Header("���� �޴� UI")] [SerializeField] GameObject mainUI = null;
        [Header("ģ���� ������ �� ���� UI")][SerializeField] private GameObject readyObj = null;

        private string masterID = null;

        private void Awake()
        {
            // ���� �ִ� �κ� ���� ���� ��, ���� �濡 ����
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

