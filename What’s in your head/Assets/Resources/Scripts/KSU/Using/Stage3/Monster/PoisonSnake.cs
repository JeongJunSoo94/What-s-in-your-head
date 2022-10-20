using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU
{
    public class PoisonSnake : DefenseMonster
    {
        public enum state { Idle, Chase, Stuck, Sturn, Dead}

        public state currentState = state.Idle;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
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
}
