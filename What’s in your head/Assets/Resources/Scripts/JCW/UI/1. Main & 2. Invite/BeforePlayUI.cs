using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using JCW.Network;

namespace JCW.UI
{
    public class BeforePlayUI : MonoBehaviour
    {
        [Header("�� ID")] [SerializeField] private Text myID = null;
        [Header("�Էµ� ģ�� ID")] [SerializeField] private InputField friendID = null;
        [Header("�˻� ��ư")] [SerializeField] private Button searchButton = null;
        [Header("�ڷΰ��� ��ư")] [SerializeField] private Button backButton = null;
        [Header("�ʴ��� ���� ����")] [SerializeField] GameObject passUI = null;
        [Header("�ʴ��� ���� ����")] [SerializeField] GameObject failUI = null;

        private void Awake()
        {
            //myID.text = PhotonManager.Instance.userID;

            

            backButton.onClick.AddListener(() =>
            {
                if (PhotonNetwork.NetworkClientState.ToString() == ClientState.Joined.ToString())
                    PhotonNetwork.LeaveRoom();
                this.gameObject.SetActive(false);
            });

            searchButton.onClick.AddListener(() =>
            {                       
                for (int i = 0 ; i<PhotonNetwork.PlayerList.Length ; ++i)
                {
                    if (PhotonNetwork.PlayerList[i].NickName==friendID.text
                        && myID.text != friendID.text)
                    {                        
                        failUI.SetActive(false);
                        passUI.SetActive(false);
                        passUI.SetActive(true);
                        PhotonManager.Instance.TryMakeRoom(friendID.text);
                        friendID.text = "";
                        return;
                    }   
                }
                friendID.text = "";
                passUI.SetActive(false);
                failUI.SetActive(false);
                failUI.SetActive(true);
            });
        }

        private void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(nameof(WaitForServer));
        }

        private void OnDisable()
        {
            myID.text = "���� ���� ��..";
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                searchButton.onClick.Invoke();
        }

        IEnumerator WaitForServer()
        {
            yield return new WaitUntil(() => PhotonNetwork.NetworkClientState.ToString() == ClientState.Joined.ToString());

            myID.text = PhotonManager.Instance.userID;
            yield break;
        }
    }
}

