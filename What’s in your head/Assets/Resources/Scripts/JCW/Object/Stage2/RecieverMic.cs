using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JJS;
namespace JCW.Object
{
    public class RecieverMic : MonoBehaviour
    {
        [Header("���� ����")][SerializeField] float detectRange;
        [Header("��ȣ�ۿ��� �� �ִ� ������Ʈ��")] [SerializeField] List<PlayObject> objectList;

        //(���� �ڶ� ��ũ��Ʈ �ڷ���) (������);
        Singing nellaSinging;
        Animator animator;

        private void Awake()
        {
            transform.GetChild(1).localScale = new Vector3(detectRange, detectRange, detectRange);
            animator = GetComponent<Animator>();
        }

        public void Play(bool enable)
        {
            animator.SetBool("Play", enable);
            for (int i = 0; i < objectList.Count; ++i)
            {
                if(enable)
                    objectList[i].enabled = enable;
                objectList[i].isActive = enable;
            }
        }

        int scriptNum = 0;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Nella"))
            {
                // �ڶ� ���� �� �� ��ũ��Ʈ�� �ڶ� ��ũ��Ʈ �ȿ� Dictionary ���·� ���� ��, �ڶ��� �÷� ���� ������ �� List�� ���鼭
                // ������ RecieverMic ��ũ��Ʈ�� ������ �ִ� ������Ʈ���� isActive�� true�� �ٲ���.
                // ���� ���°� ������ false�� �ٲ���.
                if(nellaSinging == null)
                    nellaSinging = other.GetComponent<Singing>();

                scriptNum = nellaSinging.GetMicIndex();
                Debug.Log("���� ��ųʸ��� �߰�");
                nellaSinging.MicDictionary.Add(scriptNum, this);
                //for (int i =0 ; i< objectList.Count ; ++i)
                //{                    
                //    objectList[i].enabled = true;
                //    objectList[i].isActive = true;
                //}
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Nella"))
            {
                Play(false);
                nellaSinging.indexQueue.Enqueue(scriptNum);
                nellaSinging.MicDictionary.Remove(scriptNum);
            }
        }

    }
}

