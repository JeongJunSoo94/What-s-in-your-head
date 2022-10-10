using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBaseSMB : StateMachineBehaviour
{
    PlayerController3D player;
    public PlayerController3D GetPlayerController3D(Animator animator)
    {
        if (player == null)
        {
            player = animator.gameObject.GetComponent<PlayerController3D>();
        }
        return player;
    }
}
