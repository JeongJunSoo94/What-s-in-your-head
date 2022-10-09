using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JCW.UI
{
    public class ComeAndGo : MonoBehaviour
    {
        [Header("������� �����ϴ� �ð�")][SerializeField] private float startTime = 3.0f;
        [Header("������� �� ���� �ð�")][SerializeField] private float disappearingTime = 1f;
        [Header("�Էµ� ģ�� ID")] [SerializeField] private Text friendID = null;

        Color transparentWhite;
        Color transparentBlack;

        private Image thisImg = null;
        private Text thisText = null;

        private void Awake()
        {
            thisImg = this.gameObject.GetComponent<Image>();
        }

        private void OnEnable()
        {            
            transparentWhite = new Color(1, 1, 1, 1);
            thisImg.color = transparentWhite;
            transparentBlack = new Color(0, 0, 0, 1);
            if (this.gameObject.transform.childCount != 0)
            {
                thisText = this.gameObject.transform.GetChild(0).GetComponent<Text>();
                thisText.color = transparentBlack;
                thisText.text = friendID.text + " �Կ��� �ʴ븦 ���½��ϴ�.";
            }
            StartCoroutine(nameof(Disappear));
        }

        IEnumerator Disappear()
        {
            // ������ ��Ÿ���Բ�
            yield return new WaitForSeconds(startTime);
            int i = 0;
            while (i<50)
            {
                yield return new WaitForSeconds(disappearingTime/50f);
                ++i;
                transparentWhite.a -= (5.1f/255f);
                transparentBlack.a -= (5.1f/255f);
                thisImg.color = transparentWhite;
                if (thisText != null)
                    thisText.color = transparentBlack;
            }
            this.gameObject.SetActive(false);
            yield return null;

        }
    }
}

