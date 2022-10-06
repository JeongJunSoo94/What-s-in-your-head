using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using JCW.Network;

namespace JCW.UI
{
    public class BeforePlayUI : MonoBehaviour
    {
        [Header("�� ID")] [SerializeField] private Text myID = null;
        [Header("�Էµ� ģ�� ID")] [SerializeField] private Text friendID = null;
        [Header("�˻� ��ư")] [SerializeField] private Button searchButton = null;
        [Header("���� �޴� UI")] [SerializeField] GameObject mainUI = null;
        [Header("�ʴ��� ���� ����")] [SerializeField] GameObject passUI = null;
        [Header("�ʴ��� ���� ����")] [SerializeField] GameObject failUI = null;

        void Start()
        {
            myID.text = PhotonManager.instance.userID;            

            searchButton.onClick.AddListener(() =>
            {                
                for (int i = 0 ; i<PhotonNetwork.PlayerList.Length ; ++i)
                {
                    if (PhotonNetwork.PlayerList[i].NickName==friendID.text
                        && myID.text != friendID.text)
                    {                        
                        failUI.SetActive(false);
                        passUI.SetActive(true);
                        mainUI.SendMessage("TryMakeRoom", friendID.text);
                        return;
                    }
                }
                passUI.SetActive(false);
                failUI.SetActive(true);
            });
        }

        public void GetOutOfRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}

