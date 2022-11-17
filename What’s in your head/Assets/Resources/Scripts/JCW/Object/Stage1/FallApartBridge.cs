using System.Collections;
using System.Collections.Generic;
using JCW.AudioCtrl;
using UnityEngine;

namespace JCW.Object.Stage1
{
    [RequireComponent(typeof(AudioSource))]
    public class FallApartBridge : MonoBehaviour
    {
        [Header("���� ���� ��ȣ ǥ�� �ð�")][SerializeField] float warnTime = 3f;
        [Header("���� ���� �����Ÿ� �ð� ����")][SerializeField] float flickTime = 0.5f;
        [Header("�� �� �� �ر��Ǵ� ��")][SerializeField] float destroyTime = 5f;
        [Header("�ر� �� ������Ǵ� �ð�")][SerializeField] float resetTime = 7f;
        [Header("��� ���׸���")][SerializeField] Material warnMat;

        MeshRenderer meshRenderer;
        Material normalMat;
        AudioSource audioSource;
        BoxCollider boxCollider;
        GameObject triggerBox;

        bool isStart = false;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            boxCollider = GetComponent<BoxCollider>();
            normalMat = meshRenderer.sharedMaterial;
            audioSource = GetComponent<AudioSource>();
            triggerBox = transform.GetChild(0).gameObject;
            JCW.AudioCtrl.AudioSettings.SetAudio(audioSource, 1f, 50f);
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
            SoundManager.Instance.Play3D_RPC("S1S2_RainBowBridgeBroken2", audioSource);

            // ���⼭ ī�޶� ��鸲 ������ �ɵ�
            while (curTime < warnTime)
            {
                curTime += Time.deltaTime;
                curFlickTime = curTime;
                if (curFlickTime > flickTime)
                {
                    curFlickTime = 0f;
                    meshRenderer.sharedMaterial = isNormal ? warnMat : normalMat;
                    isNormal = !isNormal;
                }
                yield return null;
            }
            while (curTime < destroyTime)
            {
                curTime += Time.deltaTime;
                yield return null;
            }
            SoundManager.Instance.Play3D_RPC("S1S2_RainBowBridgeBroken1", audioSource);
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
