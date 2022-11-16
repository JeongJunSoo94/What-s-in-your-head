using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using JCW.AudioCtrl;

namespace YC_OBJ
{
    [RequireComponent(typeof(AudioSource))]
    public class FlowerTree : MonoBehaviour
    {
        [Header("<기획 편집 사항>")]
        [Space]

        [Header("[히트 시간 (해당 시간동안 물총을 맞을 경우 꽃이 피게 된다)]")]
        [SerializeField] float MaxTime = 2.0f;
        public float maxTime { get; private set; } // 최대 시간 게이지

    
        public float curTime { get; private set; } = 0; // 현재 시간 게이지
        bool isCurHit = false;
        int curHitCount = 0;
        float delayTime = 0.12f; // 총알을 현재 맞고 있는지를 몇 초 전과 비교할 것인지 (총알 발사 속도와 연관)


        string interactionObjTag = "NellaWater";

        bool isBloom = false;

        JCW.UI.InGame.Indicator.OneIndicator indicator;

        PhotonView pv;

        GameObject FlowerObj;

        AudioSource audioSource;


        void Awake()
        {   
            indicator = transform.GetChild(4).GetComponent<JCW.UI.InGame.Indicator.OneIndicator>();  //인덱스 번호 확인!

            maxTime = MaxTime;

            pv = this.gameObject.GetComponent<PhotonView>();

            FlowerObj = transform.GetChild(0).gameObject;

            audioSource = GetComponent<AudioSource>();
            JCW.AudioCtrl.AudioSettings.SetAudio(audioSource, 1, 50f);
        }

        void Update()
        {
            if (!isBloom)
            {
                SetCurTime();

                indicator.SetGauge(curTime / maxTime);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isBloom) return;

            if (other.gameObject.CompareTag(interactionObjTag))
            {
                curHitCount++;

                StartCoroutine(nameof(SetPrevHit));
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
                        pv.RPC(nameof(SetBloom), RpcTarget.AllViaServer);
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
            int preHitCount = curHitCount; // 현재 카운트를 저장 해둔다

            yield return new WaitForSeconds(delayTime); // 딜레이를 준다

            if (preHitCount == curHitCount) // 딜레이 주기 전에 저장해둔 카운트와, 현재 카운트를 비교한다
            {
                isCurHit = false;
            }
            else // 맞았다
            {
                isCurHit = true;
            }
        }

        [PunRPC]
        public void SetBloom()
        {
            indicator.gameObject.SetActive(false);
            FlowerObj.GetComponent<PotFlower>().SetBloom();
            FlowerObj.gameObject.GetComponent<CapsuleCollider>().enabled = false;
            //SoundManager.Instance.Play3D_RPC("PlantGrow", audioSource);

        }

     
    }

}
