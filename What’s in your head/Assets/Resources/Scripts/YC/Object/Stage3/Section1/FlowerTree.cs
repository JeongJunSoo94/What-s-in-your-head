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
        [Header("<��ȹ ���� ����>")]
        [Space]

        [Header("[��Ʈ �ð� (�ش� �ð����� ������ ���� ��� ���� �ǰ� �ȴ�)]")]
        [SerializeField] float MaxTime = 2.0f;
        public float maxTime { get; private set; } // �ִ� �ð� ������

    
        public float curTime { get; private set; } = 0; // ���� �ð� ������
        bool isCurHit = false;
        int curHitCount = 0;
        float delayTime = 0.12f; // �Ѿ��� ���� �°� �ִ����� �� �� ���� ���� ������ (�Ѿ� �߻� �ӵ��� ����)


        string interactionObjTag = "NellaWater";

        bool isBloom = false;

        JCW.UI.InGame.Indicator.OneIndicator indicator;

        PhotonView pv;

        GameObject FlowerObj;

        AudioSource audioSource;


        void Awake()
        {   
            indicator = transform.GetChild(4).GetComponent<JCW.UI.InGame.Indicator.OneIndicator>();  //�ε��� ��ȣ Ȯ��!

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
        public void SetBloom()
        {
            indicator.gameObject.SetActive(false);
            FlowerObj.GetComponent<PotFlower>().SetBloom();
            FlowerObj.gameObject.GetComponent<CapsuleCollider>().enabled = false;
            //SoundManager.Instance.Play3D_RPC("PlantGrow", audioSource);

        }

     
    }

}
