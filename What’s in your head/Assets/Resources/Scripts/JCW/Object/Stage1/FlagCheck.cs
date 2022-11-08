using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JCW.Object.Stage1
{
    public class FlagCheck : MonoBehaviour
    {
        [Header("���� ������Ʈ�� BothButton ��ũ��Ʈ")] [SerializeField] BothButton bothButton;
        [Header("���� �� ������Ʈ")] [SerializeField] Transform lightButton;
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

