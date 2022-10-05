using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

// MonoBehaviourPun : Callback �Լ� �����͵��� �������̵� �� �� ����
public class PhotonManager : MonoBehaviourPunCallbacks
{
    [SerializeField]    private string SceneMenu;
    [SerializeField]    private string SceneLobby;
    [SerializeField]    private string SceneRoom;
    //�̱���
    public static PhotonManager instance = null;

    //���� �Է�
    private readonly string version = "1.0";

    //����� ���̵� �Է�
    private string userID = "�͸�";

    //���̵� ��ĥ ���� ����
    private string name_number = "";

    [Header("�г��� �Է�")]
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
        // ���� ���� �����鿡�� �ڵ����� ���� �ε�
        PhotonNetwork.AutomaticallySyncScene = true;
        // ���� ������ �������� ���� ���
        PhotonNetwork.GameVersion = version;

        // ���� ������ ��� Ƚ�� ����. �ʴ� 30ȸ
        Debug.Log("�������� �ʴ� ��� Ƚ�� : " + PhotonNetwork.SendRate);

        // ���� ���̵� �Ҵ�
        PhotonNetwork.LocalPlayer.NickName = NicknameInput == null ? userID + name_number : NicknameInput.text;        
        // ���� ����
        PhotonNetwork.ConnectUsingSettings();
    }

    // ���� ������ ���� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnConnectedToMaster()
    {
        Debug.Log("������ ���� �Ϸ� !");
        PhotonNetwork.JoinLobby(); // �κ� ���� 
        PhotonNetwork.LoadLevel(SceneLobby);
    }    

    // �κ� ���� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnJoinedLobby()
    {        
        Debug.Log($"�κ� ���� ���� : {PhotonNetwork.InLobby}");
        //PhotonNetwork.JoinRandomRoom(); // ���� ��ġ����ŷ ��� ����
    }

    // ������ �� ������ �������� ��� ȣ��Ǵ� �ݹ� �Լ�
    /*
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"�� ���ӿ� �����Ͽ� ���� ����ϴ�. {returnCode}:{message}");

        // ���� �Ӽ� ����
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;    // �ִ� �����ڼ�, ���� ����� 20CCU�̹Ƿ� 20 �ʰ��δ� ���Ѵ�.
        roomOptions.IsOpen = true;      // ���� ���� ����
        roomOptions.IsVisible = true;   // �κ񿡼� �� ��Ͽ� �����ų�� ����

        // �� ����
        PhotonNetwork.CreateRoom("My Room", roomOptions);        
    }
    */

    // �� ������ �Ϸ�� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnCreatedRoom()
    {
        Debug.Log("�� ���� �Ϸ�");
        Debug.Log($"�� �̸� : {PhotonNetwork.CurrentRoom.Name}");
    }

    // �뿡 ������ �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(SceneRoom);
        // �ߺ� �г��� ������ ���� �߰� �̸�
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
        string isMaster = PhotonNetwork.LocalPlayer.IsMasterClient ? "(����)" : "";
        Debug.Log(isMaster + PhotonNetwork.LocalPlayer.NickName + "��" + $" �� ���� �Ϸ� : {PhotonNetwork.InRoom}");
        Debug.Log($"�÷��̾� �� : {PhotonNetwork.CurrentRoom.PlayerCount}");

        // �뿡 ������ ����� ���� Ȯ��
        foreach(var player in PhotonNetwork.CurrentRoom.Players)
        {
            // $ => String.Format() : $"" �ֵ���ǥ �ȿ� �ִ� ������ ��Ʈ������ �ٲ���־��.            
            Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}");
        }
        StartCoroutine(nameof(MakeChar));
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"�� ���ӿ� �����߽��ϴ�. {returnCode}:{message}");

        if(SceneManager.GetActiveScene().name == SceneLobby)
        {
            LobbyUI.instance.gameObject.SetActive(true);
            LobbyUI.instance.WarningObj.SetActive(true);
            LobbyUI.instance.WarningObj.transform.GetChild(1).gameObject.GetComponent<Text>().text = "������ �� ���� ���Դϴ� !";

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
