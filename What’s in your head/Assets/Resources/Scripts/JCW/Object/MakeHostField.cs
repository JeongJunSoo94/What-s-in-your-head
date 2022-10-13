using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace JCW.Object
{

    public class MakeHostField : MonoBehaviour
    {
        [Header("===========���� ��===========")]
        [Header("ù ��° ���� �ð�")][SerializeField] float firstSpawnTime = 40f;
        [Header("�� ��° ���� �ð�")][SerializeField] float secondSpawnTime = 120f;

        int usingCount;
        List<int> firstSpawnPlace;
        float elapsedTime = 0f;

        int curPhase = 0;

        private void Awake()
        {
            usingCount = this.gameObject.GetComponent<ContaminationFieldSetting>().count;

            int N = 2*usingCount - 1;

            // ������ �� �ִ� �� ������ ���صα�
            firstSpawnPlace = new() { 1, 2*N-1, 2*N*(N-1)+1, 2*N*N-1 };


            // �� �� 1���� �����ϰ� �����
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
                    // 4 ������ �� 2���� ����
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
                    // ���� 2 ������ �� 1���� ����
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
