using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary> 
/// 
/// Current Issue       : 제작 에셋 포지션 수정 필요
/// 
/// Object Name         : 화분 꽃봉오리
/// 
/// Object Description  : 일정시간동안 넬라의 물총을 맞게 된다면, 꽃이 피고, 스테디의 갈고리 스킬과 상호작용이 가능하다.
/// 
/// Script Description  :
///                         
/// Player Intraction   : 넬라(물총 스킬)
/// 
/// </summary>
/// 

namespace YC_OBJ
{
    public class PotFlower : MonoBehaviour
    {
        [Header("<기획 편집 사항>")]
        [Header("[히트 시간 (해당 시간동안 물총을 맞을 경우 꽃을 피우게 된다)]")]
        [SerializeField] float MaxTime = 2.0f;
        public float maxTime  { get; private set; } // 최대 시간 게이지


        public float curTime { get; private set; } = 0; // 현재 시간 게이지
        bool isTrigger = false;

        
        bool isCurHit = false;
        int curHitCount = 0;
        float delayTime = 0.2f; // 총알을 현재 맞고 있는지를 몇 초 전과 비교할 것인지 (총알 발사 속도와 연관)

        string interactionObjTag = "NellaWater";

        Animator animator;
        PhotonView pv;

        private void Awake()
        {
            maxTime = MaxTime;

            animator = this.gameObject.GetComponent<Animator>();

            pv = this.gameObject.GetComponent<PhotonView>();
        }

        void Update()
        {
            if(!isTrigger) SetCurTime();
        }

        void SetCurTime()
        {
            if(isCurHit)
            {
                if (curTime > maxTime) return;

                curTime += Time.deltaTime;

                if (curTime > maxTime)
                {
                    curTime = maxTime;

                    if (pv.IsMine)
                        animator.SetBool("isBoom", true);

                    isTrigger = true;
                }

            }
            else
            {
                if (curTime < 0) return;

                curTime -= Time.deltaTime;

                if (curTime < 0)
                    curTime = 0;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("트리거 체크!");
            if(other.gameObject.CompareTag(interactionObjTag))
            {
                Debug.Log("태그 체크!");
                curHitCount++;

                StartCoroutine(nameof(SetPrevHit));
            }
        }

        IEnumerator SetPrevHit()
        {
            int preHitCount = curHitCount; // 현재 카운트를 저장 해둔다

            yield return new WaitForSeconds(delayTime); // 딜레이를 준다

            if (preHitCount == curHitCount) // 딜레이 주기 전에 저장해둔 카운트와, 현재 카운트를 비교한다
            {
                isCurHit = false;
            }
            else // 맞았다
            {
                isCurHit = true;
            }
        }

        
    }
}
