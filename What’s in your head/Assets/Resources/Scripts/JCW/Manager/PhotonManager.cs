using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using JCW.UI;

// MonoBehaviourPun : Callback 함수 같은것들을 오버라이드 할 수 없음
namespace JCW.Network
{
    [RequireComponent(typeof(PhotonView))]
    public class PhotonManager : MonoBehaviourPunCallbacks
    {
        [Header("ID 길이")] [SerializeField] [Range(1,8)] private uint LengthID = 6;
        [Header("친구와 만났을 때 열릴 UI")] [SerializeField] private GameObject readyUI = null;      
        [Header("초대장")]   [SerializeField] private Invitation InvitationUI;
        [Header("해상도")]   [SerializeField] private int width = 1920;
                            [SerializeField] private int height = 1080;
                            [SerializeField] private bool isFullScreen = true;
        //버전 입력
        private readonly string version = "1.0";

        //사용자 아이디 입력
        [HideInInspector] public string userID = "익명";
        [HideInInspector] public PhotonView myPhotonView;
        [HideInInspector] public RoomOptions myRoomOptions;


        //싱글톤
        public static PhotonManager Instance = null;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                myPhotonView = gameObject.GetComponent<PhotonView>();
                myRoomOptions = new()
                {
                    MaxPlayers = 2,    // 최대 접속자수, 포톤 무료는 20CCU이므로 20 초과로는 못한다.
                    IsOpen = true,      // 룸의 오픈 여부
                    IsVisible = false,   // 로비에서 룸 목록에 노출시킬지 여부     
                };
                DontDestroyOnLoad(this.gameObject);
            }
            else if (Instance != this)
            {
                Debug.Log("싱글톤에 어긋났으므로 삭제합니다");
                Destroy(this.gameObject);
            }

            // << FOV를 위한 설정 
            PhotonNetwork.SendRate = 60;
            PhotonNetwork.SerializationRate = 30;

            Screen.SetResolution(width, height, isFullScreen);
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
            Debug.Log($"로비 접속 성공");
        }

        // 룸 생성이 완료된 후 호출되는 콜백 함수
        public override void OnCreatedRoom()
        {
            Debug.Log("방 생성 완료");
        }

        // 룸에 입장한 후 호출되는 콜백 함수
        public override void OnJoinedRoom()
        {
            // 룸에 접속한 사용자 정보 확인
            foreach (var player in PhotonNetwork.CurrentRoom.Players)
            {
                // $ => String.Format() : $"" 쌍따옴표 안에 있는 내용을 스트링으로 바꿔어주어라.            
                Debug.Log($"ID : {player.Value.NickName}");
            }
            //StartCoroutine(nameof(MakeChar));
        }
        public void ChangeStage()
        {            
            PhotonNetwork.LoadLevel(++GameManager.Instance.curStageIndex);
            if (GameManager.Instance.curStageIndex == 1)
                StartCoroutine(nameof(MakeChar));
            //if(PhotonNetwork.IsMasterClient)
            //myPhotonView.RPC(nameof(ChangeStageRPC), RpcTarget.AllViaServer);
        }

        // 친구 검색창에서 돋보기 버튼 누르면 작동
        public void TryMakeRoom(string friendName)
        {
            photonView.RPC(nameof(GetInvitation), RpcTarget.Others, friendName, PhotonNetwork.LocalPlayer.NickName);
        }


        // 초대장 받는 사람 기준의 함수.
        [PunRPC]
        void GetInvitation(string inviteeName, string masterName)
        {
            if (inviteeName == PhotonNetwork.LocalPlayer.NickName)
            {
                InvitationUI.gameObject.SetActive(true);
                InvitationUI.SetMasterName(masterName);
            }
        }

        public void LetMasterMakeRoom(string masterName)
        {
            photonView.RPC("MakeRoom", RpcTarget.Others, masterName);
        }

        [PunRPC]
        void MakeRoom(string masterName)
        {            
            if (masterName == PhotonNetwork.LocalPlayer.NickName)
            {                
                PhotonNetwork.LeaveRoom();
                StartCoroutine(nameof(WaitForRoom), masterName);
            }
        }

        IEnumerator WaitForRoom(string masterName)
        {
            while (PhotonNetwork.NetworkClientState.ToString() != ClientState.JoinedLobby.ToString())
            {
                yield return new WaitForSeconds(0.05f);
            }
            PhotonNetwork.JoinOrCreateRoom(masterName, myRoomOptions, null);
            readyUI.SetActive(true);
            yield return null;
        }

        IEnumerator MakeChar()
        {
            yield return new WaitForSeconds(0.3f);

            // 넬라인 지 아닌지 판단해서 캐릭터 생성해주면 됨
            // 현재 자신이 마스터인지 아닌지와 어떤 캐릭터를 선택했는지가 담겨있음.
            GameManager.Instance.SetRandomSeed();
            if (GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient])
            {
                Debug.Log("넬라 생성");
                PhotonNetwork.Instantiate("Prefabs/YC/MainCamera_Nella", new Vector3(0, 0, 0), Quaternion.identity, 0);
                PhotonNetwork.Instantiate("Prefabs/JJS/NellaMousePoint", new Vector3(-10, 0, -5), Quaternion.identity);
                PhotonNetwork.Instantiate("Prefabs/JJS/JJS_Nella", new Vector3(-5, 0, 0), Quaternion.identity);
            }
            else
            {
                Debug.Log("스테디 생성");
                PhotonNetwork.Instantiate("Prefabs/YC/MainCamera_Steady", new Vector3(0, 0, 0), Quaternion.identity, 0);
                PhotonNetwork.Instantiate("Prefabs/JJS/SteadyMousePoint", new Vector3(10, 0, -5), Quaternion.identity);
                PhotonNetwork.Instantiate("Prefabs/JJS/JJS_Steady", new Vector3(5, 0, 0), Quaternion.identity);
            }
            StopAllCoroutines();
        }
    }

}
