using System.Collections;
using System.Collections.Generic;
using JCW.AudioCtrl;
using UnityEngine;


namespace JCW.Object
{
    [RequireComponent(typeof(AudioSource))]
    public class CymbalsButton : MonoBehaviour
    {
        [Header("버튼 들어가는 속도")] [SerializeField] float pressedSpeed = 5f;
        [Header("버튼 나오는 속도")] [SerializeField] float releaseSpeed = 5f;
        [Header("버튼 나오기 시작하는 시간")] [SerializeField] float releaseTime = 7f;
        [Header("평상 시 버튼 메테리얼")] [SerializeField] Material normalMat;
        [Header("작동 시 버튼 메테리얼")] [SerializeField] Material activeMat;
        [Header("영구적인 버튼 여부")] [SerializeField] bool isPermanent;
        [Header("상호작용할 수 있는 오브젝트들")] [SerializeField] List<PlayObject> objectList;

        //bool isPressed;
        float pressed_Ratio = 1f;
        Animator anim;
        MeshRenderer meshRenderer;
        //bool isMatChanged = false;

        AudioSource audioSource;

        WaitForSeconds ws;

        private void Awake()
        {
            anim = transform.parent.parent.GetComponent<Animator>();
            meshRenderer = transform.GetChild(1).GetComponent<MeshRenderer>();
            meshRenderer.material = normalMat;
            audioSource = GetComponent<AudioSource>();
            AudioCtrl.AudioSettings.SetAudio(audioSource, 1f, 50f);
            ws = new(releaseTime);
        }
        /*
        void Update()
        {
            if (isPressed)
            {
                if (pressed_Ratio != 0f)
                {
                    // 닿는 순간 메테리얼 변경
                    if (!isMatChanged)
                    {
                        //SoundManager.Instance.Play3D_RPC("ButtonTurnOn", audioSource);
                        meshRenderer.material = activeMat;
                        isMatChanged = true;
                        Debug.Log("메테리얼 교체");
                    }

                    // 애니메이션 재생
                    pressed_Ratio = Mathf.Lerp(pressed_Ratio, 0, pressedSpeed * Time.deltaTime);
                    if (pressed_Ratio < 0.01f)
                    {
                        Debug.Log("1->0이 되었으므로 다시 버튼 작동 해제");
                        pressed_Ratio = 0f;
                        for (int i = 0 ; i<objectList.Count ; ++i)
                        {
                            objectList[i].GetComponent<PlayObject>().enabled = true;
                            objectList[i].isActive = true;
                        }
                        if (isPermanent)
                            Destroy(this);
                    }
                    anim.SetFloat("pressed_ratio", pressed_Ratio);
                    anim.Play("Press Blend Tree");
                }
            }
            else if (!isPermanent && pressed_Ratio != 1f)
            {

                if (pressed_Ratio > 0.95f)
                {
                    isPressed = false;
                    Debug.Log("0->1이 되었으므로 버튼 작동");
                    pressed_Ratio = 1f;
                    meshRenderer.material = normalMat;
                    isMatChanged = false;
                    for (int i = 0 ; i<objectList.Count ; ++i)
                    {
                        objectList[i].isActive = false;
                    }
                }
                pressed_Ratio = Mathf.Lerp(pressed_Ratio, 1, releaseSpeed * Time.deltaTime);
                anim.SetFloat("pressed_ratio", pressed_Ratio);
                anim.Play("Press Blend Tree");
            }
        }
        */
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Cymbals"))
                StartCoroutine("ActiveObj");
        }

        IEnumerator ActiveObj()
        {
            // 닿는 순간 메테리얼 변경
            StopCoroutine("DeactiveObj");
            meshRenderer.material = activeMat;

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
                Destroy(this);
            else
                StartCoroutine("DeactiveObj");
            yield break;
        }

        IEnumerator DeactiveObj()
        {
            yield return ws;
            while (pressed_Ratio <= 0.95f)
            {
                pressed_Ratio = Mathf.Lerp(pressed_Ratio, 1, releaseSpeed * Time.deltaTime);
                anim.SetFloat("pressed_ratio", pressed_Ratio);
                anim.Play("Press Blend Tree");
                yield return null;
            }

            pressed_Ratio = 1f;
            anim.SetFloat("pressed_ratio", pressed_Ratio);
            meshRenderer.material = normalMat;
            for (int i = 0 ; i<objectList.Count ; ++i)
            {
                objectList[i].isActive = false;
            }
            yield break;
        }
    }
}

