using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JJS.CharacterSMB;

namespace KSU.FSM
{
    public class RopeSMB : CharacterBaseSMB
    {
        RopeAction playerRopeAction;
        public RopeAction GetRopeAction(Animator animator)
        {
            if (playerRopeAction == null)
            {
                playerRopeAction = animator.gameObject.GetComponent<RopeAction>();
            }
            return playerRopeAction;
        }
    }
}
