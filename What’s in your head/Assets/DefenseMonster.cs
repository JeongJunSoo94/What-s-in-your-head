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
        protected int currentHP;
        [SerializeField] protected int attackPower;
        [SerializeField] protected GameObject targetObject;
        protected GameObject detectedTarget;
        protected GameObject currentTarget;
        [SerializeField] protected float detectingRange;
        public float moveSpeed; //시리얼
        //public float stuckTime; //시리얼
        public float sturnTime; //시리얼
        public float ropeRadius = 1f;
        public int ropeVertexes = 50;

        protected Collider[] detectedColliders;

        protected NavMeshAgent monsterNavAgent;
        protected Animator monsterAnimator;
        protected LineRenderer monsterRope;

        protected Spawner spawner;

        public bool isTargetFounded = false;

        bool isDefenseMode = false;

        // Start is called before the first frame update
        void Awake()
        {
            //monsterRigidBody = GetComponent<Rigidbody>();
            monsterNavAgent = GetComponent<NavMeshAgent>();
            spawner = GetComponentInParent<Spawner>();
            monsterAnimator = GetComponent<Animator>();
            monsterRope = GetComponent<LineRenderer>();
            InitRope();
            //isDefenseMode = GameManger 에서 받기
        }

        private void OnEnable()
        {
            InitMonster();
        }

        protected virtual void InitMonster()
        {
            currentHP = maxHP;
            detectedTarget = null;
            currentTarget = null;
            monsterNavAgent.speed = moveSpeed;
            monsterAnimator.SetBool("isAttacked", false);
            monsterAnimator.SetBool("isDead", false);
            monsterAnimator.SetBool("isSturn", false);
            monsterAnimator.SetBool("isChasing", false);
            monsterAnimator.SetBool("isTargetOn", false);
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

        public void GetDamage(int damage)
        {
            currentHP -= damage;
            monsterAnimator.SetBool("isAttacked", true);
            if (currentHP < 0)
            {
                monsterAnimator.SetBool("isDead", true);
            }
        }


        public void GetSturn()
        {
            StartCoroutine(nameof(DelaySturn));
        }

        IEnumerator DelaySturn()
        {
            monsterAnimator.SetBool("isSturn", true);
            yield return new WaitForSeconds(sturnTime);
            monsterAnimator.SetBool("isSturn", false);
        }

        public void Dead()
        {
            StopAllCoroutines();
            spawner.Despawn(this.gameObject);
        }

        public void Chase()
        {
            if (detectedTarget != null)
            {
                if (currentTarget != detectedTarget)
                {
                    currentTarget = detectedTarget;
                }
            }
            else if (isDefenseMode)
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
                    if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                    {
                        detectedTarget = collider.gameObject;
                        monsterAnimator.SetBool("isChasing", true);
                    }
                }
            }
            else if (isDefenseMode)
            {
                detectedTarget = null;
                monsterAnimator.SetBool("isChasing", true);
            }
        }
    }
}