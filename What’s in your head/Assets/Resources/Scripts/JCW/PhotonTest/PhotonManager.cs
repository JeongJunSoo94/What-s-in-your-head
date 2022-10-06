using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

// MonoBehaviourPun : Callback �Լ� �����͵��� �������̵� �� �� ����
namespace JCW.Network
{
    [RequireComponent(typeof(PhotonView))]
    public class PhotonManager : MonoBehaviourPunCallbacks
    {
        [Header("ID ����")] [SerializeField] [Range(1,8)] private uint LengthID = 6;
        //���� �Է�
        private readonly string version = "1.0";

        //����� ���̵� �Է�
        [HideInInspector] public string userID = "�͸�";
        [HideInInspector] public PhotonView myPhotonView;
        [HideInInspector] public RoomOptions myRoomOptions;


        //�̱���
        public static PhotonManager instance = null;


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                myPhotonView = gameObject.GetComponent<PhotonView>();
                myRoomOptions = new()
                {
                    MaxPlayers = 2,    // �ִ� �����ڼ�, ���� ����� 20CCU�̹Ƿ� 20 �ʰ��δ� ���Ѵ�.
                    IsOpen = true,      // ���� ���� ����
                    IsVisible = false,   // �κ񿡼� �� ��Ͽ� �����ų�� ����     
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
            // ���� ���� �����鿡�� �ڵ����� ���� �ε�
            PhotonNetwork.AutomaticallySyncScene = true;
            // ���� ������ �������� ���� ���
            PhotonNetwork.GameVersion = version;

            // ���� ������ ��� Ƚ�� ����. �ʴ� 30ȸ
            Debug.Log("�������� �ʴ� ��� Ƚ�� : " + PhotonNetwork.SendRate);

            // ������ ���� ������ ���̵� ����
            int minID = (int)Mathf.Pow(10, LengthID-1);
            int maxID = (int)Mathf.Pow(10, LengthID);
            var random = new System.Random(Guid.NewGuid().GetHashCode()).Next(minID, maxID);

            // ���� ���̵� �Ҵ�
            userID = random.ToString();

            PhotonNetwork.LocalPlayer.NickName = userID;

            // ���� ����
            PhotonNetwork.ConnectUsingSettings();
        }

        // ���� ������ ���� �� ȣ��Ǵ� �ݹ� �Լ�
        public override void OnConnectedToMaster()
        {
            Debug.Log("������ ���� �Ϸ� !");
            PhotonNetwork.JoinLobby(); // �κ� ���� 
                                       //PhotonNetwork.LoadLevel(SceneLobby);
        }

        // �κ� ���� �� ȣ��Ǵ� �ݹ� �Լ�
        public override void OnJoinedLobby()
        {
            Debug.Log($"�κ� ���� ���� : {PhotonNetwork.InLobby}");
        }

        // �� ������ �Ϸ�� �� ȣ��Ǵ� �ݹ� �Լ�
        public override void OnCreatedRoom()
        {
            Debug.Log("�� ���� �Ϸ�");
            Debug.Log($"�� �̸� : {PhotonNetwork.CurrentRoom.Name}");
        }

        // �뿡 ������ �� ȣ��Ǵ� �ݹ� �Լ�
        public override void OnJoinedRoom()
        {
            Debug.Log("������ �� �̸� : " + PhotonNetwork.CurrentRoom.Name);

            // �뿡 ������ ����� ���� Ȯ��
            foreach (var player in PhotonNetwork.CurrentRoom.Players)
            {
                // $ => String.Format() : $"" �ֵ���ǥ �ȿ� �ִ� ������ ��Ʈ������ �ٲ���־��.            
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
