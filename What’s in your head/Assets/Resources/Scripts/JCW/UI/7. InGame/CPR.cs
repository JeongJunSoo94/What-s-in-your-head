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
        [Header("��Ȱ ������ �̹���")] [SerializeField] Image heartGauge;
        [Header("��Ȱ ������ ������")] [SerializeField] [Range(0f,0.05f)] float increaseValue;
        [Header("��ư �Է� �� ������")] [SerializeField] [Range(0f,0.05f)] float addIncreaseValue = 0.01f;
        [Header("��ư �Է� �� ����� ����")] [SerializeField] VideoPlayer heartBeat;


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

