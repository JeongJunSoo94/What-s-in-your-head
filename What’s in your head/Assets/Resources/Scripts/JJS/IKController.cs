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

    public bool enableIK;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (enableIK)
        {
            SetLookAt();
        }
    }

    public virtual void SetLookAt()
    {
        animator.SetLookAtWeight(1);
        animator.SetLookAtPosition(hitpos.position);

        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, posWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rotWeight);

        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandFollowObj.position);
       
        Transform rightHandTransform = animator.GetBoneTransform(HumanBodyBones.RightHand);
        Quaternion handRotation = Quaternion.LookRotation(hitpos.position - rightHandFollowObj.position, rightHandTransform.forward*-1);
        animator.SetIKRotation(AvatarIKGoal.RightHand, handRotation);

        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, posWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, rotWeight);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandFollowObj.position);

        //Transform leftHandTransform = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        //animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTransform.position);

    }
}
