using System.Collections;
using System.Collections.Generic;
using JCW.AudioCtrl;
using Photon.Pun;
using UnityEngine;
using YC.Camera_;

namespace JCW.Object.Stage1
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(PhotonView))]
    public class FallApartBridge : MonoBehaviour
    {
        [Header("위험 전조 신호 표시 시간")] [SerializeField] float warnTime = 3f;
        [Header("위험 전조 깜빡거릴 시간 간격")] [SerializeField] float flickTime = 0.1f;
        [Header("몇 초 후 붕괴되는 지")] [SerializeField] float destroyTime = 5f;
        [Header("붕괴 후 재생성되는 시간")] [SerializeField] float resetTime = 7f;

        MeshRenderer meshRenderer;
        MeshRenderer meshRenderer_Blink;
        AudioSource audioSource;
        BoxCollider boxCollider;

        bool isStart = false;
        PhotonView pv;


        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer_Blink = transform.GetChild(1).GetComponent<MeshRenderer>();
            boxCollider = transform.GetChild(0).GetComponent<BoxCollider>();
            audioSource = GetComponent<AudioSource>();
            pv = GetComponent<PhotonView>();
            SoundManager.Set3DAudio(pv.ViewID, audioSource, 1f, 50f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Nella") || other.CompareTag("Steady"))
            {
                other.GetComponent<CameraController>().ShakeCamera(true);
                if (!isStart)
                {
                    isStart = true;
                    StartCoroutine(BreakBridge(other));
                   
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Nella") || other.CompareTag("Steady"))
            {
                other.GetComponent<CameraController>().ShakeCamera(false);
            }
        }


        IEnumerator BreakBridge(Collider other)
        {
            float curTime = 0f;
            float curFlickTime = 0f;
            bool isNormal = true;
            SoundManager.Instance.Play3D_RPC("S1S2_RainBowBridgeBroken2", pv.ViewID);

            // 여기서 카메라 흔들림 넣으면 될듯

            while (curTime < destroyTime)
            {
                curTime += Time.deltaTime;
                if (curTime > destroyTime - warnTime)
                {
                    curFlickTime += Time.deltaTime;
                    if (curFlickTime > flickTime)
                    {
                        curFlickTime = 0f;
                        meshRenderer.enabled = !isNormal;
                        meshRenderer_Blink.enabled = isNormal;
                        isNormal = !isNormal;
                    }
                }
                yield return null;
            }
            SoundManager.Instance.Play3D_RPC("S1S2_RainBowBridgeBroken", pv.ViewID);
            meshRenderer.enabled = false;
            meshRenderer_Blink.enabled = false;
            boxCollider.enabled = false;
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
            meshRenderer.enabled = true;
            meshRenderer_Blink.enabled = false;
            boxCollider.enabled = true;
            isStart = false;
            yield break;
        }
    }

}
