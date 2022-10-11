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
        [Header("ģ���� ������ �� ���� UI")] [SerializeField] private GameObject readyUI = null;      
        [Header("�ʴ���")] [SerializeField] private GameObject InvitationUI;
        [Header("�ػ�")] [SerializeField] private int width = 1920;
                            [SerializeField] private int height = 1080;
                            [SerializeField] private bool isFullScreen = true;
        //���� �Է�
        private readonly string version = "1.0";

        //����� ���̵� �Է�
        [HideInInspector] public string userID = "�͸�";
        [HideInInspector] public PhotonView myPhotonView;
        [HideInInspector] public RoomOptions myRoomOptions;


        //�̱���
        public static PhotonManager Instance = null;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                myPhotonView = gameObject.GetComponent<PhotonView>();
                myRoomOptions = new()
                {
                    MaxPlayers = 2,    // �ִ� �����ڼ�, ���� ����� 20CCU�̹Ƿ� 20 �ʰ��δ� ���Ѵ�.
                    IsOpen = true,      // ���� ���� ����
                    IsVisible = false,   // �κ񿡼� �� ��Ͽ� �����ų�� ����     
                };
                DontDestroyOnLoad(this.gameObject);
            }
            else if (Instance != this)
            {
                Debug.Log("�̱��濡 ��߳����Ƿ� �����մϴ�");
                Destroy(this.gameObject);
            }

            // << FOV�� ���� ���� 
            PhotonNetwork.SendRate = 60;
            PhotonNetwork.SerializationRate = 30;

            Screen.SetResolution(width, height, isFullScreen);
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
            Debug.Log($"�κ� ���� ����");
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
        public void ChangeStage()
        {
            myPhotonView.RPC(nameof(ChangeStageRPC), RpcTarget.AllViaServer);
        }

        [PunRPC]
        public void ChangeStageRPC()
        {
            PhotonNetwork.LoadLevel(GameManager.Instance.curStageIndex);
            if (GameManager.Instance.curStageIndex != 0)
                StartCoroutine(nameof(MakeChar));
        }

        // ģ�� �˻�â���� ������ ��ư ������ �۵�
        public void TryMakeRoom(string friendName)
        {
            photonView.RPC("GetInvitation", RpcTarget.Others, friendName, PhotonNetwork.LocalPlayer.NickName);
        }


        // �ʴ��� �޴� ��� ������ �Լ�.
        [PunRPC]
        void GetInvitation(string inviteeName, string masterName)
        {
            if (inviteeName == PhotonNetwork.LocalPlayer.NickName)
            {
                InvitationUI.SetActive(true);
                InvitationUI.SendMessage("SetMasterName", masterName);
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

            // �ڶ��� �� �ƴ��� �Ǵ��ؼ� ĳ���� �������ָ� ��
            // ���� �ڽ��� ���������� �ƴ����� � ĳ���͸� �����ߴ����� �������.

            //if(GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient]) 
            //      �ڶ� ����
            //else
            //      ���׵� ����


            PhotonNetwork.Instantiate("Prefabs/JCW/SoundManager/SoundManager", Vector3.zero, Quaternion.identity);
            PhotonNetwork.Instantiate("Prefabs/JCW/Photon/Player", Vector3.zero, Quaternion.identity);
            StopCoroutine(nameof(MakeChar));
        }
    }

}
