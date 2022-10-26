using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JCW.Object
{
    public class PlayerShadow : MonoBehaviour
    {
        [Header("�⺻ �׸��� ������ ��")] [SerializeField] [Range(0f, 0.1f)] float defaultShadowValue = 0.01f;
        [Header("�ִ� �׸��� ������ ��")] [SerializeField] [Range(0f, 0.1f)] float maxShadowValue = 0.025f;

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
                // ���� 0.03�� �ּ�, 1.99 ������ �ִ밪
                //playerState.height
                //shadowAlpha.a = 
            }
        }
    }
}

