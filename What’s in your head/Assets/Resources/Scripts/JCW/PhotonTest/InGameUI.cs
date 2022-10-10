using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using JCW.Options.InputBindings;

[RequireComponent(typeof(PhotonView))]
public class InGameUI : MonoBehaviourPunCallbacks
{
    [SerializeField][Tooltip("채팅 UI")] private GameObject ChatUI;
    [SerializeField][Tooltip("이전 채팅 내용")] private GameObject previousChat;
    [SerializeField][Tooltip("닉네임")] private GameObject nickName;
    [SerializeField][Tooltip("현재 작성 중인 채팅 내용")] private InputField Chat;

    private ScrollRect prev_ScrollRect = null;

    void Start()
    {
        Application.targetFrameRate = 60;
        prev_ScrollRect = ChatUI.transform.GetChild(0).GetComponent<ScrollRect>();
        nickName.GetComponent<Text>().text = PhotonNetwork.LocalPlayer.NickName;
        PhotonNetwork.IsMessageQueueRunning = true;
    }


    void Update()
    {
        if (ITT_KeyManager.Instance.GetKeyDown(PlayerAction.Chat))
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
