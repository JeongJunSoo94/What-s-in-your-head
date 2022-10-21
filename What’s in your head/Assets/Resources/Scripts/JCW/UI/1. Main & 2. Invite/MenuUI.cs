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
        

        [Header("로고 시작 시간")][SerializeField] private float startTime = 1.0f;
        [Header("로고 작동 유예 시간")][SerializeField] private float appearingTime = 0.5f;
        [Header("서서히 나타날 로고")][SerializeField] private Image titleLogo;
        [Header("서서히 나타날 버튼들")][SerializeField] private List<Button> buttonList;
        [Header("플레이 버튼 후 열릴 UI")][SerializeField] private GameObject playObj;
        [Header("옵션 버튼 후 열릴 UI")][SerializeField] private GameObject optionObj;

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
            // 서서히 나타나게끔
            yield return new WaitForSeconds(startTime);
            for (int i = 1 ; i<=50 ; ++i)
            {
                yield return new WaitForSeconds(appearingTime/50f);
                transparentColor.a = 0.02f * i;
                titleLogo.color = transparentColor;
                for (int j = 0 ; j<buttonTexts.Count ; ++j)
                    buttonTexts[j].color = transparentColor;
            }

            // 버튼 작동 켜기
            for (int k = 0 ; k<buttonList.Count ; ++k)
            {
                buttonList[k].interactable = true;
                buttonList[k].gameObject.transform.GetChild(0).gameObject.GetComponent<FontColorShift>().enabled = true;
            }

            buttonList[(int)MenuSelect.Play].onClick.AddListener(() => 
            {                 
                RoomOptions lobbyOptions = new()
                {
                    MaxPlayers = 20,    // 최대 접속자수, 포톤 무료는 20CCU이므로 20 초과로는 못한다.
                    IsOpen = true,      // 룸의 오픈 여부
                    IsVisible = false,   // 로비에서 룸 목록에 노출시킬지 여부  
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

