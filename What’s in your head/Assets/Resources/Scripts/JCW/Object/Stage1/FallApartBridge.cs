using System.Collections;
using System.Collections.Generic;
using JCW.AudioCtrl;
using Photon.Pun;
using UnityEngine;

namespace JCW.Object.Stage1
{
    [RequireComponent(typeof(AudioSource))]
    public class FallApartBridge : MonoBehaviour
    {
        [Header("위험 전조 신호 표시 시간")] [SerializeField] float warnTime = 3f;
        [Header("위험 전조 깜빡거릴 시간 간격")] [SerializeField] float flickTime = 0.7f;
        [Header("몇 초 후 붕괴되는 지")] [SerializeField] float destroyTime = 5f;
        [Header("붕괴 후 재생성되는 시간")] [SerializeField] float resetTime = 7f;
        [Header("경고 메테리얼")] [SerializeField] Material warnMat;

        MeshRenderer meshRenderer;
        Material normalMat;
        AudioSource audioSource;
        BoxCollider boxCollider;
        GameObject triggerBox;

        bool isStart = false;
        PhotonView pv;

        int audioID = 0;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            boxCollider = GetComponent<BoxCollider>();
            normalMat = meshRenderer.sharedMaterial;
            audioSource = GetComponent<AudioSource>();
            triggerBox = transform.GetChild(0).gameObject;
            audioID = JCW.AudioCtrl.AudioSettings.SetAudio(audioSource, 1f, 50f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Nella") || other.CompareTag("Steady"))
            {
                if (!isStart)
                {
                    isStart = true;
                    StartCoroutine(nameof(BreakBridge));
                }
            }
        }

        IEnumerator BreakBridge()
        {
            float curTime = 0f;
            float curFlickTime = 0f;
            bool isNormal = true;
            SoundManager.Instance.Play3D_RPC("S1S2_RainBowBridgeBroken2", audioID);
            Debug.Log(gameObject.name);

            // 여기서 카메라 흔들림 넣으면 될듯

            while (curTime < destroyTime)
            {
                curTime += Time.deltaTime;
                if (curTime > destroyTime - warnTime)
                {
                    curFlickTime = curTime;
                    if (curFlickTime > flickTime)
                    {
                        curFlickTime = 0f;
                        meshRenderer.sharedMaterial = isNormal ? warnMat : normalMat;
                        isNormal = !isNormal;
                    }
                }
                yield return null;
            }
            SoundManager.Instance.Play3D_RPC("S1S2_RainBowBridgeBroken", audioID);
            Debug.Log(gameObject.name);
            meshRenderer.enabled = false;
            boxCollider.enabled = false;
            triggerBox.SetActive(false);
            StartCoroutine(nameof(ResetBridge));
            yield break;
        }


        IEnumerator ResetBridge()
        {
            float curTime = 0f;
            while (curTime < resetTime)
            {
                curTime += Time.deltaTime;
                yield return null;
            }
            meshRenderer.sharedMaterial = normalMat;
            meshRenderer.enabled = true;
            boxCollider.enabled = true;
            triggerBox.SetActive(true);
            isStart = false;
            yield break;
        }
    }

}
