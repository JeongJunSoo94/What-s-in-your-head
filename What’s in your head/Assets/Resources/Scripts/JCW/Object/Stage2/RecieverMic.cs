using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JJS;
namespace JCW.Object
{
    public class RecieverMic : MonoBehaviour
    {
        [Header("감지 범위")][SerializeField] float detectRange;
        [Header("상호작용할 수 있는 오브젝트들")] [SerializeField] List<PlayObject> objectList;

        //(대충 넬라 스크립트 자료형) (변수명);
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
                // 넬라가 접근 시 이 스크립트를 넬라 스크립트 안에 Dictionary 형태로 저장 후, 넬라의 플룻 연주 상태일 때 List로 돌면서
                // 각각의 RecieverMic 스크립트가 가지고 있는 오브젝트들의 isActive를 true로 바꿔줌.
                // 연주 상태가 끝나면 false로 바꿔줌.
                if(nellaSinging == null)
                    nellaSinging = other.GetComponent<Singing>();

                scriptNum = nellaSinging.GetMicIndex();
                Debug.Log("싱잉 딕셔너리에 추가");
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

