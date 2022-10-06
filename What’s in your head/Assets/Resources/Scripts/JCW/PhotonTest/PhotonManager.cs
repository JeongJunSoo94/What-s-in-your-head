using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

// MonoBehaviourPun : Callback 함수 같은것들을 오버라이드 할 수 없음
namespace JCW.Network
{
    [RequireComponent(typeof(PhotonView))]
    public class PhotonManager : MonoBehaviourPunCallbacks
    {
        [Header("ID 길이")] [SerializeField] [Range(1,8)] private uint LengthID = 6;
        //버전 입력
        private readonly string version = "1.0";

        //사용자 아이디 입력
        [HideInInspector] public string userID = "익명";
        [HideInInspector] public PhotonView myPhotonView;
        [HideInInspector] public RoomOptions myRoomOptions;


        //싱글톤
        public static PhotonManager instance = null;


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                myPhotonView = gameObject.GetComponent<PhotonView>();
                myRoomOptions = new()
                {
                    MaxPlayers = 2,    // 최대 접속자수, 포톤 무료는 20CCU이므로 20 초과로는 못한다.
                    IsOpen = true,      // 룸의 오픈 여부
                    IsVisible = false,   // 로비에서 룸 목록에 노출시킬지 여부     
                };
                DontDestroyOnLoad(this.gameObject);
            }
            else if (instance != this)
            {
                Destroy(this.gameObject);
            }

            Screen.SetResolution(1920, 1080, true);
        }
        public void Connect()
        {
            // 같은 룸의 유저들에게 자동으로 씬을 로딩
            PhotonNetwork.AutomaticallySyncScene = true;
            // 같은 버전의 유저끼리 접속 허용
            PhotonNetwork.GameVersion = version;

            // 포톤 서버와 통신 횟수 설정. 초당 30회
            Debug.Log("서버와의 초당 통신 횟수 : " + PhotonNetwork.SendRate);

            // 무작위 완전 난수로 아이디 설정
            int minID = (int)Mathf.Pow(10, LengthID-1);
            int maxID = (int)Mathf.Pow(10, LengthID);
            var random = new System.Random(Guid.NewGuid().GetHashCode()).Next(minID, maxID);

            // 유저 아이디 할당
            userID = random.ToString();

            PhotonNetwork.LocalPlayer.NickName = userID;

            // 서버 접속
            PhotonNetwork.ConnectUsingSettings();
        }

        // 포톤 서버에 접속 후 호출되는 콜백 함수
        public override void OnConnectedToMaster()
        {
            Debug.Log("서버와 연결 완료 !");
            PhotonNetwork.JoinLobby(); // 로비 입장 
                                       //PhotonNetwork.LoadLevel(SceneLobby);
        }

        // 로비에 접속 후 호출되는 콜백 함수
        public override void OnJoinedLobby()
        {
            Debug.Log($"로비 접속 성공 : {PhotonNetwork.InLobby}");
        }

        // 룸 생성이 완료된 후 호출되는 콜백 함수
        public override void OnCreatedRoom()
        {
            Debug.Log("방 생성 완료");
            Debug.Log($"방 이름 : {PhotonNetwork.CurrentRoom.Name}");
        }

        // 룸에 입장한 후 호출되는 콜백 함수
        public override void OnJoinedRoom()
        {
            Debug.Log("접속한 방 이름 : " + PhotonNetwork.CurrentRoom.Name);

            // 룸에 접속한 사용자 정보 확인
            foreach (var player in PhotonNetwork.CurrentRoom.Players)
            {
                // $ => String.Format() : $"" 쌍따옴표 안에 있는 내용을 스트링으로 바꿔어주어라.            
                Debug.Log($"ID : {player.Value.NickName}");
            }
            //StartCoroutine(nameof(MakeChar));
        }

        IEnumerator MakeChar()
        {
            yield return new WaitForSeconds(0.3f);

            PhotonNetwork.Instantiate("Prefabs/JCW/SoundManager/SoundManager", Vector3.zero, Quaternion.identity);
            PhotonNetwork.Instantiate("Prefabs/JCW/Photon/Player", Vector3.zero, Quaternion.identity);
            StopCoroutine(nameof(MakeChar));
        }
    }

}
