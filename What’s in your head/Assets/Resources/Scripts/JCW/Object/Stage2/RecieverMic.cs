using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.Object
{
    public class RecieverMic : MonoBehaviour
    {
        [Header("��ȣ�ۿ��� �� �ִ� ������Ʈ��")] [SerializeField] List<PlayObject> objectList;

        //(���� �ڶ� ��ũ��Ʈ �ڷ���) (������);

        int scriptNum = 0;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Nella"))
            {
                // �ڶ� ���� �� �� ��ũ��Ʈ�� �ڶ� ��ũ��Ʈ �ȿ� List ���·� ���� ��, �ڶ��� �÷� ���� ������ �� List�� ���鼭
                // ������ RecieverMic ��ũ��Ʈ�� ������ �ִ� ������Ʈ���� isActive�� true�� �ٲ���.
                // ���� ���°� ������ false�� �ٲ���.

                /*
                if(������ == null)
                {
                    ������ = �ڶ�.GetComponent<��ũ��Ʈ>();
                    scriptNum = ������.Dictionary����.Count;
                    ������.Dictionary����.Add(scriptNum, this);
                }                
                */
                for (int i =0 ; i< objectList.Count ; ++i)
                {                    
                    objectList[i].enabled = true;
                    objectList[i].isActive = true;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Nella"))
            {
                /*
                if(������ != null)
                {
                    ������.Dictionary����[scriptNum].isActive = false;
                    ������.Dictionary����.Remove(scriptNum);
                    ������ = null;
                    scriptNum = 0;
                }                
                */
                for (int i = 0 ; i < objectList.Count ; ++i)
                {
                    objectList[i].isActive = false;                    
                }
            }
        }

        public void SetObjectState(bool isOn)
        {
            for (int i = 0 ; i < objectList.Count ; ++i)
            {
                if(isOn)
                    objectList[i].enabled = true;
                objectList[i].isActive = isOn;
            }
        }
    }
}

