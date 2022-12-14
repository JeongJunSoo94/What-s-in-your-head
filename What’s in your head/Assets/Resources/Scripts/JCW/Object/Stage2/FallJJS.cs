using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KSU.Object.Stage2;
using JCW.AudioCtrl;
using Photon.Pun;

namespace JCW.Object
{
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(AudioSource))]

    public class FallJJS : LinkedObjectWithReciever
    {
        [Header("추락 초기 속도")] [SerializeField] float fallSpeed;
        [Header("추락 가속도")] [SerializeField] float fallAccelerateSpeed;
        Transform groundPlatform;

        Vector3 finalPos;

        SandSackShadow sandSackShadow;
        PhotonView pv;
        AudioSource audioSource;

        public LayerMask layer;
        public JCW.Spawner.Spawner_Photon spawner;
        public WaitForSeconds wait = new WaitForSeconds(0.01f);

        private void Awake()
        {
            //transform.GetChild(0).GetComponent<SandSackShadow>().groundPlatform = this.groundPlatform;
            sandSackShadow = transform.GetChild(0).GetComponent<SandSackShadow>();            
            finalPos = transform.position;
            pv = GetComponent<PhotonView>();
            audioSource = GetComponent<AudioSource>();
            SoundManager.Set3DAudio(pv.ViewID, audioSource, 1f, 60f);

            //sandSackShadow.SetGroundPlatform(groundPlatform);
        }

        void Update()
        {
            GroundCheck();
            if (!isActivated)
                return;
            transform.position = Vector3.MoveTowards(transform.position, finalPos, Time.deltaTime * fallSpeed);
            fallSpeed += Time.deltaTime * fallAccelerateSpeed;
        }

        public void StartCoroutineFall(float time)
        {
            GroundCheck();
            StartCoroutine(SandAttackCoroutine(time));
        }

        public void Initialized()
        {
            transform.GetChild(2).gameObject.SetActive(false);
            GetComponent<CapsuleCollider>().enabled = true;
        }

        public void GroundCheck()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -transform.up,out hit, 200f, layer, QueryTriggerInteraction.Ignore))
            {
                groundPlatform = hit.transform;
                sandSackShadow.SetGroundPlatform(groundPlatform);
                finalPos = hit.point;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            //Debug.Log(other.gameObject.layer);

            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                //Debug.Log("모래주머니 캐릭터 깔고 뭉갬");
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Platform"))
            {
                //Debug.Log("모래주머니 땅에 추락");

                //현재는 추락 시 오브젝트 자체를 없애지만, 이펙트 실행하는 함수를 실행 후에 이펙트가 끝난 이후에 없애거나 SetActive(false)해야 할듯.

                //pv.RPC(nameof(CheckStopEffect), RpcTarget.AllViaServer);
                CheckStopEffect();

                //Destroy(this.gameObject);
            }
        }

        void CheckStopEffect()
        {
            isActivated = false;
            GetComponent<CapsuleCollider>().enabled = false;
            transform.GetChild(2).gameObject.SetActive(true);
            SoundManager.Instance.Play3D("S2_SandSack_Fall", pv.ViewID);
            StartCoroutine(nameof(StopEffect));
        }

        IEnumerator StopEffect()
        {
            yield return new WaitUntil(() => transform.GetChild(2).gameObject.GetComponent<ParticleSystem>().isStopped);
            //Destroy(this.gameObject);
            spawner.Despawn(this.gameObject);
        }

        IEnumerator SandAttackCoroutine(float delayTime)
        {
            float curCool = 0;
            while (curCool < delayTime)
            {
                curCool += 0.01f;
                yield return wait;
            }
            isActivated = true;
            yield break;
        }
    }

}
