using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class InGameUI : MonoBehaviourPunCallbacks
{
    [SerializeField][Tooltip("ä�� UI")] private GameObject ChatUI;
    [SerializeField][Tooltip("���� ä�� ����")] private GameObject previousChat;
    [SerializeField][Tooltip("�г���")] private GameObject nickName;
    [SerializeField][Tooltip("���� �ۼ� ���� ä�� ����")] private InputField Chat;

    private ScrollRect prev_ScrollRect = null;

    void Start()
    {
        prev_ScrollRect = ChatUI.transform.GetChild(0).GetComponent<ScrollRect>();
        nickName.GetComponent<Text>().text = PhotonNetwork.LocalPlayer.NickName;
        PhotonNetwork.IsMessageQueueRunning = true;
    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            if (ChatUI.activeSelf && !Chat.isFocused)
                ChatUI.SetActive(false);
            else
            {
                ChatUI.SetActive(true);
            }
        }
        if(ChatUI.activeSelf && Chat.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Return))
                SendChat();
        }        
    }

    public void FocusingChat()
    {
        Chat.ActivateInputField();
        Chat.Select();
    }

    public void SendChat()
    {
        if (Chat.text == "")
            return;
        string msg = "[" + PhotonNetwork.LocalPlayer.NickName + "] : " + Chat.text;
        photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, msg);
        ReceiveMsg(msg);
        Chat.ActivateInputField();
        Chat.text = "";
    }

    [PunRPC]
    public void ReceiveMsg(string msg)
    {
        previousChat.GetComponent<Text>().text += msg + "\n";
        prev_ScrollRect.verticalNormalizedPosition = 0.0f;
    }
}
