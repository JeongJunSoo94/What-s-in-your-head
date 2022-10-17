using JCW.UI.Options.InputBindings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace JCW.UI.InGame
{
    public class CPR : MonoBehaviour
    {
        [Header("부활 게이지 이미지")] [SerializeField] Image heartGauge;
        [Header("부활 게이지 증가량")] [SerializeField] [Range(0f,0.05f)] float increaseValue;
        [Header("버튼 입력 시 증가량")] [SerializeField] [Range(0f,0.05f)] float addIncreaseValue = 0.01f;
        [Header("버튼 입력 시 재생될 비디오")] [SerializeField] VideoPlayer heartBeat;


        void Update()
        {
            heartGauge.fillAmount += increaseValue * Time.deltaTime;
            if (KeyManager.Instance.GetKeyDown(PlayerAction.Interaction))
            {
                heartGauge.fillAmount += addIncreaseValue;
                if(!heartBeat.isPlaying)
                    heartBeat.Play();
            }
        }
    }
}

