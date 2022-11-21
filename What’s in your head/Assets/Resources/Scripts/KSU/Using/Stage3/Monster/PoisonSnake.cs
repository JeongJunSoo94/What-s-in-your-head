using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.AudioCtrl;

namespace KSU.AutoAim.Object.Monster
{
    public class PoisonSnake : DefenseMonster
    {
        // Update is called once per frame
        protected override void Awake()
        {
            base.Awake();           
        }

        void Update()
        {
            Detect();
        }

        protected override void OnEnable()
        {
            base.Awake();
            InitMonster();
        }
    }
}
