using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using JCW.UI;

// MonoBehaviourPun : Callback �Լ� �����͵��� �������̵� �� �� ����
namespace JCW.Network
{
    [RequireComponent(typeof(PhotonView))]
    public class PhotonManager : MonoBehaviourPunCallbacks
    {
        [Header("ID ����")] [SerializeField] [Range(1, 8)] private uint LengthID = 6;
        [Header("ģ���� ������ �� ���� UI")] [SerializeField] private GameObject readyUI = null;
        [Header("�ʴ���")] [SerializeField] private Invitation InvitationUI;
        [Header("�ػ�")] [SerializeField] private int width = 1920;
        [SerializeField] private int height = 1080;
        [SerializeField] private bool isFullScreen = true;
        [Header("�ڶ� ������")] [SerializeField] bool isNella = true;

        bool isSingle = false;
        [Header("�̱� �׽�Ʈ �� ����")] [SerializeField] string tempTitle = "Test";
        [Header("�ڶ� ������ ���")] [SerializeField] string nellaPrefabDirectory = "Prefabs/Player/Nella";
        [Header("���׵� ������ ���")] [SerializeField] string steadyPrefabDirectory = "Prefabs/Player/Steady";
        //���� �Է�
        private readonly string version = "1.0";

        //����� ���̵� �Է�
        [HideInInspector] public string userID = "�͸�";
        [HideInInspector] public PhotonView myPhotonView;
        [HideInInspector] public RoomOptions myRoomOptions;


        //�̱���
        public static PhotonManager Instance = null;


        Vector3 startPos;
        Vector3 intervalPos;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                myPhotonView = GetComponent<PhotonView>();
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
            isSingle = GameManager.Instance.isTest;
            // << FOV�� ���� ���� 
            PhotonNetwork.SendRate = 60;
            PhotonNetwork.SerializationRate = 30;
            if (isSingle)
                Connect();
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
            int minID = (int)Mathf.Pow(10, LengthID - 1);
            int maxID = (int)Mathf.Pow(10, LengthID);
            var random = new System.Random(Guid.NewGuid().GetHashCode()).Next(minID, maxID);

            // ���� ���̵� �Ҵ�
            userID = random.ToString();
            for (int i = 0 ; i < PhotonNetwork.PlayerListOthers.Length ; ++i)
            {
                if (PhotonNetwork.PlayerListOthers[i].NickName == userID)
                {
                    userID = (random - 1).ToString();
                    break;
                }
            }

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
            if (isSingle)
                PhotonNetwork.JoinOrCreateRoom(tempTitle, myRoomOptions, null);
        }

        // �� ������ �Ϸ�� �� ȣ��Ǵ� �ݹ� �Լ�
        public override void OnCreatedRoom()
        {
            Debug.Log("�� ���� �Ϸ�");
        }

        // �뿡 ������ �� ȣ��Ǵ� �ݹ� �Լ�
        public override void OnJoinedRoom()
        {
            // �뿡 ������ ����� ���� Ȯ��
            foreach (var player in PhotonNetwork.CurrentRoom.Players)
            {
                // $ => String.Format() : $"" �ֵ���ǥ �ȿ� �ִ� ������ ��Ʈ������ �ٲ���־��.            
                Debug.Log($"ID : {player.Value.NickName}");
            }
            //StartCoroutine(nameof(MakeChar));
        }
        public void MakeCharacter(Vector3 startPosition, Vector3 intervalPosition)
        {
            startPos = startPosition;
            intervalPos = intervalPosition;
            StartCoroutine(nameof(MakeChar));
            //PhotonNetwork.LoadLevel(GameManager.Instance.curStageIndex * 2 - 2 + GameManager.Instance.curStageType);
            //myPhotonView.RPC(nameof(StartMakeChar), RpcTarget.AllViaServer, PhotonNetwork.IsMasterClient);

            //if(PhotonNetwork.IsMasterClient)
            //myPhotonView.RPC(nameof(ChangeStageRPC), RpcTarget.AllViaServer);
        }

        public void DestroyCurrentRoom_RPC()
        {
            myPhotonView.RPC(nameof(DestroyCurrentRoom), RpcTarget.AllViaServer);
        }

        [PunRPC]
        void DestroyCurrentRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        // ģ�� �˻�â���� ������ ��ư ������ �۵�
        public void TryMakeRoom(string friendName)
        {
            photonView.RPC(nameof(GetInvitation), RpcTarget.Others, friendName, PhotonNetwork.LocalPlayer.NickName);
        }


