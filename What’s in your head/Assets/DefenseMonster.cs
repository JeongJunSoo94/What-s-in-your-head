using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using JCW.Spawner;

namespace KSU
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

        protected Rigidbody monsterRigidBody;
        protected NavMeshAgent monsterNavAgent;
        protected Animator monsterAnimator;

        protected Spawner spawner;

        bool isDefenseMode = false;

        // Start is called before the first frame update
        void Start()
        {
            monsterRigidBody = GetComponent<Rigidbody>();
            monsterNavAgent = GetComponent<NavMeshAgent>();
            spawner = GetComponentInParent<Spawner>();
            monsterAnimator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            InitMonster();
        }

        void InitMonster()
        {
            currentHP = maxHP;
            detectedTarget = null;
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

        protected void Chase()
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

            monsterNavAgent.destination = currentTarget.transform.position;
        }

        protected void Detect()
        {
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