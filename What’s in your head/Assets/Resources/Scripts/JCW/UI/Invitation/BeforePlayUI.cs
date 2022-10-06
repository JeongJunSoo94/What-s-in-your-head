using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using JCW.Network;

namespace JCW.UI
{
    public class BeforePlayUI : MonoBehaviour
    {
        [Header("내 ID")] [SerializeField] private Text myID = null;
        [Header("입력된 친구 ID")] [SerializeField] private Text friendID = null;
        [Header("검색 버튼")] [SerializeField] private Button searchButton = null;
        [Header("메인 메뉴 UI")] [SerializeField] GameObject mainUI = null;
        [Header("초대장 전송 성공")] [SerializeField] GameObject passUI = null;
        [Header("초대장 전송 실패")] [SerializeField] GameObject failUI = null;

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

