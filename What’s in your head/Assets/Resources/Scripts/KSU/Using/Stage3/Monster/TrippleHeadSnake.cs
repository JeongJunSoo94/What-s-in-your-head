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

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Detect();
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
            monsterAnimator.SetBool("isTargetOn", false);
            monsterAnimator.SetBool("isReadyToRush", false);
            monsterAnimator.SetBool("isRushing", false);
        }

        public void SetRushTaget()
        {
            rushTarget = currentTarget;
            StartCoroutine(nameof(DelayReadyToRush));
        }

        IEnumerator DelayReadyToRush()
        {
            monsterAnimator.SetBool("isReadyToRush", true);
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
        }

        IEnumerator DelayRush()
        {
            monsterAnimator.SetBool("isRushing", true);
            yield return new WaitForSeconds(rushTime);
            monsterAnimator.SetBool("isRushing", false);
        }

        public void Rush()
        {
            rushTarget = null;
            monsterNavAgent.destination = (transform.position + transform.forward);
        }
    }
}
