using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 넬라의 물총 게이지 측정 클래스 (MeasurmentWaterGunDamage)

namespace YC_OBJ
{
    public class MeasurmentWaterGunDamage : MonoBehaviour
    {
        [Header("<기획 편집 사항>")]
        [Space]

        [Header("게이지 카운트 최대 값")]
        [Header(":  현재 게이지가 해당 값을 넘으면 이벤트가 이루어진다")]
        [SerializeField] float maxCount = 100;

        [Header("게이지 카운트 변화 값")]
        [Header(":  물총을 맞지 않았을 때 깎이는 값")]
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
