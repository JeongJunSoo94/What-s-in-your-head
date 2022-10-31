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


        public float curTime { get; private set; } = 0; // ���� �ð� ������
        bool isCurHit = false;
        int curHitCount = 0;
        float delayTime = 0.2f; // �Ѿ��� ���� �°� �ִ����� �� �� ���� ���� ������ (�Ѿ� �߻� �ӵ��� ����)


        string interactionObjTag = "NellaWater";

        float growTime;
        bool isGrowed = false;

        Animator animator;
        JCW.UI.InGame.Indicator.OneIndicator indicator;

        PhotonView pv;

        //string GrowedAniStateName = "Growed";
        //string LessingAniStateName = "Lessing";

        [SerializeField] LiftPlayer LiftObj;

        Vector3 tempPos = Vector3.zero;


        void Awake()
        {
            animator = GetComponent<Animator>();
            animator.speed = animationSpeed;
            indicator = transform.GetChild(3).GetComponent<JCW.UI.InGame.Indicator.OneIndicator>();  //�ε��� ��ȣ Ȯ��!

            maxTime = MaxTime;

            pv = this.gameObject.GetComponent<PhotonView>();

        }

        void Update()
        {
            if (isGrowed)
            {
                growTime += Time.deltaTime;

                if (growTime > maxGrowedTime)
                {                   
                    if (pv.IsMine)
                    {
                        //animator.SetBool("isLess", true);
                        //animator.SetBool("isGrow", false);
                        //indicator.gameObject.SetActive(true);
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

        bool CheckAnimation() // ���� ���� Ȥ�� �پ�� �ִϸ��̼��� ���� ������ üũ�Ѵ�
        {
            if (((animator.GetCurrentAnimatorStateInfo(0).IsName("Lessing") || animator.GetCurrentAnimatorStateInfo(0).IsName("Growed")) && 
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f))
            {
                //Debug.Log("������!");
                if(tempPos == Vector3.zero)
                {
                    tempPos = LiftObj.player.transform.localPosition;
                }
                return true;
            }
            else
            {
                //Debug.Log("������ �ƴ�!");
                return false;
            } 
        }

        // << :
        void BlockPlayerMove()
        {
            if (CheckAnimation())
                SetPlayerMovePossible(false);
            else
                SetPlayerMovePossible(true);
        }

        void SetPlayerMovePossible (bool can)
        {
            if (!LiftObj.player) return;

            if (can)
            {
                LiftObj.player.GetComponent<PhotonTransformView>().enabled = true;
                LiftObj.player.GetComponent<PlayerState>().isOutOfControl = false;

                //LiftObj.GetComponent<MeshCollider>().enabled = true;


            }
            else
            {
                LiftObj.player.GetComponent<PhotonTransformView>().enabled = false;
                LiftObj.player.GetComponent<PlayerState>().isOutOfControl = true;
                LiftObj.player.transform.localPosition = tempPos;

                //LiftObj.GetComponent<MeshCollider>().enabled = false;

                //LiftObj.player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
        // >> :

        void SetCurTime()
        {
            if (isCurHit)
            {
                curTime += Time.deltaTime;

                if (curTime > maxTime)
                {                  
                    if (pv.IsMine)
                    {
                        //animator.SetBool("isGrow", true);
                        //animator.SetBool("isLess", false);
                        //indicator.gameObject.SetActive(false);
                        pv.RPC(nameof(SetGrow_RPC), RpcTarget.AllViaServer);
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

            animator.SetBool("isGrow", true);
            animator.SetBool("isLess", false);
            indicator.gameObject.SetActive(false);

            curTime = maxTime;
            isGrowed = true;
            curTime = 0;
        }

        [PunRPC]
        public void SetLess_RPC()
        {
            animator.SetBool("isLess", true);
            animator.SetBool("isGrow", false);
            indicator.gameObject.SetActive(true);

            isGrowed = false;
            growTime = 0;
        }
    }
}


