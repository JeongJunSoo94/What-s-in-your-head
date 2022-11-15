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
        [Header("���� ��ư")] [SerializeField] Button leftButton = null;
        [Header("���� ��ư")] [SerializeField] Button rightButton = null;
        [Space(10f)]
        [Header("0��°�� �������� ��ü ����")]
        [Header("�������� 1 ���� ����Ʈ")] [SerializeField] List<string> stage1 = new();
        [Header("�������� 2 ���� ����Ʈ")] [SerializeField] List<string> stage2 = new();
        [Header("�������� 3 ���� ����Ʈ")] [SerializeField] List<string> stage3 = new();
        [Space(10f)]
        [Header("���� �������� ����")] [SerializeField] Text curStage;
        [Header("���� ���� ����Ʈ")] [SerializeField] GameObject sections;
        [Space(10f)]
        [Header("������ �������� ��� ����Ʈ")] [SerializeField] List<Sprite> bgList;        

        // �ҷ��� �� �ִ� �������� ���
        int stageCount = 0;
        int latestStageType = 0;
        PhotonView pv;
        Dictionary<int, List<string>> stageDict = new();

        Image backgroundImg;

        private void Awake()
        {            
            pv = PhotonView.Get(this);
            if (!pv.IsMine)
                return;
            GameManager.Instance.curStageIndex = 1;
            GameManager.Instance.curStageType = -1;
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

            stageDict.Add(1, stage1);
            stageDict.Add(2, stage2);
            stageDict.Add(3, stage3);

            backgroundImg = transform.GetChild(1).GetComponent<Image>();

            leftButton.onClick.AddListener(() =>
            {
                if(PhotonNetwork.IsMasterClient)
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
            if (PhotonNetwork.IsMasterClient)
                pv.RPC(nameof(InitSet), RpcTarget.AllViaServer);
        }

        private void OnDisable()
        {
            // �����ߴ� ���ǹ�ư�� �ʱ�ȭ
        }

        [PunRPC]
        void InitSet()
        {
            backgroundImg.sprite = bgList[GameManager.Instance.curStageIndex];
            curStage.text = stageDict[GameManager.Instance.curStageIndex][0];
            for (int i = 1 ; i < stageDict[GameManager.Instance.curStageIndex].Count ; ++i)
            {
                sections.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = stageDict[GameManager.Instance.curStageIndex][i];
            }
            if (GameManager.Instance.curStageIndex == 1)
                leftButton.interactable = false;
            if (GameManager.Instance.curStageIndex == stageCount)
            {
                rightButton.interactable = false;
                sections.transform.GetChild(1).GetComponent<Button>().interactable = latestStageType == 2;
            }
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
                    for (int i = 1 ; i < stageDict[GameManager.Instance.curStageIndex].Count; ++i)
                    {
                        sections.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = stageDict[GameManager.Instance.curStageIndex][i];
                    }
                    rightButton.interactable = true;
                }
                if (GameManager.Instance.curStageIndex == 1)
                    leftButton.interactable = false;
            }
            else
            {
                if (GameManager.Instance.curStageIndex < stageCount)
                {
                    curStage.text = stageDict[++GameManager.Instance.curStageIndex][0];
                    backgroundImg.sprite = bgList[GameManager.Instance.curStageIndex];
                    for (int i = 1 ; i < stageDict[GameManager.Instance.curStageIndex].Count ; ++i)
                    {
                        sections.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = stageDict[GameManager.Instance.curStageIndex][i];
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
