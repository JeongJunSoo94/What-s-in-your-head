using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using JCW.Network;
using Photon.Realtime;
using System.IO;
using JCW.UI.InGame;

namespace JCW.UI
{
    public class ButtonEnter : MonoBehaviour
    {
        [Header("�÷��̾� ��ư")][SerializeField]         Button player1_Button = null;
                                [SerializeField]        Button player2_Button = null;
        [Header("�ڷΰ��� ��ư")][SerializeField]         Button backButton = null;        
        [Header("�⺻ ��ư ��������Ʈ")][SerializeField]  Sprite defaultSprite1 = null;
                                     [SerializeField]  Sprite defaultSprite2 = null;
        [Header("�Ϸ� ��ư ��������Ʈ")][SerializeField]  Sprite readySprite1 = null;        
                                     [SerializeField]  Sprite readySprite2 = null;
        [Header("ĳ���� ���� UI")] [SerializeField]      GameObject charSelectUI = null;
        [Header("�Ѿ�� ��ư")] [SerializeField]        Button moveOnButton = null;
        [Header("�ε� UI")] [SerializeField]              CutScene openingUI;

         bool isReady = false;

         Image player1_Img;
         Image player2_Img;

         PhotonView photonView;

        [HideInInspector] public bool isNewGame = true;

         void Awake()
        {
            player1_Img = player1_Button.gameObject.GetComponent<Image>();
            player2_Img = player2_Button.gameObject.GetComponent<Image>();
            photonView = gameObject.GetComponent<PhotonView>();
            GameManager.Instance.curStageIndex = 1;
            GameManager.Instance.curStageType = 1;
            GameManager.Instance.curSection = 0;

            backButton.onClick.AddListener(() =>
            {
                photonView.RPC(nameof(CancleEnter), RpcTarget.AllViaServer);
                photonView.RPC(nameof(Leave), RpcTarget.AllViaServer);
            });

            player1_Button.onClick.AddListener(() =>
            {
                if(PhotonNetwork.IsMasterClient)
                    photonView.RPC(nameof(SetEnter), RpcTarget.AllViaServer, true, false);
            });
            player2_Button.onClick.AddListener(() =>
            {
                if(!PhotonNetwork.IsMasterClient)
                    photonView.RPC(nameof(SetEnter), RpcTarget.AllViaServer, false, false);
            });

            moveOnButton.onClick.AddListener(() =>
            {
                photonView.RPC(nameof(MoveOn), RpcTarget.AllViaServer);                
            });
        }
         void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                photonView.RPC(nameof(SetEnter), RpcTarget.AllViaServer, PhotonNetwork.IsMasterClient, false);
            }
        }

        [PunRPC]
         void SetEnter(bool isMaster, bool leave = false)
        {
            isReady = !isReady;
            if (leave && isReady)
                isReady = !isReady;

            if (isMaster)
                player1_Img.sprite = isReady ? readySprite1 : defaultSprite1;
            else
                player2_Img.sprite = isReady ? readySprite2 : defaultSprite2;

            if (player1_Img.sprite == readySprite1
                && player2_Img.sprite == readySprite2)
                moveOnButton.gameObject.SetActive(true);
            else
                moveOnButton.gameObject.SetActive(false);

        }
        [PunRPC]
         void CancleEnter()
        {
            isReady = false;
            player1_Img.sprite = defaultSprite1;
            player2_Img.sprite = defaultSprite2;
        }

        [PunRPC]
         void MoveOn()
        {
            CancleEnter();

            moveOnButton.gameObject.SetActive(false);
            if (isNewGame)
            {
                openingUI.gameObject.SetActive(true);
                StartCoroutine(nameof(WaitForCutSceneEnd));
            }
            else
                charSelectUI.SetActive(true);
        }

        [PunRPC]
         void Leave()
        {
            PhotonNetwork.LeaveRoom();
            StartCoroutine(nameof(WaitForRoom));
            
        }

        IEnumerator WaitForRoom()
        {
            yield return new WaitUntil(() => PhotonNetwork.NetworkClientState.ToString() == ClientState.JoinedLobby.ToString());
            RoomOptions lobbyOptions = new()
            {
                MaxPlayers = 20,    // �ִ� �����ڼ�, ���� ����� 20CCU�̹Ƿ� 20 �ʰ��δ� ���Ѵ�.
                IsOpen = true,      // ���� ���� ����
                IsVisible = false,   // �κ񿡼� �� ��Ͽ� �����ų�� ����  
            };
            PhotonNetwork.JoinOrCreateRoom("Lobby", lobbyOptions, null);
            this.gameObject.SetActive(false);
            yield break;
        }

        IEnumerator WaitForCutSceneEnd()
        {
            yield return new WaitUntil(() => openingUI.gameObject.activeSelf == false);
            charSelectUI.SetActive(true);
            yield break;
        }

        // �������� ����
        #region 
        public void LoadLatestStage()
        {
            string path = Application.dataPath + "/Resources/CheckPointInfo/Stage";
            bool isTextCheck = false;
            for (int i=3 ; i>=1 ; --i)
            {
                for (int j=2 ; j>=1 ; --j)
                {
                    if (Directory.Exists(path + i + "/" + j + "/"))
                    {
                        SetStage_RPC(i);
                        SetStageType_RPC(j);
                        SetSection_RPC(Directory.GetFiles(path + i + "/" + j + "/").Length);
                        isTextCheck = true;
                        break;
                    }                
                }
            }            
            if(isTextCheck)
            {
                Text tempText = transform.GetChild(2).GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>();
                switch(GameManager.Instance.curStageIndex)
                {
                    case 0:
                    case 1:
                        tempText.text = "�׸� ��ũ";
                        break;
                    case 2:
                        tempText.text = "����ȸ";
                        break;
                    case 3:
                        tempText.text = "�Ĺ���";
                        break;
                }
            }
            else
            {
                GameManager.Instance.curStageIndex = 1;
            }
            isNewGame = false;
        }

        public void SetNewGame_RPC()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            photonView.RPC(nameof(SetNewGame), RpcTarget.AllViaServer);
        }

        [PunRPC]
        void SetNewGame()
        {
            isNewGame = true;
            GameManager.Instance.curStageIndex = 1;
            GameManager.Instance.curStageType = 1;
            GameManager.Instance.curSection = 0;
        }


        public void SetStage_RPC(int stageCount)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            photonView.RPC(nameof(SetStage), RpcTarget.AllViaServer, stageCount);
        }

        [PunRPC]
        void SetStage(int stageCount)
        {
            GameManager.Instance.curStageIndex = stageCount;
        }

        public void SetStageType_RPC(int stageType)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            photonView.RPC(nameof(SetStageType), RpcTarget.AllViaServer, stageType);
        }

        [PunRPC]
        void SetStageType(int stageType)
        {
            GameManager.Instance.curStageType = stageType;
        }

        public void SetSection_RPC(int sectionCount)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            photonView.RPC(nameof(SetSection), RpcTarget.AllViaServer, sectionCount);
        }

        [PunRPC]
        void SetSection(int sectionCount)
        {
            GameManager.Instance.curSection = sectionCount;
        }
        #endregion 
    }
}
