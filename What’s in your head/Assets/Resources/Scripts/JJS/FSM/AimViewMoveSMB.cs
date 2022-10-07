using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimViewMoveSMB : StateMachineBehaviour
{
    PlayerController3D player;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.transform.gameObject.GetComponent<PlayerController3D>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.TopViewInputMove();
        animator.SetFloat("MoveX", player.moveDir.normalized.x * (player.characterState.isMove ? 1.0f : 0.0f));
        animator.SetFloat("MoveZ", player.moveDir.normalized.z * (player.characterState.isMove ? 1.0f : 0.0f));
        player.InputMove();
        player.InputRun();
        player.RotateAim();
       
    }

}
