using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.AutoAim.Object.Monster.FSM
{
    public class MonsterSMB : StateMachineBehaviour
    {
        DefenseMonster monsterController;
        
        protected DefenseMonster GetMonsterController(Animator animator)
        {
            if(monsterController == null)
            {
                monsterController = animator.GetComponent<DefenseMonster>();
            }
            return monsterController;
        }
    }
}
