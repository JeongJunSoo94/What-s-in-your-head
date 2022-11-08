using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JCW.Object.Stage1
{
    public class FlagCheck : MonoBehaviour
    {
        [Header("열릴 오브젝트의 BothButton 스크립트")] [SerializeField] BothButton bothButton;
        [Header("빛을 낼 오브젝트")] [SerializeField] Transform lightButton;
        [Header("초기화 할 시간")] [SerializeField] [Range(0,50)] float resetTime = 5f;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Nella") || other.CompareTag("Steady"))
            {
                StopAllCoroutines();
                ++bothButton.bothCount;
                lightButton.GetChild(0).gameObject.SetActive(false);
                lightButton.GetChild(1).gameObject.SetActive(true);                
                GetComponent<Animator>().SetBool("isCheck", true);
                Destroy(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Nella") || other.CompareTag("Steady"))
            {
                StartCoroutine(nameof(Reset));
            }
        }

        IEnumerator Reset()
        {
            float curTime = 0f;
            while(curTime < resetTime)
            {
                curTime += Time.deltaTime;
                yield return null;
            }
            GetComponent<Animator>().SetBool("isCheck", true);
            --bothButton.bothCount;
        }
    }
}

