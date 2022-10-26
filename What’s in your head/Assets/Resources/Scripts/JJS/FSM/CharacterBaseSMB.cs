using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KSU;
using JCW.UI.InGame;

namespace JJS.CharacterSMB
{
    public class CharacterBaseSMB : StateMachineBehaviour
    {
        PlayerController player;
        public PlayerController GetPlayerController(Animator animator)
        {
            if (player == null)
            {
                player = animator.gameObject.GetComponent<PlayerController>();
            }
            return player;
        }
    }

}
