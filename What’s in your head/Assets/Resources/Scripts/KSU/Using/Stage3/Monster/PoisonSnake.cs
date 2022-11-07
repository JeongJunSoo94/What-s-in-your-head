using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.AutoAim.Object.Monster
{
    public class PoisonSnake : DefenseMonster
    {
        // Update is called once per frame
        void Update()
        {
            Detect();
        }

        private void OnEnable()
        {
            InitMonster();
        }
    }
}
