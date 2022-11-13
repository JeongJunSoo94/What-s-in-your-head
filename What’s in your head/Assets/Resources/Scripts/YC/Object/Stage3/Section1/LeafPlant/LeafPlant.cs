using JCW.UI.InGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using JCW.UI.InGame.Indicator;

/// <summary> 
/// 
/// Current Issue       : 
/// 
/// Object Name         : ȭ�� ���ĸ�
/// 
/// Object Description  : �ڶ��� ���ѿ� N�� �̻� ��� �ȴٸ�(�������� ��� ä�����ٸ�) �ش� ������Ʈ�� ũ�Ⱑ ������ Ŀ����(����, �ִϸ��̼�).
///                         �������� ä��� ���� �÷��̾ ��ų�� ���߸� �������� ������ �����Ѵ�.
///                         ���Ѱ� ��ȣ�ۿ��ϴ� �κ��� ������Ʈ �ϴ�(��) �κ��̴�.
///                         
///                         ���忡�� �ܰ�(������)�� ������ ���� �ܰ迡 ���� ��, ���� �������� �����ϰ�, 
///                         ���� �ð�(������ Ÿ��)�� ������ �Ǹ� �ٽ� �۾����� (�۾����� �� ��ų ��ȣ�ۿ�X).
///                         
/// Script Description  : 
///                         
/// Player Intraction   : �ڶ� (���� ��ų - �޼� ����)
/// 
/// </summary>

namespace YC_OBJ
{
    public class LeafPlant : MonoBehaviour
    {
        [Header("<��ȹ ���� ����>")]
        [Space]

        [Header("[��Ʈ �ð� (�ش� �ð����� ������ ���� ��� ���� �ǰ� �ȴ�)]")]
        [SerializeField] float MaxTime = 2.0f;
        public float maxTime { get; private set; } // �ִ� �ð� ������

        [Header("[���� ���� �ð� (������ �Ϸ�� ��, �ش� �ð��� ������ �۾�����)]")]
        [SerializeField] int maxGrowedTime = 5;

        [Header("[�ִϸ��̼� ����ӵ�]")]
        [SerializeField] float animationSpeed = 1f;

        [Header("[�� �Ʒ� �����̴� �ӵ�]")]
        [SerializeField] float moveSpeed = 5f;

        public float curTime { get; private set; } = 0; // ���� �ð� ������
        bool isCurHit = false;
        int curHitCount = 0;
        float delayTime = 0.13f; // �Ѿ��� ���� �°� �ִ����� �� �� ���� ���� ������ (�Ѿ� �߻� �ӵ��� ����)


        string interactionObjTag = "NellaWater";

        float growTime;
        bool isGrowed = false;

        Animator animator;
        JCW.UI.InGame.Indicator.OneIndicator indicator;

        PhotonView pv;

        [SerializeField] LiftPlayer LiftObj;

        Vector3 tempPos = Vector3.zero;

        AnimatorStateInfo animatorStateInfo;

        [SerializeField] GameObject Bone;

        PhotonView bonePV;

        //bool isLerp = false;

        bool isMove = false;
        bool isUpDir = false;

        float height = 10;
        Vector3 upPos;
        Vector3 initPos;
        void Awake()
        {
            animator = GetComponent<Animator>();
            animator.speed = animationSpeed;
            indicator = transform.GetChild(3).GetComponent<JCW.UI.InGame.Indicator.OneIndicator>();  //�ε��� ��ȣ Ȯ��!

            maxTime = MaxTime;

            pv = this.gameObject.GetComponent<PhotonView>();
            bonePV = Bone.GetComponent<PhotonView>();

            animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);

            initPos  = Bone.transform.localPosition;
            upPos = new Vector3(initPos.x, initPos.y + height, initPos.z);
        }

        
        void FixedUpdate()
        {
            if (isGrowed)
            {
                growTime += Time.deltaTime;

                if (growTime > maxGrowedTime)
                {
                    if (pv.IsMine)
                    {
                        pv.RPC(nameof(SetLess_RPC), RpcTarget.AllViaServer);
                    }
                }

            }
            else
            {
                SetCurTime();

                indicator.SetGauge(curTime / maxTime);
            }
            //BlockPlayerMove();

            MoveLiftObj();
        }




