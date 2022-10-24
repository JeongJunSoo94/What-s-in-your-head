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
        public float stuckTime; //시리얼
        public float sturnTime; //시리얼

        protected Collider[] detectedColliders;

        protected NavMeshAgent monsterNavAgent;
        protected Animator monsterAnimator;

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

            //isDefenseMode = GameManger 에서 받기
        }

        private void OnEnable()
        {
            InitMonster();
        }

        void InitMonster()
        {
            currentHP = maxHP;
            detectedTarget = null;
            currentTarget = null;
            monsterNavAgent.speed = moveSpeed;
        }

        public void GetDamag(int damage)
        {
            currentHP -= damage;
            if (currentHP < 0)
            {
                monsterAnimator.SetBool("isDead", true);
            }
        }

        public void Dead()
        {
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
                    }
                }
            }
            else if (isDefenseMode)
            {
                detectedTarget = null;
            }
        }
    }
}