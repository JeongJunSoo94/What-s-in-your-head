using JCW.UI.InGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using JCW.UI.InGame.Indicator;

/// <summary> 
/// 
/// Current Issue       : 
/// 
/// Object Name         : 화분 이파리
/// 
/// Object Description  : 넬라의 물총에 N초 이상 닿게 된다면(게이지가 모두 채워진다면) 해당 오브젝트의 크기가 서서히 커진다(성장, 애니메이션).
///                         게이지가 채우는 도중 플레이어가 스킬을 멈추면 게이지는 서서히 감소한다.
///                         물총과 상호작용하는 부분은 오브젝트 하단(땅) 부분이다.
///                         
///                         성장에는 단계(게이지)가 있으며 최종 단계에 진입 후, 일정 구간까지 성장하고, 
///                         일정 시간(라이프 타임)이 지나게 되면 다시 작아진다 (작아지는 중 스킬 상호작용X).
///                         
/// Script Description  : 
///                         
/// Player Intraction   : 넬라 (물총 스킬 - 급속 성장)
/// 
/// </summary>

namespace YC_OBJ
{
    public class LeafPlant : MonoBehaviour
    {

        [Header("<기획 편집 사항>")]
        [Space]

        [Header("[히트 시간 (해당 시간동안 물총을 맞을 경우 꽃이 피게 된다)]")]
        [SerializeField] float MaxTime = 2.0f;
        public float maxTime { get; private set; } // 최대 시간 게이지

        [Header("[성장 지속 시간 (성장이 완료된 후, 해당 시간이 지나면 작아진다)]")]
        [SerializeField] int maxGrowedTime = 5;

        [Header("[애니메이션 재생속도]")]
        [SerializeField] float animationSpeed = 1f;


        public float curTime { get; private set; } = 0; // 현재 시간 게이지
        bool isCurHit = false;
        int curHitCount = 0;
        float delayTime = 0.2f; // 총알을 현재 맞고 있는지를 몇 초 전과 비교할 것인지 (총알 발사 속도와 연관)


        string interactionObjTag = "NellaWater";

        float growTime;
        bool isGrowed = false;

        Animator animator;
        JCW.UI.InGame.Indicator.OneIndicator indicator;

        PhotonView pv;

        //string GrowedAniStateName = "Growed";
        //string LessingAniStateName = "Lessing";

        [SerializeField] LiftPlayer LiftObj;

        Vector3 tempPos = Vector3.zero;


        void Awake()
        {
            animator = GetComponent<Animator>();
            animator.speed = animationSpeed;
            indicator = transform.GetChild(3).GetComponent<JCW.UI.InGame.Indicator.OneIndicator>();  //인덱스 번호 확인!

            maxTime = MaxTime;

            pv = this.gameObject.GetComponent<PhotonView>();

        }

        void Update()
        {
            if (isGrowed)
            {
                growTime += Time.deltaTime;

                if (growTime > maxGrowedTime)
                {                   
                    if (pv.IsMine)
                    {
                        //animator.SetBool("isLess", true);
                        //animator.SetBool("isGrow", false);
                        //indicator.gameObject.SetActive(true);
                        pv.RPC(nameof(SetLess_RPC), RpcTarget.AllViaServer);
                    }
                }
            }
            else
            {
                SetCurTime();

                indicator.SetGauge(curTime / maxTime);
            }


            //BlockPlayerMove();

        }

        private void OnTriggerEnter(Collider other)
        {
            if (isGrowed) return;

            if (other.gameObject.CompareTag(interactionObjTag))
            {
                curHitCount++;

                StartCoroutine(nameof(SetPrevHit));
            }
        }

        bool CheckAnimation() // 현재 성장 혹은 줄어듦 애니메이션이 진행 중인지 체크한다
        {
            if (((animator.GetCurrentAnimatorStateInfo(0).IsName("Lessing") || animator.GetCurrentAnimatorStateInfo(0).IsName("Growed")) && 
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f))
            {
                //Debug.Log("진행중!");
                if(tempPos == Vector3.zero)
                {
                    tempPos = LiftObj.player.transform.localPosition;
                }
                return true;
            }
            else
            {
                //Debug.Log("진행중 아님!");
                return false;
            } 
        }

        // << :
        void BlockPlayerMove()
        {
            if (CheckAnimation())
                SetPlayerMovePossible(false);
            else
                SetPlayerMovePossible(true);
        }

        void SetPlayerMovePossible (bool can)
        {
            if (!LiftObj.player) return;

            if (can)
            {
                LiftObj.player.GetComponent<PhotonTransformView>().enabled = true;
                LiftObj.player.GetComponent<PlayerState>().isOutOfControl = false;

                //LiftObj.GetComponent<MeshCollider>().enabled = true;


            }
            else
            {
                LiftObj.player.GetComponent<PhotonTransformView>().enabled = false;
                LiftObj.player.GetComponent<PlayerState>().isOutOfControl = true;
                LiftObj.player.transform.localPosition = tempPos;

                //LiftObj.GetComponent<MeshCollider>().enabled = false;

                //LiftObj.player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
        // >> :

        void SetCurTime()
        {
            if (isCurHit)
            {
                curTime += Time.deltaTime;

                if (curTime > maxTime)
                {                  
                    if (pv.IsMine)
                    {
                        //animator.SetBool("isGrow", true);
                        //animator.SetBool("isLess", false);
                        //indicator.gameObject.SetActive(false);
                        pv.RPC(nameof(SetGrow_RPC), RpcTarget.AllViaServer);
                    }
                }

            }
            else
            {
                curTime -= Time.deltaTime;

                if (curTime < 0)
                    curTime = 0;
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

        [PunRPC]
        public void SetGrow_RPC()
        {

            animator.SetBool("isGrow", true);
            animator.SetBool("isLess", false);
            indicator.gameObject.SetActive(false);

            curTime = maxTime;
            isGrowed = true;
            curTime = 0;
        }

        [PunRPC]
        public void SetLess_RPC()
        {
            animator.SetBool("isLess", true);
            animator.SetBool("isGrow", false);
            indicator.gameObject.SetActive(true);

            isGrowed = false;
            growTime = 0;
        }
    }
}


