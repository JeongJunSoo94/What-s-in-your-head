using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JCW.Object
{
    public class PressButton : MonoBehaviour
    {
        [Header("버튼 내려가는 속도")] [SerializeField] float pressedSpeed = 5f;
        [Header("버튼 올라오는 속도")] [SerializeField] float releaseSpeed = 5f;
        [Header("평상 시 버튼 메테리얼")] [SerializeField] Material normalMat;
        [Header("작동 시 버튼 메테리얼")] [SerializeField] Material activeMat;

        bool isPressed;
        float pressed_Ratio = 0f;
        Animator anim;
        MeshRenderer meshRenderer;
        bool isMatChanged = false;

        int curPressCount = 0;

        private void Awake()
        {
            anim = transform.parent.parent.parent.GetComponent<Animator>();
            meshRenderer = transform.parent.GetChild(1).GetComponent<MeshRenderer>();
            meshRenderer.material = normalMat;
        }

        void Update()
        {
            if(isPressed)
            {
                if(pressed_Ratio != 0f)
                {
                    if (!isMatChanged)
                    {
                        meshRenderer.material = activeMat;
                        isMatChanged = true;
                    }
                    pressed_Ratio = Mathf.Lerp(pressed_Ratio, 0, pressedSpeed * Time.deltaTime);
                    if (pressed_Ratio < 0.01f)
                        pressed_Ratio = 0f;
                    anim.SetFloat("pressed_ratio", pressed_Ratio);
                    anim.Play("Press Blend Tree");
                }                
            }
            else
            {
                if(pressed_Ratio != 1f)
                {
                    if (pressed_Ratio > 0.95f)
                    {
                        pressed_Ratio = 1f;
                        meshRenderer.material = normalMat;
                        isMatChanged = false;
                    }
                    pressed_Ratio = Mathf.Lerp(pressed_Ratio, 1, releaseSpeed * Time.deltaTime);
                    anim.SetFloat("pressed_ratio", pressed_Ratio);
                    anim.Play("Press Blend Tree");
                }                
            }


        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.CompareTag("Nella") || collision.gameObject.CompareTag("Steady"))
            {
                isPressed = true;
                collision.gameObject.transform.parent = this.transform;
                ++curPressCount;
            }
                
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Nella") || collision.gameObject.CompareTag("Steady"))
            {
                --curPressCount;
                collision.gameObject.transform.parent = null;
                if(curPressCount==0)
                    isPressed = false;
            }
        }
    }
}

