using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU
{
    public class TrippleHeadSnake : DefenseMonster
    {
        public enum state { Idle, Chase, Stuck, Sturn, Rush, Dead }

        public state currentState = state.Idle;

        [SerializeField] float rushSpeed;
        GameObject rushTarget;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            void Update()
            {
                Detect();
                switch (currentState)
                {
                    case state.Chase:
                    case state.Stuck:
                    case state.Sturn:
                        Detect();
                        Chase();
                        break;
                }
            }
        }

        void SetTaget()
        {
             if(rushTarget == null)
            {
                rushTarget = currentTarget;
            }

        }

        void Rotate()
        {
            // 十君覗 馬切
            transform.LookAt(rushTarget.transform.position);
        }

        public void Rush()
        {
            rushTarget = null;
            monsterRigidBody.velocity = transform.forward * rushSpeed;
        }
    }
}
