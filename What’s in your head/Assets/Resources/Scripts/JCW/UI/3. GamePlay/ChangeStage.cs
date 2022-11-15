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
        [Header("좌측 버튼")] [SerializeField] private Button leftButton = null;
        [Header("우측 버튼")] [SerializeField] private Button rightButton = null;
        [Header("스테이지 제목 리스트")] [SerializeField] private List<string> stages = new();
        [Header("현재 스테이지 제목")] [SerializeField] private Text curStage;
        [Header("섹션 제목 리스트")] [SerializeField] private GameObject sections;
        [Header("ReadyUI")] [SerializeField] ButtonEnter readyUI;

        // 불러올 수 있는 스테이지 목록
        int stageCount = 0;
        int latestStageType = 0;
        PhotonView pv;

        private void Awake()
        {            
            pv = PhotonView.Get(this);
            if (!pv.IsMine)
                return;            

            leftButton.onClick.AddListener(() =>
            {
                pv.RPC(nameof(ClickButton), RpcTarget.AllViaServer, true);

            });
            rightButton.onClick.AddListener(() =>
            {
                pv.RPC(nameof(ClickButton), RpcTarget.AllViaServer, false);
            });           
        }

        private void OnEnable()
        {
            readyUI.isNewGame = false;
            GameManager.Instance.curStageIndex = 1;
            GameManager.Instance.curStageType = 1;
            GameManager.Instance.curSection = 0;
            string path = Application.dataPath + "/Resources/CheckPointInfo/";
            if (Directory.Exists(path))
            {
                stageCount = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly).Length;
                latestStageType = Directory.GetDirectories(path + "/Stage/" + stageCount, "*", SearchOption.TopDirectoryOnly).Length;
            }
            else
            {
                leftButton.interactable = false;
                rightButton.interactable = false;
            }
        }

        [PunRPC]
        void ClickButton(bool isLeft)
        {
            if (isLeft)
            {
                if (GameManager.Instance.curStageIndex != 0)
                {
                    curStage.text = stages[--GameManager.Instance.curStageIndex];
                    for (int i = 0 ; i < 2 ; ++i)
                    {
                        sections.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = curStage.text + " " + (i + 1).ToString();
                    }
                    rightButton.interactable = true;
                }
                if (GameManager.Instance.curStageIndex == 0)
                    leftButton.interactable = false;
            }
            else
            {
                if (GameManager.Instance.curStageIndex < stageCount)
                {
                    curStage.text = stages[++GameManager.Instance.curStageIndex];
                    for (int i = 0 ; i < 2 ; ++i)
                    {
                        sections.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = curStage.text + " " + (i + 1).ToString();
                    }
                    leftButton.interactable = true;
                }
                if (GameManager.Instance.curStageIndex == stageCount)
                {
                    rightButton.interactable = false;
                    sections.transform.GetChild(1).GetComponent<Button>().interactable = latestStageType == 2;
                }
            }
        }
    }
}
