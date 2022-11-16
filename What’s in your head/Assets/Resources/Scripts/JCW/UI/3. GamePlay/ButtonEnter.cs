using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using JCW.Network;
using Photon.Realtime;
using System.IO;
using JCW.UI.InGame;
using JCW.AudioCtrl;

namespace JCW.UI
{
    public class ButtonEnter : MonoBehaviour
    {
        [Header("플레이어 버튼")] [SerializeField] private Button player1_Button = null;
        [SerializeField] private Button player2_Button = null;
        [Header("뒤로가기 버튼")] [SerializeField] private Button backButton = null;
        [Header("기본 버튼 스프라이트")] [SerializeField] private Sprite defaultSprite1 = null;
        [SerializeField] private Sprite defaultSprite2 = null;
        [Header("완료 버튼 스프라이트")] [SerializeField] private Sprite readySprite1 = null;
        [SerializeField] private Sprite readySprite2 = null;
        [Header("캐릭터 선택 UI")] [SerializeField] private GameObject charSelectUI = null;
        [Header("넘어가기 버튼")] [SerializeField] private Button moveOnButton = null;
        [Space(10f)]
        [Header("가져올 스테이지 배경 리스트")] [SerializeField] List<Sprite> bgList;
        [Header("컷씬 오브젝트")] [SerializeField] CutScene cutSceneUI;


        private bool isReady = false;

        private Image player1_Img;
        private Image player2_Img;

        private PhotonView photonView;
        Transform continueContentsTF;

        [HideInInspector] public bool isNewGame = true;

        private void Awake()
        {
            player1_Img = player1_Button.gameObject.GetComponent<Image>();
            player2_Img = player2_Button.gameObject.GetComponent<Image>();

            continueContentsTF = transform.GetChild(2).GetChild(1).GetChild(1);

            photonView = gameObject.GetComponent<PhotonView>();
            GameManager.Instance.curStageIndex = 1;
            GameManager.Instance.curStageType = 0;
            GameManager.Instance.curSection = 0;

            backButton.onClick.AddListener(() =>
            {
                photonView.RPC(nameof(CancleEnter), RpcTarget.AllViaServer);
                photonView.RPC(nameof(Leave), RpcTarget.AllViaServer);
            });

            player1_Button.onClick.AddListener(() =>
            {
                if (PhotonNetwork.IsMasterClient)
                    photonView.RPC(nameof(SetEnter), RpcTarget.AllViaServer, true, false);
            });
            player2_Button.onClick.AddListener(() =>
            {
                if (!PhotonNetwork.IsMasterClient)
                    photonView.RPC(nameof(SetEnter), RpcTarget.AllViaServer, false, false);
            });

            moveOnButton.onClick.AddListener(() =>
            {
                photonView.RPC(nameof(MoveOn), RpcTarget.AllViaServer);
            });
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                photonView.RPC(nameof(SetEnter), RpcTarget.AllViaServer, PhotonNetwork.IsMasterClient, false);
            }
        }

        [PunRPC]
        private void SetEnter(bool isMaster, bool leave = false)
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
        private void CancleEnter()
        {
            isReady = false;
            player1_Img.sprite = defaultSprite1;
            player2_Img.sprite = defaultSprite2;
        }

        [PunRPC]
        private void MoveOn()
        {
            if (GameManager.Instance.curStageType < 0)
                return;
            if (isNewGame)
            {
                SoundManager.Instance.PauseResumeBGM();
                cutSceneUI.gameObject.SetActive(true);
                StartCoroutine(nameof(WaitUntilCutSceneEnd));
            }
            else
            {
                CancleEnter();
                charSelectUI.SetActive(true);
                moveOnButton.gameObject.SetActive(false);
            }            
        }

        IEnumerator WaitUntilCutSceneEnd()
        {
            yield return new WaitUntil(() => !cutSceneUI.gameObject.activeSelf);
            CancleEnter();
            charSelectUI.SetActive(true);
            moveOnButton.gameObject.SetActive(false);
            SoundManager.Instance.PauseResumeBGM();
            yield break;
        }

        [PunRPC]
        private void Leave()
        {
            PhotonNetwork.LeaveRoom();
            StartCoroutine(nameof(WaitForRoom));

        }

        IEnumerator WaitForRoom()
        {
            while (PhotonNetwork.NetworkClientState.ToString() != ClientState.JoinedLobby.ToString())
            {
                yield return new WaitForSeconds(0.05f);
            }
            RoomOptions lobbyOptions = new()
            {
                MaxPlayers = 20,    // 최대 접속자수, 포톤 무료는 20CCU이므로 20 초과로는 못한다.
                IsOpen = true,      // 룸의 오픈 여부
                IsVisible = false,   // 로비에서 룸 목록에 노출시킬지 여부  
            };
            PhotonNetwork.JoinOrCreateRoom("Lobby", lobbyOptions, null);
            this.gameObject.SetActive(false);
            yield break;
        }

        public void SetNewGame()
        {
            isNewGame = true;
            GameManager.Instance.curStageIndex = 1;
            GameManager.Instance.curStageType = 0;
        }

        public void LoadLatestStage()
        {
            isNewGame = false;
            string path = Application.dataPath + "/Resources/CheckPointInfo/Stage";
            bool isTextCheck = false;
            for (int i = 3 ; i >= 1 ; --i)
            {
                for (int j = 2 ; j >= 1 ; --j)
                {
                    if (Directory.Exists(path + i + "/" + j + "/"))
                    {
                        SetStage_RPC(i);
                        SetStageType_RPC(j);
                        isTextCheck = true;
                        break;
                    }
                }
                if (isTextCheck) break;
            }
            if (!isTextCheck)
                GameManager.Instance.curStageIndex = 1;            
                
        }

        [PunRPC]
        void ChangeContinuousImg(int idx)
        {
            switch(idx)
            {
                case 0:
                case 1:                    
                    continueContentsTF.GetChild(0).GetComponent<Text>().text = "테마 파크";
                    continueContentsTF.GetChild(1).GetComponent<Image>().sprite = bgList[1];
                    break;
                case 2:
                    continueContentsTF.GetChild(0).GetComponent<Text>().text = "연주회";
                    continueContentsTF.GetChild(1).GetComponent<Image>().sprite = bgList[2];
                    break;
                case 3:
                    continueContentsTF.GetChild(0).GetComponent<Text>().text = "수목원";
                    continueContentsTF.GetChild(1).GetComponent<Image>().sprite = bgList[3];
                    break;
            }
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
            photonView.RPC(nameof(ChangeContinuousImg), RpcTarget.AllViaServer, GameManager.Instance.curStageIndex);
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
    }
}
