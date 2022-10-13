using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JCW.Object
{
    public class HostField : MonoBehaviour
    {
        [Header("주위 필드 감염 소요 시간")][SerializeField][Range(0,13)] float infectTime;

        bool isPurified = false;
        int myIndex;

        float elapsedTime = 0f;

        // 감염된 필드의 인접 오프셋
        List<int> nextTargetOffset;
        Transform parentObj;

        int maxLimit;
        int convertIndex;

        bool isStart = false;

        private void Awake()
        {
            parentObj = transform.parent.gameObject.transform;            
            int N = transform.parent.gameObject.GetComponent<ContaminationFieldSetting>().count;
            convertIndex = 2*N-1;
            nextTargetOffset = new() { 2, -2, 2*convertIndex, -2*convertIndex  };
        }

        private void OnEnable()
        {
            if (isPurified)
                this.gameObject.SetActive(false);
        }


        void Update()
        {
            if (!isStart)
                return;
            if (isPurified)
                this.gameObject.SetActive(false);
            else if(nextTargetOffset.Count != 0)
            {
                elapsedTime+=Time.deltaTime;
                if (elapsedTime > infectTime)
                {                    
                    elapsedTime = 0f;

                    var random = new System.Random(Guid.NewGuid().GetHashCode());
                    int i = random.Next(0, nextTargetOffset.Count);
                    
                    // 전염시킬 인덱스가 수를 벗어나거나, 이미 켜져있을 때
                    while (myIndex + nextTargetOffset[i] < 0 || myIndex + nextTargetOffset[i] > maxLimit
                        || parentObj.GetChild(myIndex + nextTargetOffset[i]).gameObject.activeSelf)
                    {
                        nextTargetOffset.Remove(nextTargetOffset[i]);
                        if (nextTargetOffset.Count == 0)
                            return;
                        //random = new System.Random(Guid.NewGuid().GetHashCode());
                        i = random.Next(0,nextTargetOffset.Count);                        
                    }
                    // 오염 필드 켜주기
                    parentObj.GetChild(myIndex + nextTargetOffset[i]).gameObject.SetActive(true);
                    parentObj.GetChild(myIndex + nextTargetOffset[i]).gameObject.SendMessage("SetIndex", myIndex+nextTargetOffset[i]);
                    parentObj.GetChild(myIndex + nextTargetOffset[i]).gameObject.SendMessage("DeleteHost", myIndex);
                    

                    // 기존 필드 꺼주기
                    parentObj.GetChild(myIndex + nextTargetOffset[i] - 1 ).gameObject.SetActive(false);
                    nextTargetOffset.RemoveAt(i);
                }
            }
        }

        public void SetIndex(int index)
        {
            maxLimit = transform.parent.gameObject.transform.childCount - 1;
            myIndex = index;
            if ((myIndex-1) % (convertIndex*2) == 0)
                nextTargetOffset.Remove(-2);
            else if((myIndex + 1) % (convertIndex*2) == 0 )
                nextTargetOffset.Remove(2);

            if (myIndex < 2*convertIndex)
                nextTargetOffset.Remove(-2*convertIndex);
            else if (myIndex + 2*convertIndex > maxLimit)
                nextTargetOffset.Remove(2*convertIndex);
            
        }

        public void DeleteHost(int hostIndex)
        {
            nextTargetOffset.Remove(hostIndex-myIndex);            
            isStart = true;
            
        }

        public void SetStart()
        {
            isStart = true;
        }

        public void SetPurified()
        {
            isPurified = true;
            parentObj.GetChild(myIndex-1).gameObject.SetActive(true);
        }


        private void OnCollisionEnter(Collision collision)
        {
            switch (collision.gameObject.tag)
            {
                case "Nella":
                case "Steady":
                    Debug.Log(collision.gameObject.name + " 사망");
                    break;
                case "물총":
                    SetPurified();
                    break;
            }

        }
    }

}
