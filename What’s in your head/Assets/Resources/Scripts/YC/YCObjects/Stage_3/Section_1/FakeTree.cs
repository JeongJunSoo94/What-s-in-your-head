using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// 
/// Current Issue       : 애니메이션 초반 딜레이 수정 필요
/// 
/// Object Name         : 페이크 트리
/// 
/// 
/// Object Description  : 스테디가 높은 곳에서 오염된 나무를 발견하면, 넬라가 정화하여 오염물질을 없앤다.
///                         해당 오브젝트는 생성시 덩굴 오브젝트의 멤버변수인 리스트에 추가되고, 리스트의 모든 덩굴 오브젝트가 정화되면 스테디가 건널 수 있다.
///                         정화되지 않은 채 플레이어가 착지하면 플레이어는 죽는다
/// 
/// Script Description  : 이 오브젝트는 자식 오브젝트로 나무와 오염된 물질을 갖고 있다. 이 오브젝트가 OnTriggerEnter(Collider other)가 작동하면
///                         오염된 물질을 Destroy() 한다. (cf. 자식오브젝트의 충돌 체크를 부모오브젝트에서 하기 위해, 부모 오브젝트에 RigidBody를 넣어준다.)
///                         이후 정화됨을 알려주는 isPure 변수는 true가 된다.
///                         
///                         isPure가 true 되면 스테디가 오염된 나무를 건널 수 있도록 콜라이더의 사이즈가 오브젝트에 맞게 증가한다
///                         (이전에는 나무의 반 크기가 콜라이더)
///                         
/// Player Intraction   : 넬라(물총 스킬 - 정화)
/// 
/// </summary>

namespace YC_OBJ
{
    public class FakeTree : MonoBehaviour
    {     
        [Header("<기획 편집 사항>")]
        [Space]

        [Header("[히트 시간 (해당 시간동안 물총을 맞을 경우 정화가 이루어진다)]")]
        [SerializeField] float MaxTime = 5.0f;
        public float maxTime { get; private set; } // 최대 시간 게이지

        [Header("[정화 지속 시간 (정화가 완료된 후, 해당 시간이 지나면 다시 오염된다)]")]
        [SerializeField] float maxPureTime = 5;

        [Header("[애니메이션 재생속도]")]
        [SerializeField] float animationSpeed = 1f;


        public float curTime { get; private set; } = 0; // 현재 시간 게이지
        bool isCurHit = false;
        bool isPrevHit = false;
        int curHitCount = 0;
        float delayTime = 0.3f; // 총알을 현재 맞고 있는지를 몇 초 전과 비교할 것인지 (총알 발사 속도와 연관)

        string interactionObjTag = "NellaWater";


        public bool isPure { get; private set;}

        float curPureTime;

        PhotonView pv;

        Animator animator2; // 하단 오염물질 애니메이터

        void Awake()
        {
            isPure = false;

            maxTime = MaxTime - animationSpeed * 2;

            maxPureTime += animationSpeed * 2;

            animator2 = transform.GetChild(0).GetComponent<Animator>();
            animator2.speed = animationSpeed;

            pv = GetComponent<PhotonView>();
        }

        void Start() { } // 컴포넌트 상에서 스크립트 활성화 위해서

        void Update()
        {
            if (isPure)
            {
                curPureTime += Time.deltaTime;

                if (curPureTime > maxPureTime)
                {
                    if (pv.IsMine)
                    {
                        animator2.SetBool("isCreate", true);
                        animator2.SetBool("isDestroy", false);

                        pv.RPC(nameof(SetState_RPC), RpcTarget.AllViaServer, false);
                    }
                }
            }
            else
            {

                SetCurTime();

            }
        }

        // 정화 여부와 그에 따른 센터와 사이즈, 오염물질의 활성화 여부를 세팅한다
        [PunRPC]
        void SetState_RPC(bool pure)
        {
            isPure = pure;

            if (pure) // 오염 -> 정화
            {
                
                curTime = 0;              
            }
            else // 정화 -> 오염
            {
                
                curPureTime = 0;
             
            }
        }
   
        void SetCurTime()
        {
            if (isCurHit)
            {
                curTime += Time.deltaTime;

                if (curTime > maxTime)
                {
                    if (pv.IsMine)
                    {

                        animator2.SetBool("isDestroy", true);
                        animator2.SetBool("isCreate", false);

                        pv.RPC(nameof(SetState_RPC), RpcTarget.AllViaServer, true);
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(interactionObjTag))
            {
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

