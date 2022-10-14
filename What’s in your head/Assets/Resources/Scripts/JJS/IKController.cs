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

        Transform rotation = animator.GetBoneTransform(HumanBodyBones.RightHand);
        Quaternion handRotation = Quaternion.LookRotation(hitpos.position- rightHandFollowObj.position, rotation.forward*-1);
        animator.SetIKRotation(AvatarIKGoal.RightHand, handRotation);
        //Debug.Log (rotation);
        //rotation = Quaternion.LookRotation(rotation.eulerAngles + new Vector3(0,90,0));

        //animator.SetIKRotation(AvatarIKGoal.RightHand, rotation);
        //Quaternion rotY = Quaternion.Euler(0, handRotation.y, 0);
        //Quaternion rotZ = Quaternion.Euler(0, 0, handRotation.z);
        //Quaternion handRotation = Quaternion.Euler(xRot, yRot, zRot);
    
        //animator.SetBoneLocalRotation(HumanBodyBones.RightHand, rotation);


        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, posWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, rotWeight);

        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandFollowObj.position);
        handRotation = Quaternion.LookRotation(leftHandFollowObj.position - transform.position);
        //animator.SetIKRotation(AvatarIKGoal.LeftHand, handRotation);
        animator.SetBoneLocalRotation(HumanBodyBones.LeftHand, handRotation);

    }
}
