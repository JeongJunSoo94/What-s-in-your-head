using JJS.CharacterSMB;
using KSU.AutoAim.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.FSM
{
    public class SteadyCymbalsSMB : CharacterBaseSMB
    {
        SteadyCymbalsAction cymbalsAction;
        public SteadyCymbalsAction GetGrappleAction(Animator animator)
        {
            if (cymbalsAction == null)
            {
                cymbalsAction = animator.gameObject.GetComponent<SteadyCymbalsAction>();
            }
            return cymbalsAction;
        }
    }
}
