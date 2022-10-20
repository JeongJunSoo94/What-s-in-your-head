using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using JCW.UI.Options;

namespace JCW.UI
{
    public class MenuUI : MonoBehaviour
    {
        private enum MenuSelect { Play, Option, Exit};
        

        [Header("�ΰ� ���� �ð�")][SerializeField] private float startTime = 1.0f;
        [Header("�ΰ� �۵� ���� �ð�")][SerializeField] private float appearingTime = 0.5f;
        [Header("������ ��Ÿ�� �ΰ�")][SerializeField] private Image titleLogo;
        [Header("������ ��Ÿ�� ��ư��")][SerializeField] private List<Button> buttonList;
        [Header("�÷��� ��ư �� ���� UI")][SerializeField] private GameObject playObj;
        [Header("�ɼ� ��ư �� ���� UI")][SerializeField] private GameObject optionObj;

        private readonly List<Text> buttonTexts = new();

        Color transparentColor = new(1,1,1,0);
        
        private void Awake()
        {
            titleLogo = transform.GetChild(2).gameObject.GetComponent<Image>();
            for (int i = 0 ; i<buttonList.Count ; ++i)
            {
                buttonTexts.Add(buttonList[i].transform.GetChild(0).GetComponent<Text>());
            }            
        }
        private void Start()
        {
            StartCoroutine(nameof(Appear));
        }        

        IEnumerator Appear()
        {
            // ������ ��Ÿ���Բ�
            yield return new WaitForSeconds(startTime);
            for (int i = 1 ; i<=50 ; ++i)
            {
                yield return new WaitForSeconds(appearingTime/50f);
                transparentColor.a = 0.02f * i;
                titleLogo.color = transparentColor;
                for (int j = 0 ; j<buttonTexts.Count ; ++j)
                    buttonTexts[j].color = transparentColor;
            }

            // ��ư �۵� �ѱ�
            for (int k = 0 ; k<buttonList.Count ; ++k)
            {
                buttonList[k].interactable = true;
                buttonList[k].gameObject.transform.GetChild(0).gameObject.GetComponent<FontColorShift>().enabled = true;
            }

            buttonList[(int)MenuSelect.Play].onClick.AddListener(() => 
            {                 
                RoomOptions lobbyOptions = new()
                {
                    MaxPlayers = 20,    // �ִ� �����ڼ�, ���� ����� 20CCU�̹Ƿ� 20 �ʰ��δ� ���Ѵ�.
                    IsOpen = true,      // ���� ���� ����
                    IsVisible = false,   // �κ񿡼� �� ��Ͽ� �����ų�� ����  
                };
                PhotonNetwork.JoinOrCreateRoom("Lobby", lobbyOptions, null);
                StartCoroutine(nameof(OpenRoom));
            });

            buttonList[(int)MenuSelect.Option].onClick.AddListener(() => { optionObj.SetActive(true); });

            buttonList[(int)MenuSelect.Exit].onClick.AddListener(() =>
            {
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
            });

            yield break;

        }

        IEnumerator OpenRoom()
        {
            while (PhotonNetwork.NetworkClientState != ClientState.Joined)
            {
                yield return null;
            }
            playObj.SetActive(true);
            yield break;
        }
    }
}

