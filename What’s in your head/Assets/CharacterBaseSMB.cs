using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
