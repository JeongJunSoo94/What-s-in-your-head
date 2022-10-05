using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

// MonoBehaviourPun : Callback 함수 같은것들을 오버라이드 할 수 없음
public class PhotonManager : MonoBehaviourPunCallbacks
{
    [SerializeField]    private string SceneMenu;
    [SerializeField]    private string SceneLobby;
    [SerializeField]    private string SceneRoom;
    //싱글톤
    public static PhotonManager instance = null;

    //버전 입력
    private readonly string version = "1.0";

    //사용자 아이디 입력
    private string userID = "익명";

    //아이디 겹칠 때를 위한
    private string name_number = "";

    [Header("닉네임 입력")]
    public InputField NicknameInput;

    [HideInInspector] public bool isFull = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }

        Screen.SetResolution(1200, 880, false);

    }

    public void Connect()
    {
        // 같은 룸의 유저들에게 자동으로 씬을 로딩
        PhotonNetwork.AutomaticallySyncScene = true;
        // 같은 버전의 유저끼리 접속 허용
        PhotonNetwork.GameVersion = version;

        // 포톤 서버와 통신 횟수 설정. 초당 30회
        Debug.Log("서버와의 초당 통신 횟수 : " + PhotonNetwork.SendRate);

        // 유저 아이디 할당
        PhotonNetwork.LocalPlayer.NickName = NicknameInput == null ? userID + name_number : NicknameInput.text;        
        // 서버 접속
        PhotonNetwork.ConnectUsingSettings();
    }

    // 포톤 서버에 접속 후 호출되는 콜백 함수
    public override void OnConnectedToMaster()
    {
        Debug.Log("서버와 연결 완료 !");
        PhotonNetwork.JoinLobby(); // 로비 입장 
        PhotonNetwork.LoadLevel(SceneLobby);
    }    

    // 로비에 접속 후 호출되는 콜백 함수
    public override void OnJoinedLobby()
    {        
        Debug.Log($"로비 접속 성공 : {PhotonNetwork.InLobby}");
        //PhotonNetwork.JoinRandomRoom(); // 랜덤 매치메이킹 기능 제공
    }

    // 랜덤한 룸 입장이 실패했을 경우 호출되는 콜백 함수
    /*
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"방 접속에 실패하여 방을 만듭니다. {returnCode}:{message}");

        // 룸의 속성 정의
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;    // 최대 접속자수, 포톤 무료는 20CCU이므로 20 초과로는 못한다.
        roomOptions.IsOpen = true;      // 룸의 오픈 여부
        roomOptions.IsVisible = true;   // 로비에서 룸 목록에 노출시킬지 여부

        // 룸 생성
        PhotonNetwork.CreateRoom("My Room", roomOptions);        
    }
    */

    // 룸 생성이 완료된 후 호출되는 콜백 함수
    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성 완료");
        Debug.Log($"방 이름 : {PhotonNetwork.CurrentRoom.Name}");
    }

    // 룸에 입장한 후 호출되는 콜백 함수
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(SceneRoom);
        // 중복 닉네임 방지를 위한 추가 이름
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            if (player.Value == PhotonNetwork.LocalPlayer)
                continue;
            if (player.Value.NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                if (name_number == "")
                    name_number = "1";
                else
                    name_number = (Convert.ToInt32(name_number) + 1).ToString();
            }
        }
        PhotonNetwork.LocalPlayer.NickName += name_number;
        string isMaster = PhotonNetwork.LocalPlayer.IsMasterClient ? "(방장)" : "";
        Debug.Log(isMaster + PhotonNetwork.LocalPlayer.NickName + "님" + $" 방 입장 완료 : {PhotonNetwork.InRoom}");
        Debug.Log($"플레이어 수 : {PhotonNetwork.CurrentRoom.PlayerCount}");

        // 룸에 접속한 사용자 정보 확인
        foreach(var player in PhotonNetwork.CurrentRoom.Players)
        {
            // $ => String.Format() : $"" 쌍따옴표 안에 있는 내용을 스트링으로 바꿔어주어라.            
            Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}");
        }
        StartCoroutine(nameof(MakeChar));
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"방 접속에 실패했습니다. {returnCode}:{message}");

        if(SceneManager.GetActiveScene().name == SceneLobby)
        {
            LobbyUI.instance.gameObject.SetActive(true);
            LobbyUI.instance.WarningObj.SetActive(true);
            LobbyUI.instance.WarningObj.transform.GetChild(1).gameObject.GetComponent<Text>().text = "접속할 수 없는 방입니다 !";

        }
    }

    IEnumerator MakeChar()
    {
        yield return new WaitForSeconds(0.3f);
                
        PhotonNetwork.Instantiate("Prefabs/JCW/SoundManager/SoundManager", Vector3.zero, Quaternion.identity);
        PhotonNetwork.Instantiate("Prefabs/JCW/Photon/Player", Vector3.zero, Quaternion.identity);
        StopCoroutine(nameof(MakeChar));
    }

}
