using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어를 Instantiate 하기 위한 임시 스크립트
/// </summary>

using Photon.Pun;
using Photon.Realtime;
using System.Linq;

namespace YC.Photon
{
    public class TempPhotonManager : MonoBehaviourPunCallbacks
    {
        // 버전 입력
        private readonly string version = "1.0f";

        // 사용자 아이디 입력
        private string userId = "Mary";

        

        void Awake()
        {
            Screen.SetResolution(1920, 1080, false);

            // 같은 룸의 유저들에게 자동으로 씬을 로딩
            PhotonNetwork.AutomaticallySyncScene = true;

            // 같은 버전의 유저끼리 접속 허용
            PhotonNetwork.GameVersion = version;

            // 유저 아이디 할당
            PhotonNetwork.NickName = userId;

            // << FOV를 위한 설정 
            PhotonNetwork.SendRate = 60;
            PhotonNetwork.SerializationRate = 30;

            Debug.Log(PhotonNetwork.SendRate);

            // 서버 접속
            PhotonNetwork.ConnectUsingSettings();

        }

        // 포톤 서버에 접속시 호출되는 콜백 함수
        public override void OnConnectedToMaster()
        {
            Debug.Log("Conntected to Master!");

            // 로비 입장 여부를 로그로 띄움 (아직 로비 입장 전이니 당연히 false)
            Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");

            // 로비 입장
            PhotonNetwork.JoinLobby();
        }

        // 로비 입장시 호출되는 콜백 함수
        public override void OnJoinedLobby()
        {
            Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}"); // true;

            // 생성되어 있는 룸들 중에서 랜덤하게 입장 (랜덤 매치메이킹 기능)
            PhotonNetwork.JoinRandomRoom();
        }

        // 랜덤 매치메이킹 실패시 호출되는 콜백 함수
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"JoinRandom Failed {returnCode} : {message}");

            // 룸의 속성 정의
            RoomOptions ro = new RoomOptions();
            ro.MaxPlayers = 20;     // 최대 접속자 수 : 20명
            ro.IsOpen = true;       // 룸의 오픈 여부
            ro.IsVisible = true;    // 로비에서 룸 목록에 노출 여부

            // 룸 생성
            PhotonNetwork.CreateRoom("My Room", ro);
        }

        // 룸 생성이 완료시 호출되는 콜백 함수
        public override void OnCreatedRoom()
        {
            Debug.Log("Created Room");
            Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
        }

        // 룸에 입장시 호출되는 콜백 함수
        public override void OnJoinedRoom()
        {
            Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
            Debug.Log($"PlayerCount = {PhotonNetwork.CurrentRoom.PlayerCount}");

            // 룸에 접속한 사용자 정보 확인
            foreach (var player in PhotonNetwork.CurrentRoom.Players)
            {
                Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}");
            }




            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {              
                PhotonNetwork.Instantiate("Prefabs/YC/Player", new Vector3 (-5, 0, 0), Quaternion.identity);
            }
            else if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                PhotonNetwork.Instantiate("Prefabs/YC/Player", new Vector3(5, 0, 0), Quaternion.identity);
            }
        }
    }
}
