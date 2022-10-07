using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾ Instantiate �ϱ� ���� �ӽ� ��ũ��Ʈ
/// </summary>

using Photon.Pun;
using Photon.Realtime;
using System.Linq;

namespace YC.Photon
{
    public class TempPhotonManager : MonoBehaviourPunCallbacks
    {
        // ���� �Է�
        private readonly string version = "1.0f";

        // ����� ���̵� �Է�
        private string userId = "Mary";

        

        void Awake()
        {
            Screen.SetResolution(1920, 1080, false);

            // ���� ���� �����鿡�� �ڵ����� ���� �ε�
            PhotonNetwork.AutomaticallySyncScene = true;

            // ���� ������ �������� ���� ���
            PhotonNetwork.GameVersion = version;

            // ���� ���̵� �Ҵ�
            PhotonNetwork.NickName = userId;

            // << FOV�� ���� ���� 
            PhotonNetwork.SendRate = 60;
            PhotonNetwork.SerializationRate = 30;

            Debug.Log(PhotonNetwork.SendRate);

            // ���� ����
            PhotonNetwork.ConnectUsingSettings();

        }

        // ���� ������ ���ӽ� ȣ��Ǵ� �ݹ� �Լ�
        public override void OnConnectedToMaster()
        {
            Debug.Log("Conntected to Master!");

            // �κ� ���� ���θ� �α׷� ��� (���� �κ� ���� ���̴� �翬�� false)
            Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");

            // �κ� ����
            PhotonNetwork.JoinLobby();
        }

        // �κ� ����� ȣ��Ǵ� �ݹ� �Լ�
        public override void OnJoinedLobby()
        {
            Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}"); // true;

            // �����Ǿ� �ִ� ��� �߿��� �����ϰ� ���� (���� ��ġ����ŷ ���)
            PhotonNetwork.JoinRandomRoom();
        }

        // ���� ��ġ����ŷ ���н� ȣ��Ǵ� �ݹ� �Լ�
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"JoinRandom Failed {returnCode} : {message}");

            // ���� �Ӽ� ����
            RoomOptions ro = new RoomOptions();
            ro.MaxPlayers = 20;     // �ִ� ������ �� : 20��
            ro.IsOpen = true;       // ���� ���� ����
            ro.IsVisible = true;    // �κ񿡼� �� ��Ͽ� ���� ����

            // �� ����
            PhotonNetwork.CreateRoom("My Room", ro);
        }

        // �� ������ �Ϸ�� ȣ��Ǵ� �ݹ� �Լ�
        public override void OnCreatedRoom()
        {
            Debug.Log("Created Room");
            Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
        }

        // �뿡 ����� ȣ��Ǵ� �ݹ� �Լ�
        public override void OnJoinedRoom()
        {
            Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
            Debug.Log($"PlayerCount = {PhotonNetwork.CurrentRoom.PlayerCount}");

            // �뿡 ������ ����� ���� Ȯ��
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
