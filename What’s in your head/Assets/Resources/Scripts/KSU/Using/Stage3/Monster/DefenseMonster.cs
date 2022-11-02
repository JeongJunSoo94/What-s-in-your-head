using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using JCW.Spawner;
using Photon.Pun;

namespace KSU.Monster
{
    public class DefenseMonster : MonoBehaviour
    {
        protected PhotonView pv;

        [SerializeField] protected int maxHP;
        public int currentHP;
        public float attackRange = 1f;
        public int attackDamage;
        [SerializeField] LayerMask chaseLayerFiter;
        [SerializeField] protected Transform targetObject;
        protected Transform detectedTarget;
        protected Transform currentTarget;
        [SerializeField] protected float detectingRange;
        public float detectingUIRange;
        public float moveSpeed; //시리얼
        //public float stuckTime; //시리얼
        public float stunTime;
        public float ropeRadius = 1f;
        public int ropeVertexes = 50;

        protected Collider[] detectedColliders;

        protected NavMeshAgent monsterNavAgent;
        protected Animator monsterAnimator;
        protected LineRenderer monsterRope;
        protected CapsuleCollider monsterCollider;

        protected MonsterSpawner spawner;

        public bool isAttackDelayOn = false;
        [Header("공격하기 전 딜레이 시간")] public float attackDelayTimeBefore = 2f;
        [Header("공격한 후 딜레이 시간")] public float attackDelayTimeAfter = 2f;
        [SerializeField] protected GameObject attackTrigger;
        [SerializeField] protected GameObject detectingUITrigger;

        public bool isTargetFounded = false;


        // Start is called before the first frame update
        void Awake()
        {
            pv = PhotonView.Get(this);
            monsterNavAgent = GetComponent<NavMeshAgent>();
            spawner = GetComponentInParent<MonsterSpawner>();
            monsterAnimator = GetComponent<Animator>();
            monsterRope = GetComponent<LineRenderer>();
            monsterCollider = GetComponent<CapsuleCollider>();
            detectingUITrigger.transform.localScale = Vector3.one * detectingUIRange * 2f;
            InitRope();
        }

        //private void OnDisable()
        //{
        //    InitMonster();
        //}

        protected virtual void InitMonster()
        {
            currentHP = maxHP;
            detectedTarget = null;
            currentTarget = null;
            monsterNavAgent.speed = moveSpeed;
            monsterAnimator.SetBool("isAttacked", false);
            monsterAnimator.SetBool("isDead", false);
            monsterAnimator.SetBool("isStunned", false);
            monsterAnimator.SetBool("WasStunned", false);
            monsterAnimator.SetBool("isChasing", false);
            monsterAnimator.SetBool("isAttacking", false);
            if(pv.IsMine)
                monsterNavAgent.enabled = true;
            monsterCollider.enabled = true;
            detectingUITrigger.SetActive(true);
        }

        void InitRope()
        {
            monsterRope.positionCount = ropeVertexes * 4 + 1;
            monsterRope.useWorldSpace = false;
            float x;
            float z;
            float y = 1f;
            float angle = 20f;
            for(int i = 0; i < (ropeVertexes * 4 + 1); ++i)
            {
                x = Mathf.Cos(Mathf.Deg2Rad * angle) * ropeRadius;
                z = Mathf.Sin(Mathf.Deg2Rad * angle) * ropeRadius;
                y -= 0.1f / ropeVertexes; 
                monsterRope.SetPosition(i, new Vector3(x, y, z));
                angle += (360f / ropeVertexes);
            }

        }

        public virtual void GetDamage(int damage)
        {
            if(pv.IsMine && !monsterAnimator.GetBool("isAttacked") && !monsterAnimator.GetBool("isDead"))
            {
                pv.RPC(nameof(SetDamage), RpcTarget.AllViaServer, damage);
            }
        }

        [PunRPC]
        protected void SetDamage(int damage)
        {
            currentHP -= damage;
            monsterAnimator.SetBool("isAttacked", true);
            if (currentHP <= 0)
            {
                monsterAnimator.SetBool("isDead", true);
            }
        }


        public void GetStun()
        {
            if (pv.IsMine && !monsterAnimator.GetBool("isAttacked") && !monsterAnimator.GetBool("isDead"))
            {
                pv.RPC(nameof(SetStun), RpcTarget.AllViaServer);
            }
        }

        [PunRPC]
        protected void SetStun()
        {
            monsterAnimator.SetBool("isStunned", true);
        }


