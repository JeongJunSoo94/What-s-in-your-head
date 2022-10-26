using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JJS.CharacterSMB;
using JCW.UI.Options.InputBindings;

namespace KSU.FSM
{
    public class RailSMB : CharacterBaseSMB
    {
        RailAction railAction;
        public RailAction GetRailAction(Animator animator)
        {
            if (railAction == null)
            {
                railAction = animator.gameObject.GetComponent<RailAction>();
            }
            return railAction;
        }
    }
}
