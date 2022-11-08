using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JCW.Object.Stage1
{
    public class FlagCheck : MonoBehaviour
    {
        [Header("열릴 오브젝트의 BothButton 스크립트")] [SerializeField] BothButton bothButton;
        [Header("빛을 낼 오브젝트")] [SerializeField] Transform lightButton;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Nella") || other.CompareTag("Steady"))
            {
                ++bothButton.bothCount;
                lightButton.GetChild(0).gameObject.SetActive(false);
                lightButton.GetChild(1).gameObject.SetActive(true);
                Destroy(this);
            }
        }
    }
}