        public void StartStun()
        {
            monsterRope.enabled = true;
            // 스턴 이펙트 켜기
        }

        public void EndStun()
        {
            monsterRope.enabled = false;
            monsterAnimator.SetBool("isStunned", false);
            monsterAnimator.SetBool("WasStunned", false);
            // 스턴 이펙트 끄기
        }

        public virtual void Dead()
        {
            StopAllCoroutines();
            attackTrigger.SetActive(false);
            detectingUITrigger.SetActive(false);
            monsterNavAgent.enabled = false;
            monsterRope.enabled = false;
            monsterCollider.enabled = false;
        }

        public void Disappear()
        {
            if(pv.IsMine)
            {
                pv.RPC(nameof(Despawn), RpcTarget.AllViaServer);
            }
        }

        [PunRPC]
        protected void Despawn()
        {
            if(spawner == null)
            {
                spawner = GetComponentInParent<MonsterSpawner>();
            }
            spawner.Despawn(this.gameObject);
        }

        public void Chase()
        {
            if(pv.IsMine)
            {
                if (!monsterNavAgent.enabled)
                    monsterNavAgent.enabled = true;

                if (detectedTarget != null)
                {
                    if (currentTarget != detectedTarget)
                    {
                        currentTarget = detectedTarget;
                    }
                }
                else if (GameManager.Instance.isTopView)
                {
                    currentTarget = targetObject;
                }

                if (currentTarget != null)
                {
                    monsterNavAgent.destination = currentTarget.transform.position;
                    isTargetFounded = true;
                }
                else
                {
                    isTargetFounded = false;
                }
            }
        }

        public void StopChasing()
        {
            if (pv.IsMine)
            {
                pv.RPC(nameof(SetChasing), RpcTarget.AllViaServer, false);
            }
        }

        public void StartChasing()
        {
            if (pv.IsMine)
            {
                pv.RPC(nameof(SetChasing), RpcTarget.AllViaServer, true);
            }
        }

        [PunRPC]
        protected void SetChasing(bool isChasing)
        {
            monsterNavAgent.enabled = isChasing;
        }

        protected void Detect()
        {
            if(pv.IsMine)
            {
                if (detectedTarget != null)
                    return;

                detectedColliders = Physics.OverlapSphere(transform.position, detectingRange);
                if (detectedColliders.Length > 0)
                {
                    foreach (var collider in detectedColliders)
                    {
                        RaycastHit hit;
                        bool rayChecked = Physics.Raycast(transform.position + Vector3.up * 0.5f, (collider.gameObject.transform.position - transform.position).normalized, out hit, detectingRange, chaseLayerFiter, QueryTriggerInteraction.Ignore);
                        if (rayChecked)
                        {
                            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                            {
                                detectedTarget = collider.gameObject.transform;
                                //detectedTarget = collider.gameObject;
                                monsterAnimator.SetBool("isChasing", true);
                            }
                        }
                    }
                }
                else if (GameManager.Instance.isTopView)
                {
                    detectedTarget = null;
                    //detectedTarget = null;
                    monsterAnimator.SetBool("isChasing", true);
                }
            }
        }

        public bool IsReadyToAttck()
        {
            Vector3 distVec = transform.position - currentTarget.transform.position;
            distVec.y = 0;
            if (!isAttackDelayOn && (attackRange > distVec.magnitude))
            {
                Debug.Log("어택 하니");
                return true;
            }
            else
            {
                Debug.Log("어택 안하니");
                return false;
            }
        }

        public void PrepareAttack()
        {
            StartCoroutine(nameof(DelayAttackBefore));
        }

        IEnumerator DelayAttackBefore()
        {
            yield return new WaitForSeconds(attackDelayTimeBefore);
            if(!monsterAnimator.GetBool("isStunned") || !monsterAnimator.GetBool("isAttacked"))
            {
                monsterAnimator.SetBool("isAttacking", true);
            }
        }

        public void StartAttack()
        {
            StartCoroutine(nameof(DelayAttackAfter));
        }

        IEnumerator DelayAttackAfter()
        {
            isAttackDelayOn = true;
            yield return new WaitForSeconds(attackDelayTimeAfter);
            isAttackDelayOn = false;
        }

        public void StartBiting()
        {
            attackTrigger.SetActive(true);
        }

        public void EndBiting()
        {
            attackTrigger.SetActive(false);
        }
    }
}