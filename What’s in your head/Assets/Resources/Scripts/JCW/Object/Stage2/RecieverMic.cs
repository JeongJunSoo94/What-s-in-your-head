using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.Object
{
    public class RecieverMic : MonoBehaviour
    {
        [Header("상호작용할 수 있는 오브젝트들")] [SerializeField] List<PlayObject> objectList;

        //(대충 넬라 스크립트 자료형) (변수명);

        int scriptNum = 0;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Nella"))
            {
                // 넬라가 접근 시 이 스크립트를 넬라 스크립트 안에 List 형태로 저장 후, 넬라의 플룻 연주 상태일 때 List로 돌면서
                // 각각의 RecieverMic 스크립트가 가지고 있는 오브젝트들의 isActive를 true로 바꿔줌.
                // 연주 상태가 끝나면 false로 바꿔줌.

                /*
                if(변수명 == null)
                {
                    변수명 = 넬라.GetComponent<스크립트>();
                    scriptNum = 변수명.Dictionary변수.Count;
                    변수명.Dictionary변수.Add(scriptNum, this);
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
                if(변수명 != null)
                {
                    변수명.Dictionary변수[scriptNum].isActive = false;
                    변수명.Dictionary변수.Remove(scriptNum);
                    변수명 = null;
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

