using System.Collections;
using System.Collections.Generic;
using JCW.AudioCtrl;
using UnityEngine;


namespace JCW.Object
{
    [RequireComponent(typeof(AudioSource))]
    public class PressButton : MonoBehaviour
    {
        [Header("버튼 들어가는 속도")] [SerializeField] float pressedSpeed = 5f;
        [Header("버튼 나오는 속도")] [SerializeField] float releaseSpeed = 5f;
        [Header("(심벌즈) 버튼 해제되는 시간")] [SerializeField] float releaseTime = 5f;
        [Header("평상 시 버튼 메테리얼")] [SerializeField] Material normalMat;
        [Header("작동 시 버튼 메테리얼")] [SerializeField] Material activeMat;
        [Header("영구적인 버튼 여부")] [SerializeField] bool isPermanent;

        bool isPressed;
        float pressed_Ratio = 0f;
        Animator anim;
        MeshRenderer meshRenderer;
        bool isMatChanged = false;

        int curPressCount = 0;

        AudioSource audioSource;

        private void Awake()
        {
            anim = transform.parent.parent.parent.GetComponent<Animator>();
            meshRenderer = transform.parent.GetChild(1).GetComponent<MeshRenderer>();
            meshRenderer.material = normalMat;
            audioSource = GetComponent<AudioSource>();
            AudioCtrl.AudioSettings.SetAudio(audioSource, 1f, 50f);
        }

        void Update()
        {
            if(isPressed)
            {
                if(pressed_Ratio != 0f)
                {
                    if (!isMatChanged)
                    {
                        SoundManager.Instance.Play3D_RPC("ButtonTurnOn", audioSource);
                        meshRenderer.material = activeMat;
                        isMatChanged = true;
                    }
                    pressed_Ratio = Mathf.Lerp(pressed_Ratio, 0, pressedSpeed * Time.deltaTime);
                    if (pressed_Ratio < 0.01f)
                    {
                        pressed_Ratio = 0f;
                        if (isPermanent)
                            Destroy(this);
                    }
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
            switch(collision.gameObject.tag)
            {
                case "Nella":
                case "Steady":
                    isPressed = true;
                    collision.gameObject.transform.parent = this.transform;
                    ++curPressCount;
                    break;
                case "Cymbals":
                    StopAllCoroutines();
                    isPressed = true;
                    if(!isPermanent)
                        StartCoroutine(nameof(ReleaseTime));
                    break;
                default:
                    break;
            }
                
        }

        private void OnCollisionExit(Collision collision)
        {
            switch (collision.gameObject.tag)
            {
                case "Nella":
                case "Steady":
                    --curPressCount;
                    collision.gameObject.transform.parent = null;
                    if (curPressCount == 0)
                        isPressed = false;
                    break;
                default:
                    break;
            }
        }

        IEnumerator ReleaseTime()
        {
            yield return new WaitForSeconds(releaseTime);
            isPressed = false;
        }
    }
}

