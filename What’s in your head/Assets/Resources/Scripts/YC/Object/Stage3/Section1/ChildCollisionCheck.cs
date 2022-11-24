using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using JCW.AudioCtrl;
using Photon.Pun;

namespace YC_OBJ
{
    public class ChildCollisionCheck : MonoBehaviour
    {
        [Header("<��ȹ ���� ����>")]
        [Header("[������� �ִϸ��̼� �ӵ� (0.5 ~ 2.0 ��õ)]")]
        [SerializeField] float changeValue = 1.2f;

        string interactionTag1 = "SteadyBeam";
        [SerializeField] Bush bushObj;
        [SerializeField] GameObject upCollider;

        Renderer ren;
        MeshCollider meshCollider;

        bool isHit = false;

        AudioSource audioSource;
        PhotonView pv;

        void Start() 
        {

            ren = this.gameObject.GetComponent<MeshRenderer>();

            meshCollider = this.gameObject.GetComponent<MeshCollider>();

            audioSource = this.gameObject.GetComponent<AudioSource>();
            pv = this.gameObject.GetComponent<PhotonView>();

            SoundManager.Set3DAudio(pv.ViewID, audioSource, 2.5f, 30f, true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(interactionTag1))
            {           
                Attacked(0.1f);
            }
        }

        public void Attacked(float delayTime) // �ڶ� ��Ÿ���� �ҽ�, ��Ÿ �ʿ��� ȣ���ϴ� ����޽��� / �ڶ�����̶� ���׵������ ����Ʈ �����̰� �ٸ���.
        {
            if (isHit) return;

            isHit = true;
            meshCollider.enabled = false;
            upCollider.GetComponent<BoxCollider>().enabled = false;
            //ren.shadowCastingMode = ShadowCastingMode.Off;

            StartCoroutine(nameof(EffectShader), delayTime);
        }

        IEnumerator EffectShader(float delayTime)
        {

            SoundManager.Instance.Play3D_RPC("S3_BushDestroy", pv.ViewID);
            yield return new WaitForSeconds(delayTime);


            float curSplitValue = ren.material.GetFloat("_SplitValue");
            float minSplitValur = -0.1f;

            while (curSplitValue > minSplitValur)
            {
                curSplitValue = ren.material.GetFloat("_SplitValue");
                curSplitValue -= changeValue * Time.deltaTime;

                if (curSplitValue < minSplitValur)
                    curSplitValue = minSplitValur;
            
                ren.material.SetFloat("_SplitValue", curSplitValue);

                yield return null;

            }
            bushObj.SendMessage(nameof(bushObj.Destroy_Cor));
        }
    }
}
