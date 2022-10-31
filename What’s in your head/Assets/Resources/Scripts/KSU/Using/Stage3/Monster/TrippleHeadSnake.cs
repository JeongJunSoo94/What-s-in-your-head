using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.Monster
{
    public class TrippleHeadSnake : DefenseMonster
    {
        [SerializeField] float readyToRushTime = 2f;
        [SerializeField] float rushTime = 2f;
        [SerializeField] float rushSpeed;
        [SerializeField] float rotationSpeed;
        GameObject rushTarget;
        [SerializeField] GameObject rushTrigger;
        [SerializeField] int rushDamage;

        bool isRushDelayOn = false;
        public float rushDelayTime = 10f;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Detect();
        }

        private void OnEnable()
        {
            InitMonster();
        }

        protected override void InitMonster()
        {
            currentHP = maxHP;
            detectedTarget = null;
            currentTarget = null;
            monsterNavAgent.speed = moveSpeed;
            monsterAnimator.SetBool("isAttacked", false);
            monsterAnimator.SetBool("isDead", false);
            monsterAnimator.SetBool("isSturn", false);
            monsterAnimator.SetBool("isChasing", false);
            monsterAnimator.SetBool("isAttacking", false);
            monsterAnimator.SetBool("isReadyToRush", false);
            monsterAnimator.SetBool("isRushing", false);
            isRushDelayOn = false;
        }

        public override void GetDamage(int damage)
        {

            if(monsterAnimator.GetBool("isSturn"))
            {
                currentHP -= damage;
                monsterAnimator.SetBool("isAttacked", true);
                if (currentHP < 0)
                {
                    monsterAnimator.SetBool("isDead", true);
                }
            }
        }

        public bool IsReadyToRush()
        {
            if(!isRushDelayOn)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetRushTaget()
        {
            rushTarget = currentTarget;
            StartCoroutine(nameof(MaintainReadyToRush));
        }

        IEnumerator MaintainReadyToRush()
        {
            yield return new WaitForSeconds(readyToRushTime);
            monsterAnimator.SetBool("isReadyToRush", false);
        }

        public void RotateForRush()
        {
            Vector3 direction = (rushTarget.transform.position - transform.position);
            Vector3 forward = Vector3.Slerp(transform.forward, direction.normalized, rotationSpeed * Time.deltaTime / Vector3.Angle(transform.forward, direction.normalized));
            forward.y = 0;
            transform.LookAt(transform.position + forward);
        }
        public void StartRush()
        {
            StartCoroutine(nameof(DelayRush));
            StartCoroutine(nameof(MaintainRush));
            ActivateRush();
            monsterNavAgent.speed = rushSpeed;
            monsterNavAgent.enabled = true;
            Rush();
        }

        IEnumerator MaintainRush()
        {
            monsterAnimator.SetBool("isRushing", true);
            yield return new WaitForSeconds(rushTime);
            monsterAnimator.SetBool("isRushing", false);
        }

        IEnumerator DelayRush()
        {
            isRushDelayOn = true;
            yield return new WaitForSeconds(rushDelayTime);
            isRushDelayOn = false;
        }

        public void Rush()
        {
            monsterNavAgent.destination = (transform.position + transform.forward * 10f);
        }

        void ActivateRush()
        {
            rushTrigger.SetActive(true);
        }

        public void EndRush()
        {
            rushTarget = null;
            monsterNavAgent.destination = transform.position;
            monsterNavAgent.enabled = false;
            monsterNavAgent.speed = moveSpeed;
            rushTrigger.SetActive(false);
            
        }
    }
}
