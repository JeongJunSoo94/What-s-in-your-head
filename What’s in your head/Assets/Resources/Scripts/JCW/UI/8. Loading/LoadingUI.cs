using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using JCW.Network;
using JCW.AudioCtrl;
using UnityEngine.SceneManagement;

namespace JCW.UI
{
    [RequireComponent(typeof(PhotonView))]
    public class LoadingUI : MonoBehaviour
    {
        [Header("TIP ���")][SerializeField] List<string> tipList = new();
        [Header("�������� �� �̹��� ���")][SerializeField] List<Sprite> bgList = new();

        PhotonView photonView;

        bool isLoading = false;
        Image image;
        Text text;

        public static LoadingUI Instance = null;
        Image bgImg;
        bool isMainTitle = false;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            photonView = PhotonView.Get(this);
            bgImg = transform.GetChild(0).GetComponent<Image>();
            image = transform.GetChild(1).GetComponent<Image>();
            text = transform.GetChild(2).GetComponent<Text>();
        }
        
        private void OnEnable()
        {
            isLoading = false;
            isMainTitle = SceneManager.GetActiveScene().name == "MainTitle";
            Debug.Log("���� �� �̸� : " + SceneManager.GetActiveScene().name);
            PhotonNetwork.LevelLoadingProgress = 0f;
            if (PhotonNetwork.IsMasterClient)
                StartCoroutine(nameof(WaitForStable));
                       
            var random = new System.Random(Guid.NewGuid().GetHashCode());
            text.text = "TIP : " + tipList[random.Next(0, tipList.Count)];
            bgImg.sprite = bgList[GameManager.Instance.curStageIndex];
            SoundManager.Instance.StopBGM();
        }

        void Update()
        {
            Debug.Log("�ε� UI ���� �� ������ : " +(float)PhotonNetwork.LevelLoadingProgress*100f);
            if (!isLoading)
                return;

            image.fillAmount = PhotonNetwork.LevelLoadingProgress;
            if (image.fillAmount >= 0.9f)
            {
                image.fillAmount = 0f;
                // ���Ƿ� �������
                PhotonNetwork.LevelLoadingProgress = 0f;
                isLoading = false;
                Debug.Log("�� �ҷ����� �Ϸ�");
                //if (isMainTitle && PhotonNetwork.IsMasterClient)
                //{
                //    Debug.Log("ĳ���� ������ֱ�");
                //    PhotonManager.Instance.MakeCharacter();
                //}
                this.gameObject.SetActive(false);
            }
        }
        [PunRPC]
        void StartLoading()
        {
            Debug.Log("�ε� ���� ����");
            isLoading = true;
            if (!isMainTitle)
            {
                Debug.Log("���� �޴��� �ƴϹǷ� �����ֱ�");
                GameManager.Instance.curStageType = GameManager.Instance.curStageType >= 3 ? 0 : GameManager.Instance.curStageType + 1;
                GameManager.Instance.curStageIndex = GameManager.Instance.curStageType == 0 ? GameManager.Instance.curStageIndex + 1 : GameManager.Instance.curStageIndex;
                Debug.Log("S" + GameManager.Instance.curStageIndex + "S" + GameManager.Instance.curStageType);
            }
            if (PhotonNetwork.IsMasterClient)
            {
                int sceneNum = 4 * (GameManager.Instance.curStageIndex - 1) + 1 + GameManager.Instance.curStageType;
                Debug.Log("�� �ѹ� : " + sceneNum + " �� ����");
                PhotonNetwork.LoadLevel(sceneNum);
                //StartCoroutine(nameof(Delay));
            }
        }
        
        // �ε��� �����ֱ� ���� ������        
        IEnumerator Delay()
        {
            Debug.Log("���� ������ 1�� �ֱ�");
            yield return new WaitForSeconds(1f);
            // �ѹ� ����� ���ķδ� ���� Wait���� �� Ž ����?
            //PhotonNetwork.LoadLevel(GameManager.Instance.curStageIndex * 2 - 2 + GameManager.Instance.curStageType);            
            int sceneNum = 4 * (GameManager.Instance.curStageIndex - 1) + 1 + GameManager.Instance.curStageType;
            Debug.Log("�� �ѹ� : " + sceneNum +  " �� ����");
            PhotonNetwork.LoadLevel(sceneNum);
            yield break;
        }

        IEnumerator WaitForStable()
        {
            while(PhotonNetwork.LevelLoadingProgress > 0f)
            {
                Debug.Log("���� ���� ���� �ҷ������ִ� ���¶� ��� : " + PhotonNetwork.LevelLoadingProgress);
                yield return null;
            }
            photonView.RPC(nameof(StartLoading), RpcTarget.AllViaServer);
        }
    }
}

