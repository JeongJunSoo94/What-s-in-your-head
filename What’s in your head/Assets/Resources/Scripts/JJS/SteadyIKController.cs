using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JJS
{
    public class SteadyIKController : IKController
    {
        public GameObject ray;
        public override void SetLookAt()
        {
            animator.SetLookAtWeight(1);
            animator.SetLookAtPosition(hitpos.position);

            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, posWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rotWeight);

            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandFollowObj.position);

            Transform rightHandTransform = animator.GetBoneTransform(HumanBodyBones.RightHand);
            Vector3 rightHandPos = hitpos.position - rightHandFollowObj.position;
            Quaternion handRotation = Quaternion.LookRotation(rightHandPos.normalized, rightHandTransform.forward * -1);
            animator.SetIKRotation(AvatarIKGoal.RightHand, handRotation);


            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, posWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, rotWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandFollowObj.position);
            //Transform leftHandTransform = animator.GetBoneTransform(HumanBodyBones.LeftHand);

        }
    }

}
