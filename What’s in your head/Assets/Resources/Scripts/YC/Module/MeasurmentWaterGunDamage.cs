using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �ڶ��� ���� ������ ���� Ŭ���� (MeasurmentWaterGunDamage)

namespace YC_OBJ
{
    public class MeasurmentWaterGunDamage : MonoBehaviour
    {
        [Header("<��ȹ ���� ����>")]
        [Space]

        [Header("������ ī��Ʈ �ִ� ��")]
        [Header(":  ���� �������� �ش� ���� ������ �̺�Ʈ�� �̷������")]
        [SerializeField] float maxCount = 100;

        [Header("������ ī��Ʈ ��ȭ ��")]
        [Header(":  ������ ���� �ʾ��� �� ���̴� ��")]
        [SerializeField] int changeValue = 1;

        public float curCount { get; private set; }
        public bool Trigger { get; private set; } = false;

        float curTime = 0.0f;
        float maxTime = 0.01f;
        bool isStart = false;

        string interactionTag = "NellaWater";

     
       
        void Update()
        {
            Debug.Log(curCount);    
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(interactionTag))
            {
                if (!isStart)
                    StartCoroutine("SetCount");

                curTime = 0;

                curCount++;
            }
        }

        IEnumerator SetCount()
        {
            while (SetGauge())
            {
                yield return new WaitForSeconds(1.0f);
            }
            yield return null;
            isStart = false;
        }

        bool SetGauge()
        {
            if (Trigger) return false;

            if (curCount > 0)
            {
                curTime += Time.deltaTime;

                if (curTime > maxTime)
                {
                    curTime = 0;

                    curCount -= changeValue;

                    return false;
                }
            }

            if (curCount > maxCount)
            {
                curCount = maxCount;
                Trigger = true;
                return false;
            }

            return true;
        }

        public void Reset()
        {
            if (!Trigger) return;
                
            Trigger = false;

            curCount = 0;
        }
    }
}
