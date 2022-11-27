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
        //[Header("N x N 필드 (2~20)")][SerializeField][Range(2,20)] public int count;
        [Header("필드 가로 개수")][SerializeField][Range(2,50)] public int widthCount;
        [Header("필드 세로 개수")][SerializeField][Range(2,50)] public int heightCount;
        [Header("오염 필드")][SerializeField] GameObject hostField;
        [Header("전염 되는 필드")][SerializeField] GameObject carrierField;
        [Header("생성 시키기")] public bool create = true;
        [Header("지우기")] public bool clear = false;
        [Header("감염 가능 여부")] public bool canInfect = true;
        //[Header("간격 (기본값 5)")] [SerializeField]
        [Header("블록 간격 (기본값 : 1)")] [SerializeField] [Range(0f,1f)] float betweenGap = 1f;
        float gap;

        private void Awake()
        {
            gap = betweenGap * 2f* transform.localScale.x;            
        }
        void Update()
        {
            if (Application.isPlaying)
            {
                this.enabled = false;
                return;
            }
            // 에디터 상에서 만들어둘 수 있게 미리 생성해두기
            if( clear)
            {
                for (int k = transform.childCount - 1 ; k >= 0 ; --k)
                {
                    DestroyImmediate(transform.GetChild(k).gameObject);
                }
                clear = false;
            }

            if (create)
            {
                for (int k = transform.childCount-1 ; k>=0; --k)
                {
                    DestroyImmediate(transform.GetChild(k).gameObject);
                }
                Vector3 transformParent = this.transform.position;
                for (int i = 0 ; i< heightCount ; ++i)
                {
                    for (int j = 0 ; j< widthCount ; ++j)
                    {                        
                        Vector3 curTransform = new Vector3(transformParent.x - (widthCount * 0.5f )*gap + j*gap,
                                                            transformParent.y, 
                                                            transformParent.z + (heightCount * 0.5f)*gap - i*gap);
                        if (canInfect)
                        {
                            Instantiate(carrierField, curTransform, this.transform.rotation, this.transform).SetActive(true);
                            curTransform.y += 1f;
                            Instantiate(hostField, curTransform, this.transform.rotation, this.transform).SetActive(true);
                        }
                        else
                        {
                            Instantiate(carrierField, curTransform, this.transform.rotation, this.transform).SetActive(false);
                            curTransform.y += 1f;
                            Instantiate(hostField, curTransform, this.transform.rotation, this.transform).SetActive(true);
                        }
                        
                    }
                }
                create = false;
            }
        }
    }

}
