using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾ Instantiate �ϱ� ���� �ӽ� ��ũ��Ʈ
/// </summary>

using Photon.Pun;
using Photon.Realtime;
using System.Linq;

using YC.CameraManager_;
using YC.Camera_;
using Cinemachine;



namespace YC.Photon
{
    public class TempPhotonManager : MonoBehaviourPunCallbacks
    {
        // ���� �Է�
        private readonly string version = "1.0f";

        // ����� ���̵� �Է�
        private string userId = "Mary";

        [SerializeField] string roomName;
        [SerializeField] string nellaPrefabDirectory;
        [SerializeField] string steadyPrefabDirectory;
        [SerializeField] public bool isNella;


        static public TempPhotonManager Instance = null;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this.gameObject);
            //Cursor.lockState = CursorLockMode.Locked;
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

            // ���� ����
            PhotonNetwork.ConnectUsingSettings();

        }

        // ���� ������ ���ӽ� ȣ��Ǵ� �ݹ� �Լ�
        public override void OnConnectedToMaster()
        {
            //Debug.Log("Conntected to Master!");

            // �κ� ���� ���θ� �α׷� ��� (���� �κ� ���� ���̴� �翬�� false)
            //Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");

            // �κ� ����
            PhotonNetwork.JoinLobby();
        }

        // �κ� ����� ȣ��Ǵ� �ݹ� �Լ�
        public override void OnJoinedLobby()
        {
            //Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}"); // true;

            // �����Ǿ� �ִ� ��� �߿��� �����ϰ� ���� (���� ��ġ����ŷ ���)
            //PhotonNetwork.JoinRandomRoom();
            
            // ���� �Ӽ� ����
            RoomOptions ro = new RoomOptions();
            ro.MaxPlayers = 20;     // �ִ� ������ �� : 20��
            ro.IsOpen = true;       // ���� ���� ����
            ro.IsVisible = true;    // �κ񿡼� �� ��Ͽ� ���� ����
            PhotonNetwork.JoinOrCreateRoom(roomName, ro, null);
        }

        // ���� ��ġ����ŷ ���н� ȣ��Ǵ� �ݹ� �Լ�
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            //Debug.Log($"JoinRandom Failed {returnCode} : {message}");

            // ���� �Ӽ� ����
            RoomOptions ro = new RoomOptions();
            ro.MaxPlayers = 20;     // �ִ� ������ �� : 20��
            ro.IsOpen = true;       // ���� ���� ����
            ro.IsVisible = true;    // �κ񿡼� �� ��Ͽ� ���� ����

            // �� ����
            PhotonNetwork.CreateRoom(roomName, ro);
        }

        // �� ������ �Ϸ�� ȣ��Ǵ� �ݹ� �Լ�
        public override void OnCreatedRoom()
        {
            //Debug.Log("Created Room");
            //Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
        }

        // �뿡 ����� ȣ��Ǵ� �ݹ� �Լ�
        public override void OnJoinedRoom()
        {
            //Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
            //Debug.Log($"PlayerCount = {PhotonNetwork.CurrentRoom.PlayerCount}");

            // �뿡 ������ ����� ���� Ȯ��
            foreach (var player in PhotonNetwork.CurrentRoom.Players)
            {
                Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}");
            }



            // ���� ��Ʈ��ũ�� ���� PhotonNetwork.Instantiate�� ������Ʈ�� MonoBehaviour. Start()�Լ��� ������� �ʴ´�.
            // ���� OnEnable �� OnDisable �� �ݵ�� ���� �� �־�� �մϴ�.
            // https://doc.photonengine.com/ko-kr/pun/current/gameplay/instantiation


            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {              
                if(isNella)
                {
                    PhotonNetwork.Instantiate("Prefabs/YC/MainCamera_Nella", new Vector3(0, 0, 0), Quaternion.identity, 0);
                    PhotonNetwork.Instantiate("Prefabs/JCW/NellaMousePoint", new Vector3(0, 0, 0), Quaternion.identity, 0);
                    //PhotonNetwork.Instantiate(nellaPrefabDirectory, new Vector3(-262.4f, -2.2f, 267.9f), Quaternion.identity, 0);
                    PhotonNetwork.Instantiate(nellaPrefabDirectory, new Vector3(7f, 27f, 12.5f), Quaternion.identity, 0);
                    //PhotonNetwork.Instantiate(nellaPrefabDirectory, new Vector3(5, 0, 0), Quaternion.identity, 0);
                    GameManager.Instance.characterOwner.Add(true, true);
                    GameManager.Instance.characterOwner.Add(false, false);
                    GameManager.Instance.isAlive.Add(true, true);
                    GameManager.Instance.isAlive.Add(false, true);
                }
                else
                {
                    PhotonNetwork.Instantiate("Prefabs/YC/MainCamera_Steady", new Vector3(0, 0, 0), Quaternion.identity, 0);
                    PhotonNetwork.Instantiate("Prefabs/JCW/SteadyMousePoint", new Vector3(0, 0, 0), Quaternion.identity, 0);
                    //PhotonNetwork.Instantiate(steadyPrefabDirectory, new Vector3(-262.4f, -2.2f, 267.9f), Quaternion.identity, 0);
                    PhotonNetwork.Instantiate(steadyPrefabDirectory, new Vector3(7f, 27f, 12.5f), Quaternion.identity, 0);
                    //PhotonNetwork.Instantiate(steadyPrefabDirectory, new Vector3(-5, 0, 0), Quaternion.identity, 0);
                    GameManager.Instance.characterOwner.Add(true, false);
                    GameManager.Instance.characterOwner.Add(false, true);
                    GameManager.Instance.isAlive.Add(true, true);
                    GameManager.Instance.isAlive.Add(false, true);
                }
                
                //CameraManager.Instance.cameras[0] = GameObject.FindGameObjectWithTag("NellaCamera").GetComponent<Camera>();

                //CameraManager.Instance.cameras[0] = 
                //    PhotonNetwork.Instantiate("Prefabs/YC/MainCamera_Nella", new Vector3(0, 0, 0), Quaternion.identity, 0).GetComponent<Camera>();

                //Inst.GetComponent<CameraController>().mainCam = CameraManager.Instance.cameras[0];
                //Inst.GetComponent<CameraController>().cinemachineBrain = CameraManager.Instance.cameras[0].GetComponent<CinemachineBrain>();

            }
            else if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                if (isNella)
                {
                    PhotonNetwork.Instantiate("Prefabs/YC/MainCamera_Steady", new Vector3(-262.4f, -2.2f, 267.9f), Quaternion.identity, 0);
                    PhotonNetwork.Instantiate("Prefabs/JCW/SteadyMousePoint", new Vector3(0, 0, 0), Quaternion.identity, 0);
                    //PhotonNetwork.Instantiate(steadyPrefabDirectory, new Vector3(-262.4f, -2.2f, 267.9f), Quaternion.identity, 0);
                    PhotonNetwork.Instantiate(steadyPrefabDirectory, new Vector3(7f, 27f, 12.5f), Quaternion.identity, 0);
                    //PhotonNetwork.Instantiate(steadyPrefabDirectory, new Vector3(-5, 0, 0), Quaternion.identity, 0);
                    GameManager.Instance.characterOwner.Add(false, false);
                    GameManager.Instance.characterOwner.Add(true, true);
                    GameManager.Instance.isAlive.Add(true, true);
                    GameManager.Instance.isAlive.Add(false, true);
                }
                else
                {
                    PhotonNetwork.Instantiate("Prefabs/YC/MainCamera_Nella", new Vector3(0, 0, 0), Quaternion.identity, 0);
                    PhotonNetwork.Instantiate("Prefabs/JCW/NellaMousePoint", new Vector3(0, 0, 0), Quaternion.identity, 0);
                    //PhotonNetwork.Instantiate(nellaPrefabDirectory, new Vector3(-262.4f, -2.2f, 267.9f), Quaternion.identity, 0);
                    PhotonNetwork.Instantiate(nellaPrefabDirectory, new Vector3(7f, 27f, 12.5f), Quaternion.identity, 0);
                    //PhotonNetwork.Instantiate(nellaPrefabDirectory, new Vector3(5, 0, 0), Quaternion.identity, 0);
                    GameManager.Instance.characterOwner.Add(true, false);
                    GameManager.Instance.characterOwner.Add(false, true);
                    GameManager.Instance.isAlive.Add(true, true);
                    GameManager.Instance.isAlive.Add(false, true);
                }           
            }
        }
    }
}
