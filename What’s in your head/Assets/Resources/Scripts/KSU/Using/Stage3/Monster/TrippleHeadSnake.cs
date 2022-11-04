using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.Monster
{
    public class TrippleHeadSnake : DefenseMonster
    {
        [SerializeField] float readyToRushTime = 2f;
        [SerializeField] float rushTime = 2f;
        public float rushSpeed;
        [SerializeField] float rotationSpeed;
        Transform rushTarget;
        [SerializeField] GameObject rushTrigger;
        public int rushDamage;

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
            StopAllCoroutines();
            currentHP = maxHP;
            detectedTarget = null;
            currentTarget = null;
            monsterAnimator.SetBool("isAttacked", false);
            monsterAnimator.SetBool("isDead", false);
            monsterAnimator.SetBool("isStunned", false);
            monsterAnimator.SetBool("WasStunned", false);
            monsterAnimator.SetBool("isChasing", false);
            monsterAnimator.SetBool("isAttacking", false);
            monsterAnimator.SetBool("isReadyToRush", false);
            monsterAnimator.SetBool("isRushing", false);
            if (pv.IsMine)
            {
                monsterNavAgent.speed = moveSpeed;
                monsterNavAgent.enabled = true;
            }
            attackTrigger.SetActive(false);
            rushTrigger.SetActive(false);
            detectingUITrigger.SetActive(true);
            isRushDelayOn = false;
        }

        public override void GetDamage(int damage)
        {
            
            if (pv.IsMine && !monsterAnimator.GetBool("isAttacked") && !monsterAnimator.GetBool("isDead"))
            {
                if (monsterAnimator.GetBool("isStunned"))
                {
                    pv.RPC("SetDamage", RpcTarget.AllViaServer, damage);
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
            if (pv.IsMine)
            {
                rushTarget = currentTarget;
                StartCoroutine(nameof(MaintainReadyToRush));
            }
        }

        IEnumerator MaintainReadyToRush()
        {
            yield return new WaitForSeconds(readyToRushTime);
            monsterAnimator.SetBool("isReadyToRush", false);
        }

        public void RotateForRush()
        {
            if(pv.IsMine)
            {
                Vector3 direction = (rushTarget.transform.position - transform.position);
                Vector3 forward = Vector3.Slerp(transform.forward, direction.normalized, rotationSpeed * Time.deltaTime / Vector3.Angle(transform.forward, direction.normalized));
                forward.y = 0;
                transform.LookAt(transform.position + forward);
            }
        }
        public void StartRush()
        {
            ActivateRush();

            if (pv.IsMine)
            {
                StartCoroutine(nameof(DelayRush));
                StartCoroutine(nameof(MaintainRush));
                monsterNavAgent.speed = rushSpeed;
                monsterNavAgent.enabled = true;
                Rush();
            }
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
            if (pv.IsMine)
            {
                monsterNavAgent.destination = (transform.position + transform.forward * 10f);
            }
        }

        void ActivateRush()
        {
            rushTrigger.SetActive(true);
        }

        public void EndRush()
        {
            if (pv.IsMine)
            {
                rushTarget = null;
                monsterNavAgent.destination = transform.position;
                monsterNavAgent.enabled = false;
                monsterNavAgent.speed = moveSpeed;
            }
            rushTrigger.SetActive(false);
        }

        public override void Dead()
        {
            StopAllCoroutines();
            attackTrigger.SetActive(false);// 위 아래 둘다 필요 없을지도 그렇다면 가상함수도 필요 ㄴㄴ
            rushTrigger.SetActive(false); //
            detectingUITrigger.SetActive(false);
            monsterNavAgent.enabled = false;
            monsterRope.enabled = false;
            monsterCollider.enabled = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.collider.tag != "Nella" && collision.collider.tag != "Steady")
            {
                EndRush();
            }
        }
    }
}
