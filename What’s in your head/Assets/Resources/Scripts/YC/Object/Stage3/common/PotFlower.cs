using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary> 
/// 
/// Current Issue       : ���� ���� ������ ���� �ʿ�
/// 
/// Object Name         : ȭ�� �ɺ�����
/// 
/// Object Description  : �����ð����� �ڶ��� ������ �°� �ȴٸ�, ���� �ǰ�, ���׵��� ���� ��ų�� ��ȣ�ۿ��� �����ϴ�.
/// 
/// Script Description  :
///                         
/// Player Intraction   : �ڶ�(���� ��ų)
/// 
/// </summary>
/// 

namespace YC_OBJ
{
    public class PotFlower : MonoBehaviour
    {
        [Header("<��ȹ ���� ����>")]
        [Header("[��Ʈ �ð� (�ش� �ð����� ������ ���� ��� ���� �ǿ�� �ȴ�)]")]
        [SerializeField] float MaxTime = 2.0f;
        public float maxTime  { get; private set; } // �ִ� �ð� ������


        public float curTime { get; private set; } = 0; // ���� �ð� ������
        bool isTrigger = false;

        
        bool isCurHit = false;
        int curHitCount = 0;
        float delayTime = 0.2f; // �Ѿ��� ���� �°� �ִ����� �� �� ���� ���� ������ (�Ѿ� �߻� �ӵ��� ����)

        string interactionObjTag = "NellaWater";

        Animator animator;
        PhotonView pv;

        private void Awake()
        {
            maxTime = MaxTime;

            animator = this.gameObject.GetComponent<Animator>();

            pv = this.gameObject.GetComponent<PhotonView>();
        }

        void Update()
        {
            if(!isTrigger) SetCurTime();
        }

        void SetCurTime()
        {
            if(isCurHit)
            {
                if (curTime > maxTime) return;

                curTime += Time.deltaTime;

                if (curTime > maxTime)
                {
                    curTime = maxTime;

                    if (pv.IsMine)
                        animator.SetBool("isBoom", true);

                    isTrigger = true;
                }

            }
            else
            {
                if (curTime < 0) return;

                curTime -= Time.deltaTime;

                if (curTime < 0)
                    curTime = 0;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Ʈ���� üũ!");
            if(other.gameObject.CompareTag(interactionObjTag))
            {
                Debug.Log("�±� üũ!");
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
