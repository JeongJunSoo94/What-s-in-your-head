using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.Object
{
    [ExecuteInEditMode]
    public class ContaminationFieldSetting : MonoBehaviour
    {
        [Header("===========���� ��===========")]
        //[Header("N x N �ʵ� (2~20)")][SerializeField][Range(2,20)] public int count;
        [Header("�ʵ� ���� ����")][SerializeField][Range(2,50)] public int widthCount;
        [Header("�ʵ� ���� ����")][SerializeField][Range(2,50)] public int heightCount;
        [Header("���� �ʵ�")][SerializeField] GameObject hostField;
        [Header("���� �Ǵ� �ʵ�")][SerializeField] GameObject carrierField;
        [Header("���� ��Ű��")] public bool create = true;
        [Header("�����")] public bool clear = false;
        [Header("���� ���� ����")] public bool canInfect = true;
        //[Header("���� (�⺻�� 5)")] [SerializeField]
        [Header("��� ���� (�⺻�� : 1)")] [SerializeField] [Range(0f,1f)] float betweenGap = 1f;
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
            // ������ �󿡼� ������ �� �ְ� �̸� �����صα�
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
