using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using JCW.Spawner;

namespace KSU.Monster
{
    public class DefenseMonster : MonoBehaviour
    {
        [SerializeField] protected int maxHP;
        public int currentHP;
        public float attackRange = 1f;
        public int attackDamage;
        [SerializeField] LayerMask chaseLayerFiter;
        [SerializeField] protected GameObject targetObject;
        protected GameObject detectedTarget;
        protected GameObject currentTarget;
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

        protected Spawner spawner;

        public bool isAttackDelayOn = false;
        public float attackDelayTime = 2f;
        [SerializeField] protected GameObject attackTrigger;
        [SerializeField] protected GameObject detectingUITrigger;

        public bool isTargetFounded = false;


        // Start is called before the first frame update
        void Awake()
        {
            //monsterRigidBody = GetComponent<Rigidbody>();
            monsterNavAgent = GetComponent<NavMeshAgent>();
            spawner = GetComponentInParent<Spawner>();
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
            if(!monsterAnimator.GetBool("isAttacked") && !monsterAnimator.GetBool("isDead"))
            {
                currentHP -= damage;
                monsterAnimator.SetBool("isAttacked", true);
                if (currentHP <= 0)
                {
                    monsterAnimator.SetBool("isDead", true);
                }
            }
        }


        public void GetStun()
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
            spawner.Despawn(this.gameObject);
        }

        public void Chase()
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

        public void StopChasing()
        {
            monsterNavAgent.enabled = false;
        }

        public void StartChasing()
        {
            monsterNavAgent.enabled = true;
        }

        protected void Detect()
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
                            detectedTarget = collider.gameObject;
                            monsterAnimator.SetBool("isChasing", true);
                        }
                    }
                }
            }
            else if (GameManager.Instance.isTopView)
            {
                detectedTarget = null;
                monsterAnimator.SetBool("isChasing", true);
            }
        }

        public bool IsReadyToAttck()
        {
            if(!isAttackDelayOn && (attackRange > Vector3.Distance(transform.position, currentTarget.transform.position)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void StartAttack()
        {
            StartCoroutine(nameof(DelayAttack));
        }

        IEnumerator DelayAttack()
        {
            isAttackDelayOn = true;
            yield return new WaitForSeconds(attackDelayTime);
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