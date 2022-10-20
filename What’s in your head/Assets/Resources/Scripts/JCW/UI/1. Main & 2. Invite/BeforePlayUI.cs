using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using JCW.Network;

namespace JCW.UI
{
    public class BeforePlayUI : MonoBehaviour
    {
        [Header("내 ID")] [SerializeField] private Text myID = null;
        [Header("입력된 친구 ID")] [SerializeField] private InputField friendID = null;
        [Header("검색 버튼")] [SerializeField] private Button searchButton = null;
        [Header("뒤로가기 버튼")] [SerializeField] private Button backButton = null;
        [Header("초대장 전송 성공")] [SerializeField] GameObject passUI = null;
        [Header("초대장 전송 실패")] [SerializeField] GameObject failUI = null;

        private void Awake()
        {
            myID.text = PhotonManager.Instance.userID;

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
                        PhotonManager.Instance.gameObject.SendMessage("TryMakeRoom", friendID.text);
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

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                searchButton.onClick.Invoke();
        }

        public void GetOutOfRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}