        private void MoveLiftObj()
        {
            if (!isMove) return;

            Vector3 BonePos = Bone.transform.localPosition;

            if (isUpDir)
            {
                Bone.transform.localPosition = Vector3.MoveTowards(BonePos, upPos, Time.fixedDeltaTime * moveSpeed);

                if (Bone.transform.localPosition == upPos)
                    isMove = false;
            }
            else
            {
                Bone.transform.localPosition = Vector3.MoveTowards(BonePos, initPos, Time.fixedDeltaTime * moveSpeed);

                if (Bone.transform.localPosition == initPos)
                    isMove = false;
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            if (isGrowed) return;

            if (other.gameObject.CompareTag(interactionObjTag))
            {
                curHitCount++;

                StartCoroutine(nameof(SetPrevHit));
            }
        }

        //bool CheckAnimation() // ���� ���� Ȥ�� �پ�� �ִϸ��̼��� ���� ������ üũ�Ѵ�
        //{
        //    if (((animatorStateInfo.IsName("Lessing") || animatorStateInfo.IsName("Growed")) &&
        //        animatorStateInfo.normalizedTime < 1.0f))
        //    {
        //        //Debug.Log("������!");
        //        if(tempPos == Vector3.zero)
        //        {
        //            tempPos = LiftObj.player.transform.localPosition;
        //        }
        //        return true;
        //    }
        //    else
        //    {
        //        //Debug.Log("������ �ƴ�!");
        //        return false;
        //    } 
        //}

        //// << :
        //void BlockPlayerMove()
        //{
        //    //if (CheckAnimation())
        //    //    //SetPlayerMovePossible(false);
        //    //else
        //    //    //SetPlayerMovePossible(true);
        //}

        //public void SetPlayerMovePossible (int can)
        //{
        //    //if (!LiftObj.player) return;

        //    //if (can == 0)
        //    //{
        //    //    if(LiftObj.player.GetComponent<PhotonView>().IsMine)
        //    //    {
        //    //        //LiftObj.player.GetComponent<PhotonTransformView>().enabled = true;
        //    //        //LiftObj.player.GetComponent<PlayerState>().isOutOfControl = false;

        //    //        //LiftObj.GetComponent<MeshCollider>().enabled = true;
        //    //        //Debug.Log("�� ������!");
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    if (LiftObj.player.GetComponent<PhotonView>().IsMine)
        //    //    {
        //    //        //LiftObj.player.GetComponent<PhotonTransformView>().enabled = false;
        //    //        //LiftObj.player.GetComponent<PlayerState>().isOutOfControl = true;
        //    //        //LiftObj.player.transform.localPosition = tempPos;

        //    //        //Debug.Log("���� ��!");

        //    //        //LiftObj.GetComponent<MeshCollider>().enabled = false;
        //    //        //LiftObj.player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //    //    }
        //    //}
        //}

        void SetCurTime()
        {
            if (isGrowed) return;

            if (isCurHit)
            {
                curTime += Time.deltaTime;

                if (curTime > maxTime)
                {                                  
                    if (pv.IsMine)
                    {                    
                        pv.RPC(nameof(SetGrow_RPC), RpcTarget.AllViaServer);
                        Debug.Log("Plant �Լ� ȣ��!");
                    }
                }

            }
            else
            {
                curTime -= Time.deltaTime;

                if (curTime < 0)
                    curTime = 0;
            }
        }

        IEnumerator SetPrevHit()
        {
            int preHitCount = curHitCount; // ���� ī��Ʈ�� ���� �صд�

            yield return new WaitForSeconds(delayTime); // �����̸� �ش�

            if (preHitCount == curHitCount) // ������ �ֱ� ���� �����ص� ī��Ʈ��, ���� ī��Ʈ�� ���Ѵ�
            {
                isCurHit = false;
            }
            else // �¾Ҵ�
            {
                isCurHit = true;
            }
        }

        [PunRPC]
        public void SetGrow_RPC()
        {
            //animator.SetBool("isGrow", true);
            //animator.SetBool("isLess", false);

            if (bonePV.IsMine)
            {
                isMove = true;
                isUpDir = true;
                Debug.Log("Bone �Լ� ȣ��!");

            }

            indicator.gameObject.SetActive(false);

            curTime = maxTime;
            isGrowed = true;
            curTime = 0;
        }

        [PunRPC]
        public void SetLess_RPC()
        {
            if (bonePV.IsMine)
            {
                isMove = true;
                isUpDir = false;
                Debug.Log("Bone �Լ� ȣ��!");
            }

            animator.SetBool("isLess", true);
            animator.SetBool("isGrow", false);
            indicator.gameObject.SetActive(true);

            isGrowed = false;
            growTime = 0;
        }
    }
}


