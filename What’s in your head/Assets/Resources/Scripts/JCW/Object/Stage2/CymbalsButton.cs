using System.Collections;
using System.Collections.Generic;
using JCW.AudioCtrl;
using KSU.AutoAim.Object;
using Photon.Pun;
using UnityEngine;


namespace JCW.Object
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(PhotonView))]
    public class CymbalsButton : AutoAimTargetObject
    {
        [Header("버튼 들어가는 속도")] [SerializeField] float pressedSpeed = 5f;
        [Header("버튼 나오는 속도")] [SerializeField] float releaseSpeed = 5f;
        [Header("버튼 나오기 시작하는 시간")] [SerializeField] float releaseTime = 1f;
        [Header("바꿀 메테리얼이 달린 버튼")] [SerializeField] MeshRenderer meshRenderer;
        [Header("평상 시 버튼 메테리얼")] [SerializeField] Material normalMat;
        [Header("작동 시 버튼 메테리얼")] [SerializeField] Material activeMat;
        [Header("영구적인 버튼 여부")] [SerializeField] bool isPermanent;
        [Header("상호작용할 수 있는 오브젝트들")] [SerializeField] List<PlayObject> objectList;

        //bool isPressed;
        float pressed_Ratio = 1f;
        Animator anim;
        //MeshRenderer meshRenderer;
        //bool isMatChanged = false;

        AudioSource audioSource;

        WaitForSeconds ws;

        PhotonView pv;
        

        protected override void Awake()
        {
            base.Awake();
            pv = GetComponent<PhotonView>();
            anim = transform.parent.parent.parent.GetComponent<Animator>();
            //meshRenderer = transform.GetChild(1).GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = normalMat;
            TryGetComponent(out audioSource);
            if(SoundManager.Instance != null)
                SoundManager.Set3DAudio(pv.ViewID, audioSource, 1f, 50f);
            ws = new(releaseTime);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Cymbals"))
                StartCoroutine("ActiveObj");
        }

        IEnumerator ActiveObj()
        {
            // 닿는 순간 메테리얼 변경
            StopCoroutine("DeactiveObj");
            meshRenderer.sharedMaterial = activeMat;

            for (int i = 0 ; i<objectList.Count ; ++i)
            {
                objectList[i].GetComponent<PlayObject>().enabled = true;
                objectList[i].isActive = true;
            }

            while (pressed_Ratio >= 0.01f)
            {
                pressed_Ratio = Mathf.Lerp(pressed_Ratio, 0, pressedSpeed * Time.deltaTime);
                anim.SetFloat("pressed_ratio", pressed_Ratio);
                anim.Play("Press Blend Tree");
                yield return null;
            }
            pressed_Ratio = 0f;
            anim.SetFloat("pressed_ratio", pressed_Ratio);

            if (isPermanent)
                this.enabled = false;
            else
                StartCoroutine("DeactiveObj");
            yield break;
        }

        IEnumerator DeactiveObj()
        {
            while (pressed_Ratio <= 0.95f)
            {
                pressed_Ratio = Mathf.Lerp(pressed_Ratio, 1, releaseSpeed * Time.deltaTime);
                anim.SetFloat("pressed_ratio", pressed_Ratio);
                anim.Play("Press Blend Tree");
                yield return null;
            }

            pressed_Ratio = 1f;
            anim.SetFloat("pressed_ratio", pressed_Ratio);
            meshRenderer.sharedMaterial = normalMat;
            for (int i = 0 ; i<objectList.Count ; ++i)
            {
                objectList[i].isActive = false;
            }
            yield break;
        }
    }
}

