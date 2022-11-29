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
        [Header("비디오 플레이어")][SerializeField] VideoPlayer video;
        [Header("넬라 텍스트")][SerializeField] Text nellaText;
        [Header("스테디 텍스트")][SerializeField] Text steadyText;

        Text nellaRealText;
        Text steadyRealText;

        // 총 16초
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
            steadyText.text = "무사해서 정말 다행이야.";
            steadyRealText.text = steadyText.text;
            steadyText.gameObject.SetActive(true);
            SoundManager.Instance.PlayEffect_RPC("steady_talk_8");

            yield return new WaitForSeconds(2f);

            steadyText.gameObject.SetActive(false);
            nellaText.text = "앨리스, 이제 더는 외롭지 않을거야.";
            nellaRealText.text = nellaText.text;
            nellaText.gameObject.SetActive(true);
            SoundManager.Instance.PlayEffect_RPC("nella_talk_8");

            yield return new WaitForSeconds(2.5f);

            nellaText.gameObject.SetActive(false);
            yield return new WaitForSeconds(2.5f);

            nellaText.text = "어서 친구들 곁으로 돌아가자.";
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
