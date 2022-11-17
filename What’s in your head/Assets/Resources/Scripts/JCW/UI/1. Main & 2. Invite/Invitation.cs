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
            declineButton.onClick.AddListener(() => {  this.gameObject.SetActive(false); });
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
            yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.JoinedLobby);

            while (PhotonNetwork.JoinRoom(masterName, null))
            {
                yield return null;
            }            

            //PhotonNetwork.JoinRoom(masterName, null);
            readyObj.SetActive(true);
            this.gameObject.SetActive(false);
            yield break;
        }
    }
}

