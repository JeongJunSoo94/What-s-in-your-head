using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimViewMoveSMB : StateMachineBehaviour
{
    PlayerController3D player;
    CharacterState3D cs3d;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.transform.gameObject.GetComponent<PlayerController3D>();
        cs3d = animator.transform.gameObject.GetComponent<CharacterState3D>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.TopViewInputMove();
        animator.SetFloat("MoveX", player.moveDir.normalized.x * (cs3d.isMove ? 1.0f : 0.0f));
        animator.SetFloat("MoveZ", player.moveDir.normalized.z * (cs3d.isMove ? 1.0f : 0.0f));
        player.InputMove();
        player.InputRun();
        player.RotationAim();
       
    }

}
