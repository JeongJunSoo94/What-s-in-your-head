using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.Object
{
    [ExecuteInEditMode]
    public class ContaminationFieldSetting : MonoBehaviour
    {
        [Header("===========시작 전===========")]
        [Header("(2N-1)x(2N-1) 필드")][SerializeField][Range(2,10)] public int count;
        [Header("오염 필드")][SerializeField] GameObject hostField;
        [Header("전염 되는 필드")][SerializeField] GameObject carrierField;
        [Header("생성 시키기")] public bool create = true;


        float gap = 5f;


        private void Awake()
        {
            gap = this.gameObject.transform.localScale.x;            
        }
        void Update()
        {
            // 에디터 상에서 만들어둘 수 있게 미리 생성해두기
            if (create)
            {
                for (int k = transform.childCount-1 ; k>=0; --k)
                {
                    DestroyImmediate(transform.GetChild(k).gameObject);
                }
                Vector3 transformParent = this.transform.position;
                for (int i = 0 ; i<2 * count -1 ; ++i)
                {
                    for (int j = 0 ; j< 2 * count - 1 ; ++j)
                    {                        
                        Vector3 curTransform = new Vector3(transformParent.x - (count-1)*gap + j*gap,
                                                            transformParent.y, 
                                                            transformParent.z + (count-1)*gap - i*gap);

                        Instantiate(carrierField, curTransform, this.transform.rotation, this.transform).SetActive(true);
                        Instantiate(hostField, curTransform, this.transform.rotation, this.transform).SetActive(false);
                    }
                }
                create = false;
            }
        }
    }

}
