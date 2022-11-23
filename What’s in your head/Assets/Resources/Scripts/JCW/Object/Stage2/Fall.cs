using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KSU.Object.Stage2;
using JCW.AudioCtrl;

namespace JCW.Object
{
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(AudioSource))]
    public class Fall : LinkedObjectWithReciever
    {
        [Header("�߶� �ʱ� �ӵ�")] [SerializeField] float fallSpeed;
        [Header("�߶� ���ӵ�")] [SerializeField] float fallAccelerateSpeed;
        [Header("�ٷ� �Ʒ��� �÷���")] [SerializeField] Transform groundPlatform;
        //Transform groundPlatform;

        Vector3 finalPos;
        PhotonView pv;
        AudioSource audioSource;
        public WaitForSeconds wait = new WaitForSeconds(0.01f);
        public float delayTime = 2;
        public float interpolationHeight=30f;
        bool delayUse;
        bool isDelayed;
        private void Awake()
        {
            if (!groundPlatform)
            {
                Debug.Log("�߶��� �÷����� �������� �ʾҽ��ϴ�.");
                Destroy(this.gameObject);
                return;
            }
            pv = GetComponent<PhotonView>();
            transform.GetChild(0).GetComponent<SandSackShadow>().groundPlatform = this.groundPlatform;
            finalPos = transform.position;
            finalPos.y = groundPlatform.position.y;
            isDelayed = false;
            delayUse = true;
            audioSource = GetComponent<AudioSource>();
            SoundManager.Set3DAudio(pv.ViewID, audioSource, 1f, 60f);
        }

        void Update()
        {
            if (!isActivated)//Ʈ�� ���
                return;

            if (delayUse)
            {
                transform.GetChild(0).GetComponent<SandSackShadow>().interpolationHeight = interpolationHeight;
                StartCoroutine(SandAttackCoroutine(delayTime));
            }

            if(isDelayed)
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
            SoundManager.Instance.Play3D("S2_SandSack_Fall", pv.ViewID);
            StartCoroutine(nameof(StopEffect));
        }

        IEnumerator StopEffect()
        {
            yield return new WaitUntil(() => transform.GetChild(2).gameObject.GetComponent<ParticleSystem>().isStopped);
            Debug.Log("���ָӴ� ����");
            Destroy(this.gameObject);
        }

        IEnumerator SandAttackCoroutine(float delayTime)
        {
            delayUse = false;
            isDelayed = true;
            float curCool = 0;
            while (curCool < delayTime)
            {
                curCool += 0.01f;
                yield return wait;
            }
            isDelayed = false;
            yield break;
        }
    }

}
