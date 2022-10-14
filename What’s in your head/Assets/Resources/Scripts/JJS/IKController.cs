using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKController : MonoBehaviour
{
    [Range(0, 1)]
    public float posWeight = 1;
    [Range(0, 1)]
    public float rotWeight = 1;

    public Transform leftHandFollowObj;
    public Transform rightHandFollowObj;
    public Transform hitpos;



    [SerializeField]
    protected Animator animator;

    private float selectWeight = 1;
    [Range(0, 356)]
    public float xRot;
    [Range(0, 356)]
    public float yRot;
    [Range(0, 356)]
    public float zRot;

    public GameObject target;

    PlayerController3D player;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerController3D>();
    }


    private void OnAnimatorIK(int layerIndex)
    {
        if (player.characterState.aim)
        {
            SetLookAt();
        }
    }

    void SetLookAt()
    {
        animator.SetLookAtWeight(1);
        animator.SetLookAtPosition(hitpos.position);

        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, posWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rotWeight);

        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandFollowObj.position);
        Quaternion handRotation = Quaternion.LookRotation(rightHandFollowObj.position - transform.position);
        //Quaternion handRotation = Quaternion.Euler(xRot, yRot, zRot);
        //animator.SetIKRotation(AvatarIKGoal.RightHand, handRotation);
        animator.SetBoneLocalRotation(HumanBodyBones.RightHand, handRotation);


        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, posWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, rotWeight);

        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandFollowObj.position);
        handRotation = Quaternion.LookRotation(leftHandFollowObj.position - transform.position);
        //animator.SetIKRotation(AvatarIKGoal.LeftHand, handRotation);
        animator.SetBoneLocalRotation(HumanBodyBones.LeftHand, handRotation);

    }
}
