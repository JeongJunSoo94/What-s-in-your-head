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

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    selectWeight = 1;
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    selectWeight = 2;
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    selectWeight = 3;
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha4))
        //{
        //    selectWeight = 4;
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha5))
        //{
        //    selectWeight = 5;
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha6))
        //{
        //    selectWeight = 6;
        //}
    }

    private void OnAnimatorIK(int layerIndex)
    {
        SetLookAt();
        //if (animator)
        //{
        //    if (rightHandFollowObj != null)
        //    {
        //        switch (selectWeight)
        //        {
        //            case 1:
        //                SetPositionWeithgt();
        //                break;
        //            case 2:
        //                SetRotationWeithgt();
        //                break;
        //            case 3:
        //                SetEachWeight();
        //                break;
        //            case 4:
        //                SetRotationAngle();
        //                break;
        //            case 5:
        //                SetLookAtObj();
        //                break;
        //            case 6:
        //                SetLegWeight();
        //                break;
        //        }
        //    }
        //}
    }

    //void SetPositionWeithgt()//포지션 변경
    //{
    //    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, posWeight);
    //    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0.0f);

    //    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandFollowObj.position);
    //    Quaternion handRotation = Quaternion.LookRotation(rightHandFollowObj.position - transform.position);
    //    animator.SetIKRotation(AvatarIKGoal.RightHand, handRotation);
    //}

    //void SetRotationWeithgt()//손 회전
    //{
    //    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0.0f);
    //    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rotWeight);

    //    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandFollowObj.position);
    //    Quaternion handRotation = Quaternion.LookRotation(rightHandFollowObj.position - transform.position);
    //    animator.SetIKRotation(AvatarIKGoal.RightHand, handRotation);
    //}
    //void SetEachWeight()//손 회전과 포지션이동
    //{
    //    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, posWeight);
    //    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rotWeight);

    //    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandFollowObj.position);
    //    Quaternion handRotation = Quaternion.LookRotation(rightHandFollowObj.position - transform.position);
    //    animator.SetIKRotation(AvatarIKGoal.RightHand, handRotation);
    //}

    //private void SetRotationAngle()//손 고정에 팔이동
    //{
    //    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, posWeight);
    //    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rotWeight);

    //    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandFollowObj.position);
    //    Quaternion handRotation = Quaternion.Euler(xRot, yRot, zRot);
    //    animator.SetIKRotation(AvatarIKGoal.RightHand, handRotation);
    //}

    //private void SetLookAtObj()//화면 봄
    //{
    //    animator.SetLookAtWeight(1);
    //    animator.SetLookAtPosition(rightHandFollowObj.position);
    //}

    //private void SetLegWeight()//발 움직임
    //{
    //    animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, posWeight);
    //    animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0.0f);

    //    animator.SetIKPosition(AvatarIKGoal.RightFoot, rightHandFollowObj.position);
    //    Quaternion handRotation = Quaternion.LookRotation(rightHandFollowObj.position - transform.position);
    //    animator.SetIKRotation(AvatarIKGoal.RightFoot, handRotation);
    //}

    void SetLookAt()
    {
        animator.SetLookAtWeight(1);
        animator.SetLookAtPosition(hitpos.position);

        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, posWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rotWeight);

        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandFollowObj.position);
        Quaternion handRotation = Quaternion.LookRotation(rightHandFollowObj.position - transform.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, handRotation);

        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, posWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, rotWeight);

        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandFollowObj.position);
        handRotation = Quaternion.LookRotation(leftHandFollowObj.position - transform.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, handRotation);
        //animator.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.LookRotation(target.transform.position - transform.position));//Quaternion.Euler(xRot, yRot, zRot));

    }
}