        // �ʴ��� �޴� ��� ������ �Լ�.
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
            photonView.RPC(nameof(MakeRoom), RpcTarget.Others, masterName);
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
                yield return null;
            }
            PhotonNetwork.JoinOrCreateRoom(masterName, myRoomOptions, null);
            readyUI.SetActive(true);
            yield break;
        }

        IEnumerator MakeChar()
        {
            if (!isSingle)
                yield return new WaitUntil(() => GameManager.Instance.characterOwner.Count == 2);

            // �ڶ��� �� �ƴ��� �Ǵ��ؼ� ĳ���� �������ָ� ��
            // ���� �ڽ��� ���������� �ƴ����� � ĳ���͸� �����ߴ����� �������.
            GameManager.Instance.SetRandomSeed();
            if(isSingle)
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
                {
                    if (isNella)
                    {
                        PhotonNetwork.Instantiate("Prefabs/YC/MainCamera_Nella", new Vector3(0, 0, 0), Quaternion.identity, 0);
                        PhotonNetwork.Instantiate("Prefabs/JJS/NellaMousePoint", new Vector3(-10, 0, -5), Quaternion.identity);
                        
                        GameManager.Instance.characterOwner.Add(true, true);
                        GameManager.Instance.characterOwner.Add(false, false);
                        GameManager.Instance.isAlive.Add(true, true);
                        GameManager.Instance.isAlive.Add(false, true);
                        


                        PhotonNetwork.Instantiate(nellaPrefabDirectory, startPos + intervalPos, Quaternion.identity);
                    }
                    else
                    {
                        PhotonNetwork.Instantiate("Prefabs/YC/MainCamera_Steady", new Vector3(0, 0, 0), Quaternion.identity, 0);
                        PhotonNetwork.Instantiate("Prefabs/JJS/SteadyMousePoint", new Vector3(10, 0, -5), Quaternion.identity);
                        
                        
                        GameManager.Instance.characterOwner.Add(true, false);
                        GameManager.Instance.characterOwner.Add(false, true);
                        GameManager.Instance.isAlive.Add(true, true);
                        GameManager.Instance.isAlive.Add(false, true);
                        

                        PhotonNetwork.Instantiate(steadyPrefabDirectory, startPos - intervalPos, Quaternion.identity);
                    }

                }
                else if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                {
                    if (isNella)
                    {
                        PhotonNetwork.Instantiate("Prefabs/YC/MainCamera_Steady", new Vector3(0, 0, 0), Quaternion.identity, 0);
                        PhotonNetwork.Instantiate("Prefabs/JJS/SteadyMousePoint", new Vector3(10, 0, -5), Quaternion.identity);                      
                        
                        GameManager.Instance.characterOwner.Add(false, false);
                        GameManager.Instance.characterOwner.Add(true, true);
                        GameManager.Instance.isAlive.Add(true, true);
                        GameManager.Instance.isAlive.Add(false, true);                        

                        PhotonNetwork.Instantiate(steadyPrefabDirectory, startPos - intervalPos, Quaternion.identity);
                    }
                    else
                    {
                        PhotonNetwork.Instantiate("Prefabs/YC/MainCamera_Nella", new Vector3(0, 0, 0), Quaternion.identity, 0);
                        PhotonNetwork.Instantiate("Prefabs/JJS/NellaMousePoint", new Vector3(-10, 0, -5), Quaternion.identity);                        
                        GameManager.Instance.characterOwner.Add(false, true);
                        GameManager.Instance.characterOwner.Add(true, false);
                        GameManager.Instance.isAlive.Add(true, true);
                        GameManager.Instance.isAlive.Add(false, true);                        

                        PhotonNetwork.Instantiate(nellaPrefabDirectory, startPos + intervalPos, Quaternion.identity);
                    }
                }
            }
            else
            {
                if(GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient])
                {
                    PhotonNetwork.Instantiate("Prefabs/YC/MainCamera_Nella", new Vector3(0, 0, 0), Quaternion.identity, 0);
                    PhotonNetwork.Instantiate("Prefabs/JJS/NellaMousePoint", new Vector3(-10, 0, -5), Quaternion.identity);

                    PhotonNetwork.Instantiate(nellaPrefabDirectory, startPos + intervalPos, Quaternion.identity);
                    Debug.Log("�ڶ� ����");
                }
                else
                {
                    PhotonNetwork.Instantiate("Prefabs/YC/MainCamera_Steady", new Vector3(0, 0, 0), Quaternion.identity, 0);
                    PhotonNetwork.Instantiate("Prefabs/JJS/SteadyMousePoint", new Vector3(10, 0, -5), Quaternion.identity);

                    PhotonNetwork.Instantiate(steadyPrefabDirectory, startPos - intervalPos, Quaternion.identity);
                    Debug.Log("���׵� ����");
                }
            }
            
            //StopAllCoroutines();
            yield break;
        }
    }

}