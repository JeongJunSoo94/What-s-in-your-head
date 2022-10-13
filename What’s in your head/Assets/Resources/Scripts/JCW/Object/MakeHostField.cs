using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace JCW.Object
{

    public class MakeHostField : MonoBehaviour
    {
        [Header("===========시작 후===========")]
        [Header("첫 번째 스폰 시간")][SerializeField] float firstSpawnTime = 40f;
        [Header("두 번째 스폰 시간")][SerializeField] float secondSpawnTime = 120f;

        int usingCount;
        List<int> firstSpawnPlace;
        float elapsedTime = 0f;

        int curPhase = 0;

        private void Awake()
        {
            usingCount = this.gameObject.GetComponent<ContaminationFieldSetting>().count;

            int N = 2*usingCount - 1;

            // 스폰될 수 있는 각 꼭지점 정해두기
            firstSpawnPlace = new() { 1, 2*N-1, 2*N*(N-1)+1, 2*N*N-1 };


            // 그 중 1군데 랜덤하게 지우기
            var random = new System.Random(Guid.NewGuid().GetHashCode());
            int i = random.Next(0, 4);
            firstSpawnPlace.RemoveAt(i);
        }

        void Update()
        {
            elapsedTime += Time.deltaTime;
            switch (curPhase)
            {
                case 0:
                    // 4 꼭지점 중 2군데 생성
                    if (curPhase == 0 && elapsedTime > firstSpawnTime)
                    {
                        transform.GetChild(firstSpawnPlace[0]-1).gameObject.SetActive(false);
                        transform.GetChild(firstSpawnPlace[0]).gameObject.SetActive(true);
                        transform.GetChild(firstSpawnPlace[0]).gameObject.SendMessage("SetIndex", firstSpawnPlace[0]);
                        transform.GetChild(firstSpawnPlace[0]).gameObject.SendMessage("SetStart");

                        transform.GetChild(firstSpawnPlace[1]-1).gameObject.SetActive(false);
                        transform.GetChild(firstSpawnPlace[1]).gameObject.SetActive(true);
                        transform.GetChild(firstSpawnPlace[1]).gameObject.SendMessage("SetIndex", firstSpawnPlace[1]);
                        transform.GetChild(firstSpawnPlace[1]).gameObject.SendMessage("SetStart");

                        firstSpawnPlace.RemoveRange(0, 2);
                        ++curPhase;
                    }
                    break;
                case 1:
                    // 남은 2 꼭지점 중 1군데 생성
                    if (curPhase == 1 && elapsedTime > secondSpawnTime)
                    {
                        transform.GetChild(firstSpawnPlace[0]-1).gameObject.SetActive(false);
                        transform.GetChild(firstSpawnPlace[0]).gameObject.SetActive(true);
                        transform.GetChild(firstSpawnPlace[0]).gameObject.SendMessage("SetIndex", firstSpawnPlace[0]);
                        transform.GetChild(firstSpawnPlace[0]).gameObject.SendMessage("SetStart");
                        firstSpawnPlace.Clear();

                        ++curPhase;
                    }
                    break;
                case 2:
                    GetComponent<MakeHostField>().enabled = false;
                    break;
            }
        }
    }
}
