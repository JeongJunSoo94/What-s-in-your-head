using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using JCW.UI.Options;
using JCW.Network;

namespace JCW.UI
{    
    public class MenuUI : MonoBehaviour
    {
        [Header("�� �� �� ����")][SerializeField] private float startTime = 1.0f;
        [Header("�� �ʿ� ���� ����")][SerializeField] private float appearingTime = 0.5f;
        [Header("������ ��Ÿ�� ������Ʈ��")][SerializeField] private List<GameObject> turningOnObj = new();
        [Header("��Ÿ�� �� �۵���ų ������Ʈ��")][SerializeField] private List<GameObject> buttonObj = new();
        [Header("�÷��� ��ư �� ���� UI")][SerializeField] private GameObject playObj = null;
        [Header("�ɼ� ��ư �� ���� UI")][SerializeField] private GameObject optionObj = null;

        private readonly List<Button> turningOnButtons = new();
        private readonly List<Text> buttonTexts = new();
        private Image titleLogo = null;

        private int logoIndex = 0;

        Color transparentColor;
        
        private void Awake()
        {
            transparentColor = new Color(1, 1, 1, 0);
            Debug.Log("���� ���� : " + (transparentColor.a * 255f));

            for (int i = 0 ; i<buttonObj.Count ; ++i)
            {
                turningOnButtons.Add(buttonObj[i].GetComponent<Button>());
            }

            for (int i = 0 ; i<turningOnObj.Count ; ++i)
            {
                titleLogo = turningOnObj[i].GetComponent<Image>();
                if (titleLogo != null)
                {
                    logoIndex = i;
                    break;
                }
            }
            for (int i = 0 ; i<turningOnObj.Count ; ++i)
            {
                if (logoIndex == i)
                    continue;
                buttonTexts.Add(turningOnObj[i].GetComponent<Text>());
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
            int i = 0;
            while (i<50)
            {
                yield return new WaitForSeconds(appearingTime/50f);
                ++i;
                transparentColor.a = (5.1f*i/255f);
                titleLogo.color = transparentColor;
                for (int k = 0 ; k<buttonTexts.Count ; ++k)
                    buttonTexts[k].color = transparentColor;
            }

            for (int l = 0 ; l<turningOnObj.Count ; ++l)
            {
                if (logoIndex == l)
                    continue;
                turningOnObj[l].AddComponent<FontColorShift>();
            }

            // ��ư �۵� �ѱ�
            for (int j = 0 ; j<turningOnButtons.Count ; ++j)
            {
                turningOnButtons[j].interactable = true;
            }

            turningOnButtons[0].onClick.AddListener(() => 
            { 
                playObj.SetActive(true);
                RoomOptions temp_RO = new()
                {
                    MaxPlayers = 20,    // �ִ� �����ڼ�, ���� ����� 20CCU�̹Ƿ� 20 �ʰ��δ� ���Ѵ�.
                    IsOpen = true,      // ���� ���� ����
                    IsVisible = false,   // �κ񿡼� �� ��Ͽ� �����ų�� ����  
                };
                PhotonNetwork.JoinOrCreateRoom("Lobby", temp_RO, null);
            });
            turningOnButtons[1].onClick.AddListener(() => { optionObj.SetActive(true); });
            turningOnButtons[2].onClick.AddListener(() =>
            {
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
            });

            yield return null;

        }
    }
}

