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
        [Header("N x N �ʵ� (2~20)")][SerializeField][Range(2,20)] public int count;
        [Header("���� �ʵ�")][SerializeField] GameObject hostField;
        [Header("���� �Ǵ� �ʵ�")][SerializeField] GameObject carrierField;
        [Header("���� ��Ű��")] public bool create = true;
        [Header("���� ���� ����")] public bool canInfect = true;


        float gap = 5f;


        private void Awake()
        {
            gap = this.gameObject.transform.localScale.x;            
        }
        void Update()
        {
            if (Application.isPlaying)
            {
                this.enabled = false;
                return;
            }
            // ������ �󿡼� ������ �� �ְ� �̸� �����صα�
            if (create)
            {
                for (int k = transform.childCount-1 ; k>=0; --k)
                {
                    DestroyImmediate(transform.GetChild(k).gameObject);
                }
                Vector3 transformParent = this.transform.position;
                for (int i = 0 ; i<count ; ++i)
                {
                    for (int j = 0 ; j<count ; ++j)
                    {                        
                        Vector3 curTransform = new Vector3(transformParent.x - (count/2)*gap + j*gap,
                                                            transformParent.y, 
                                                            transformParent.z + (count/2)*gap - i*gap);
                        if (canInfect)
                        {
                            Instantiate(carrierField, curTransform, this.transform.rotation, this.transform).SetActive(true);
                            Instantiate(hostField, curTransform, this.transform.rotation, this.transform).SetActive(false);
                        }
                        else
                        {
                            Instantiate(carrierField, curTransform, this.transform.rotation, this.transform).SetActive(false);
                            Instantiate(hostField, curTransform, this.transform.rotation, this.transform).SetActive(true);
                        }
                        
                    }
                }
                create = false;
            }
        }
    }

}
