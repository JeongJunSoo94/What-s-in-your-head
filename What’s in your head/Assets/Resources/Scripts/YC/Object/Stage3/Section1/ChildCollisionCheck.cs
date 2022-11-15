using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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

        void Start() 
        {

            ren = this.gameObject.GetComponent<MeshRenderer>();

            meshCollider = this.gameObject.GetComponent<MeshCollider>();
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

                bushObj.SendMessage(nameof(bushObj.Destroy_Cor));
            }
        }
    }
}
