using System.Collections;
using System.Collections.Generic;
using JCW.AudioCtrl;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace JCW.UI.InGame
{
    public class LastSubtitle : MonoBehaviour
    {
        [Header("���� �÷��̾�")][SerializeField] VideoPlayer video;
        [Header("�ڶ� �ؽ�Ʈ")][SerializeField] Text nellaText;
        [Header("���׵� �ؽ�Ʈ")][SerializeField] Text steadyText;

        Text nellaRealText;
        Text steadyRealText;

        // �� 16��
        private void Awake()
        {
            nellaRealText = nellaText.transform.GetChild(1).GetComponent<Text>();
            steadyRealText = steadyText.transform.GetChild(1).GetComponent<Text>();
        }
        void Start()
        {
            StartCoroutine(nameof(WaitForVideo));
        }
        IEnumerator StartSubtitle()
        {
            steadyText.text = "�����ؼ� ���� �����̾�.";
            steadyRealText.text = steadyText.text;
            steadyText.gameObject.SetActive(true);
            SoundManager.Instance.PlayEffect_RPC("steady_talk_8");

            yield return new WaitForSeconds(2f);

            steadyText.gameObject.SetActive(false);
            nellaText.text = "�ٸ���, ���� ���� �ܷ��� �����ž�.";
            nellaRealText.text = nellaText.text;
            nellaText.gameObject.SetActive(true);
            SoundManager.Instance.PlayEffect_RPC("nella_talk_8");

            yield return new WaitForSeconds(2.5f);

            nellaText.gameObject.SetActive(false);
            yield return new WaitForSeconds(2.5f);

            nellaText.text = "� ģ���� ������ ���ư���.";
            nellaRealText.text = nellaText.text;
            nellaText.gameObject.SetActive(true);
            SoundManager.Instance.PlayEffect_RPC("nella_talk_7");

            yield return new WaitForSeconds(2f);
            nellaText.gameObject.SetActive(false);
            yield break;
        }

        IEnumerator WaitForVideo()
        {
            yield return new WaitUntil(() => video.isPlaying);
            yield return new WaitForSeconds(3f);
            StartCoroutine(nameof(StartSubtitle));
            yield break;
        }
    }

}
