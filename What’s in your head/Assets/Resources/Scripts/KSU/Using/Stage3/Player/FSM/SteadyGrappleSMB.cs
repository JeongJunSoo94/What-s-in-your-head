using JJS.CharacterSMB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.FSM
{
    public class SteadyGrappleSMB : CharacterBaseSMB
    {
        SteadyGrappleAction grappleAction;
        public SteadyGrappleAction GetGrappleAction(Animator animator)
        {
            if (grappleAction == null)
            {
                grappleAction = animator.gameObject.GetComponent<SteadyGrappleAction>();
            }
            return grappleAction;
        }
    }
}
