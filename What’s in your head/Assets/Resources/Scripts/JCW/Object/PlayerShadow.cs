using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JCW.Object
{
    public class PlayerShadow : MonoBehaviour
    {
        [Header("기본 그림자 스케일 값")] [SerializeField] [Range(0f, 0.1f)] float defaultShadowValue = 0.01f;
        [Header("최대 그림자 스케일 값")] [SerializeField] [Range(0f, 0.1f)] float maxShadowValue = 0.025f;

        PlayerState playerState;
        Image shadowImg;
        Color shadowAlpha;

        private void Awake()
        {
            shadowImg = transform.GetChild(0).GetChild(0).GetComponent<Image>();
            shadowAlpha = shadowImg.color;
        }



        void Update()
        {
            if(!playerState.IsGrounded)
            {
                // 대충 0.03이 최소, 1.99 정도가 최대값
                //playerState.height
                //shadowAlpha.a = 
            }
        }
    }
}

