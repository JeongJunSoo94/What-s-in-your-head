using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyUI : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject WelcomeText;
    [SerializeField] private GameObject CurrentNumberObj;
    [SerializeField] private GameObject TotalNumberObj;
    [SerializeField] private GameObject RoomNameObj;
    [SerializeField] private GameObject WarningObj;
    [SerializeField] private GameObject[] RoomList;

    private Text currentNumber;
    private Text totalNumber;

    private List<RoomInfo> curRoomList = new List<RoomInfo>();


    //�̱���
    public static LobbyUI instance = null;

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

    }

    void Start()
    {
        // �г��� ȯ���մϴ� !
        WelcomeText.GetComponent<Text>().text = PhotonNetwork.LocalPlayer.NickName + WelcomeText.GetComponent<Text>().text;

        Debug.Log(PhotonNetwork.CountOfPlayersOnMaster);
        currentNumber = CurrentNumberObj.GetComponent<Text>();
        totalNumber = TotalNumberObj.GetComponent<Text>();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        bool haveIt = false;
        foreach(var room in roomList)
        {
            // ����� ���̸�
            if(room.RemovedFromList == true)
            {
                foreach(RoomInfo ri in curRoomList)
                {
                    if(ri.Name == room.Name)
                    {
                        curRoomList.Remove(ri);
                        break;
                    }
                }
            }
            else
            {
                foreach (RoomInfo ri in curRoomList)
                {
                    if (ri.Name == room.Name)
                    {
                        haveIt = true;
                        break;
                    }
                }
                // ������ ���� ��� �߰�
                if(!haveIt)
                {
                    curRoomList.Add(room);
                }
            }
        }
        Debug.Log("���� ������� �� ���� : " + curRoomList.Count);        
    }


    private void Update()
    {
        currentNumber.text = "�κ� ������ : " + (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms).ToString()
                             + "��";
        totalNumber.text = "�� ���� ������ : " + PhotonNetwork.CountOfPlayers + "��";

        for (int i=0 ; i< curRoomList.Count ; ++i)
        {
            if(curRoomList[i].PlayerCount == curRoomList[i].MaxPlayers)
                RoomList[i].GetComponent<Button>().interactable = false;
            else
                RoomList[i].GetComponent<Button>().interactable = true;
            RoomList[i].transform.GetChild(0).GetComponent<Text>().text = curRoomList[i].Name;
            RoomList[i].transform.GetChild(1).GetComponent<Text>().text = curRoomList[i].PlayerCount.ToString() + "/" + curRoomList[i].MaxPlayers.ToString();
        }
    }
    public void MakeRoom()
    {
        if (WarningObj.activeSelf)
            return;
        // �� ����
        for (int i = 0 ; i<curRoomList.Count ; ++i)
        {
            if (RoomNameObj.GetComponent<Text>().text == curRoomList[i].Name)
            {
                WarningObj.SetActive(true);
                WarningObj.transform.GetChild(1).gameObject.GetComponent<Text>().text = "���� �����ϴ� �� �̸��Դϴ� !";
                return;
            }
        }

        this.gameObject.SetActive(false);

        // ���� �Ӽ� ����
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;    // �ִ� �����ڼ�, ���� ����� 20CCU�̹Ƿ� 20 �ʰ��δ� ���Ѵ�.
        roomOptions.IsOpen = true;      // ���� ���� ����
        roomOptions.IsVisible = true;   // �κ񿡼� �� ��Ͽ� �����ų�� ����

        PhotonNetwork.CreateRoom(RoomNameObj.GetComponent<Text>().text, roomOptions);
    }

    public void FindRoom()
    {
        if (WarningObj.activeSelf)
            return;
        // �� ����
        for (int i = 0 ; i<curRoomList.Count ; ++i)
        {
            if (RoomNameObj.GetComponent<Text>().text == curRoomList[i].Name)
            {
                this.gameObject.SetActive(false);
                PhotonNetwork.JoinRoom(RoomNameObj.GetComponent<Text>().text);
                return;
            }
        }

        WarningObj.SetActive(true);
        WarningObj.transform.GetChild(1).gameObject.GetComponent<Text>().text = "�ش� �̸��� ������ �����ϴ� !";
    }

    public void DisableWarning()
    {
        WarningObj.SetActive(false);
    }

    public void JoinSelectRoom(int num)
    {
        this.gameObject.SetActive(false);
        PhotonNetwork.JoinRoom(RoomList[num].transform.GetChild(0).GetComponent<Text>().text);
    }
}
