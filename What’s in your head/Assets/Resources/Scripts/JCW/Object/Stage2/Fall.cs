using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KSU.Object.Stage2;

namespace JCW.Object
{
    [RequireComponent(typeof(PhotonView))]
    public class Fall : LinkedObjectWithReciever
    {
        [Header("�߶� �ʱ� �ӵ�")] [SerializeField] float fallSpeed;
        [Header("�߶� ���ӵ�")] [SerializeField] float fallAccelerateSpeed;
        [Header("�ٷ� �Ʒ��� �÷���")] [SerializeField] Transform groundPlatform;
        //Transform groundPlatform;

        Vector3 finalPos;
        PhotonView pv;
        private void Awake()
        {
            if (!groundPlatform)
            {
                Debug.Log("�߶��� �÷����� �������� �ʾҽ��ϴ�.");
                Destroy(this.gameObject);
                return;
            }
            pv = PhotonView.Get(this);
            transform.GetChild(0).GetComponent<SandSackShadow>().groundPlatform = this.groundPlatform;
            finalPos = transform.position;
            finalPos.y = groundPlatform.position.y;
        }

        void Update()
        {
            if (!isActivated)
                return;
            transform.position = Vector3.MoveTowards(transform.position, finalPos, Time.deltaTime * fallSpeed);
            fallSpeed += Time.deltaTime * fallAccelerateSpeed;
        }

        private void OnTriggerEnter(Collider other)
        {
            //Debug.Log(other.gameObject.layer);

            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                //Debug.Log("���ָӴ� ĳ���� ��� ����");
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Platform"))
            {
                //Debug.Log("���ָӴ� ���� �߶�");

                //����� �߶� �� ������Ʈ ��ü�� ��������, ����Ʈ �����ϴ� �Լ��� ���� �Ŀ� ����Ʈ�� ���� ���Ŀ� ���ְų� SetActive(false)�ؾ� �ҵ�.

                //pv.RPC(nameof(CheckStopEffect), RpcTarget.AllViaServer);
                CheckStopEffect();

                //Destroy(this.gameObject);
            }
        }

        void CheckStopEffect()
        {
            Debug.Log("����Ʈ ���");
            isActivated = false;
            GetComponent<CapsuleCollider>().enabled = false;
            transform.GetChild(2).gameObject.SetActive(true);
            StartCoroutine(nameof(StopEffect));
        }

        IEnumerator StopEffect()
        {
            yield return new WaitUntil(() => transform.GetChild(2).gameObject.GetComponent<ParticleSystem>().isStopped);
            Debug.Log("���ָӴ� ����");
            Destroy(this.gameObject);
        }
    }

}
