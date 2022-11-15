using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace YC_OBJ
{
    public class ChildCollisionCheck : MonoBehaviour
    {
        [Header("<기획 편집 사항>")]
        [Header("[사라지는 애니메이션 속도 (0.5 ~ 2.0 추천)]")]
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

        public void Attacked(float delayTime) // 넬라가 기타공격 할시, 기타 쪽에서 호출하는 센드메시지 / 넬라공격이랑 스테디공격의 이펙트 딜레이가 다르다.
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
