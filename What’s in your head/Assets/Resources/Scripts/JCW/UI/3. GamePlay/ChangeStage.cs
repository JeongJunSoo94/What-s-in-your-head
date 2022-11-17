using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


namespace JCW.UI
{
    [RequireComponent(typeof(PhotonView))]
    public class ChangeStage : MonoBehaviour
    {
        [Header("Ready UI")] [SerializeField] ButtonEnter readyUI;
        [Header("좌측 버튼")] [SerializeField] Button leftButton = null; 
        [Header("우측 버튼")] [SerializeField] Button rightButton = null;
        [Space(10f)]
        [Header("0번째는 스테이지 전체 제목")]
        [Header("스테이지 1 제목 리스트")] [SerializeField] List<string> stage1 = new();
        [Header("스테이지 2 제목 리스트")] [SerializeField] List<string> stage2 = new();
        [Header("스테이지 3 제목 리스트")] [SerializeField] List<string> stage3 = new();
        [Space(10f)]
        [Header("현재 스테이지 제목")] [SerializeField] Text curStage;
        [Header("섹션 제목 리스트")] [SerializeField] GameObject sections;
        [Space(10f)]
        [Header("가져올 스테이지 배경 리스트")] [SerializeField] List<Sprite> bgList;

        // 불러올 수 있는 스테이지 목록
        int stageCount = 0;
        int latestStageType = 0;
        PhotonView pv;
        readonly Dictionary<int, List<string>> stageDict = new();

        Image backgroundImg;

        string path;

        private void Awake()
        {
            pv = PhotonView.Get(this);
            stageDict.Add(1, stage1);
            stageDict.Add(2, stage2);
            stageDict.Add(3, stage3);
            backgroundImg = transform.GetChild(1).GetComponent<Image>();

            if (!pv.IsMine)
                return;
            path = Application.dataPath + "/Resources/CheckPointInfo/";
            if (Directory.Exists(path))
                stageCount = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly).Length;
            else
            {
                leftButton.interactable = false;
                rightButton.interactable = false;
            }

            leftButton.onClick.AddListener(() =>
            {
                if (PhotonNetwork.IsMasterClient)
                    pv.RPC(nameof(ClickButton), RpcTarget.AllViaServer, true);

            });
            rightButton.onClick.AddListener(() =>
            {
                if (PhotonNetwork.IsMasterClient)
                    pv.RPC(nameof(ClickButton), RpcTarget.AllViaServer, false);
            });
        }

        private void OnEnable()
        {
            readyUI.isNewGame = false;
            if (PhotonNetwork.IsMasterClient)
                pv.RPC(nameof(InitSet), RpcTarget.AllViaServer);
        }

        [PunRPC]
        void InitSet()
        {
            GameManager.Instance.curStageIndex = 1;
            GameManager.Instance.curStageType = -1;
            GameManager.Instance.curSection = 0;
            backgroundImg.sprite = bgList[GameManager.Instance.curStageIndex];
            curStage.text = stageDict[GameManager.Instance.curStageIndex][0];
            sections.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = stageDict[GameManager.Instance.curStageIndex][1];
            sections.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = stageDict[GameManager.Instance.curStageIndex][2];

            leftButton.interactable = GameManager.Instance.curStageIndex != 1;
            rightButton.interactable = GameManager.Instance.curStageIndex != stageCount;
            if(PhotonNetwork.IsMasterClient)
                latestStageType = Directory.GetDirectories(path + "/Stage" + GameManager.Instance.curStageIndex, "*", SearchOption.TopDirectoryOnly).Length;
            sections.transform.GetChild(1).GetComponent<Button>().interactable = latestStageType == 2;
        }

        [PunRPC]
        void ClickButton(bool isLeft)
        {
            if (isLeft)
            {
                if (GameManager.Instance.curStageIndex > 1)
                {
                    curStage.text = stageDict[--GameManager.Instance.curStageIndex][0];
                    backgroundImg.sprite = bgList[GameManager.Instance.curStageIndex];
                    sections.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = stageDict[GameManager.Instance.curStageIndex][1];
                    sections.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = stageDict[GameManager.Instance.curStageIndex][2];
                    rightButton.interactable = true;
                    latestStageType = Directory.GetDirectories(path + "/Stage" + GameManager.Instance.curStageIndex, "*", SearchOption.TopDirectoryOnly).Length;
                }
                leftButton.interactable = GameManager.Instance.curStageIndex != 1;
            }
            else
            {
                if (GameManager.Instance.curStageIndex < stageCount)
                {
                    curStage.text = stageDict[++GameManager.Instance.curStageIndex][0];
                    backgroundImg.sprite = bgList[GameManager.Instance.curStageIndex];
                    sections.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = stageDict[GameManager.Instance.curStageIndex][1];
                    sections.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = stageDict[GameManager.Instance.curStageIndex][2];
                    leftButton.interactable = true;
                    latestStageType = Directory.GetDirectories(path + "/Stage" + GameManager.Instance.curStageIndex, "*", SearchOption.TopDirectoryOnly).Length;
                }
                rightButton.interactable = GameManager.Instance.curStageIndex != stageCount;
            }
            sections.transform.GetChild(1).GetComponent<Button>().interactable = latestStageType == 2;
            GameManager.Instance.curStageType = -1;
        }
    }
}
