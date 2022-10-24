using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.Monster
{
    public class TrippleHeadSnake : DefenseMonster
    {
        public enum state { Idle, Chase, Stuck, Sturn, Rush, Dead }

        public state currentState = state.Idle;

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

        public void SetRushTaget()
        {
            rushTarget = currentTarget;
        }

        public void RotateForRush()
        {
            // 十君覗 馬切
            Vector3 direction = (rushTarget.transform.position - transform.position);
            Vector3 forward = Vector3.Slerp(transform.forward, direction.normalized, rotationSpeed * Time.deltaTime / Vector3.Angle(transform.forward, direction.normalized));
            forward.y = 0;
            transform.LookAt(transform.position + forward);
        }

        public void Rush()
        {
            rushTarget = null;
            monsterNavAgent.destination = (transform.position + transform.forward);
        }
    }
}
