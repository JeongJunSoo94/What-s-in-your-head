using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// 
/// Current Issue       : �ִϸ��̼� �ʹ� ������ ���� �ʿ�
/// 
/// Object Name         : ����ũ Ʈ��
/// 
/// 
/// Object Description  : ���׵� ���� ������ ������ ������ �߰��ϸ�, �ڶ� ��ȭ�Ͽ� ���������� ���ش�.
///                         �ش� ������Ʈ�� ������ ���� ������Ʈ�� ��������� ����Ʈ�� �߰��ǰ�, ����Ʈ�� ��� ���� ������Ʈ�� ��ȭ�Ǹ� ���׵� �ǳ� �� �ִ�.
///                         ��ȭ���� ���� ä �÷��̾ �����ϸ� �÷��̾�� �״´�
/// 
/// Script Description  : �� ������Ʈ�� �ڽ� ������Ʈ�� ������ ������ ������ ���� �ִ�. �� ������Ʈ�� OnTriggerEnter(Collider other)�� �۵��ϸ�
///                         ������ ������ Destroy() �Ѵ�. (cf. �ڽĿ�����Ʈ�� �浹 üũ�� �θ������Ʈ���� �ϱ� ����, �θ� ������Ʈ�� RigidBody�� �־��ش�.)
///                         ���� ��ȭ���� �˷��ִ� isPure ������ true�� �ȴ�.
///                         
///                         isPure�� true �Ǹ� ���׵� ������ ������ �ǳ� �� �ֵ��� �ݶ��̴��� ����� ������Ʈ�� �°� �����Ѵ�
///                         (�������� ������ �� ũ�Ⱑ �ݶ��̴�)
///                         
/// Player Intraction   : �ڶ�(���� ��ų - ��ȭ)
/// 
/// </summary>

namespace YC_OBJ
{
    public class FakeTree : MonoBehaviour
    {     
        [Header("<��ȹ ���� ����>")]
        [Space]

        [Header("[��Ʈ �ð� (�ش� �ð����� ������ ���� ��� ��ȭ�� �̷������)]")]
        [SerializeField] float MaxTime = 5.0f;
        public float maxTime { get; private set; } // �ִ� �ð� ������

        [Header("[��ȭ ���� �ð� (��ȭ�� �Ϸ�� ��, �ش� �ð��� ������ �ٽ� �����ȴ�)]")]
        [SerializeField] float maxPureTime = 5;

        [Header("[�ִϸ��̼� ����ӵ�]")]
        [SerializeField] float animationSpeed = 1f;


        public float curTime { get; private set; } = 0; // ���� �ð� ������
        bool isCurHit = false;
        bool isPrevHit = false;
        int curHitCount = 0;
        float delayTime = 0.3f; // �Ѿ��� ���� �°� �ִ����� �� �� ���� ���� ������ (�Ѿ� �߻� �ӵ��� ����)

        string interactionObjTag = "NellaWater";


        public bool isPure { get; private set;}

        float curPureTime;

        PhotonView pv;

        Animator animator2; // �ϴ� �������� �ִϸ�����

        void Awake()
        {
            isPure = false;

            maxTime = MaxTime - animationSpeed * 2;

            maxPureTime += animationSpeed * 2;

            animator2 = transform.GetChild(0).GetComponent<Animator>();
            animator2.speed = animationSpeed;

            pv = GetComponent<PhotonView>();
        }

        void Start() { } // ������Ʈ �󿡼� ��ũ��Ʈ Ȱ��ȭ ���ؼ�

        void Update()
        {
            if (isPure)
            {
                curPureTime += Time.deltaTime;

                if (curPureTime > maxPureTime)
                {
                    if (pv.IsMine)
                    {
                        animator2.SetBool("isCreate", true);
                        animator2.SetBool("isDestroy", false);

                        pv.RPC(nameof(SetState_RPC), RpcTarget.AllViaServer, false);
                    }
                }
            }
            else
            {

                SetCurTime();

            }
        }

        // ��ȭ ���ο� �׿� ���� ���Ϳ� ������, ���������� Ȱ��ȭ ���θ� �����Ѵ�
        [PunRPC]
        void SetState_RPC(bool pure)
        {
            isPure = pure;

            if (pure) // ���� -> ��ȭ
            {
                
                curTime = 0;              
            }
            else // ��ȭ -> ����
            {
                
                curPureTime = 0;
             
            }
        }
   
        void SetCurTime()
        {
            if (isCurHit)
            {
                curTime += Time.deltaTime;

                if (curTime > maxTime)
                {
                    if (pv.IsMine)
                    {

                        animator2.SetBool("isDestroy", true);
                        animator2.SetBool("isCreate", false);

                        pv.RPC(nameof(SetState_RPC), RpcTarget.AllViaServer, true);
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(interactionObjTag))
            {
                curHitCount++;

                StartCoroutine(nameof(SetPrevHit));
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


    }
}

