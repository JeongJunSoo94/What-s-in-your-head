using JCW.AudioCtrl;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.Object
{
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(AudioSource))]
    public class MakeHostField : MonoBehaviour
    {
        [Header("===========���� ��===========")]
        [Header("ù ��° ���� �ð�")][SerializeField] float firstSpawnTime = 40f;
        [Header("�� ��° ���� �ð�")][SerializeField] float secondSpawnTime = 120f;
        [Header("���� �ʵ� ���� �ҿ� �ð�")][SerializeField] float infectTime = 5f;
        [Header("���� ���� ����")][SerializeField] bool canInfect = true;

        //int usingCount;
        int usingWidthCount;
        int usingHeightCount;
        List<int> firstSpawnPlace;
        float elapsedTime = 0f;

        int curPhase = 0;
        int randomIndex = 0;

        PhotonView photonView;

        bool isStart;

        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
            if (!canInfect)
            {
                this.enabled = false;
                return;
            }
            usingWidthCount = GetComponent<ContaminationFieldSetting>().widthCount;
            usingHeightCount = GetComponent<ContaminationFieldSetting>().heightCount;

            // ������ �� �ִ� �� ������ ���صα�
            firstSpawnPlace = new() { 1, 2* usingWidthCount - 1, 2* usingWidthCount * (usingHeightCount-1)+1, 2* usingWidthCount * usingHeightCount - 1 };          

            StartCoroutine(nameof(WaitForPlayer));
        }

        void Update()
        {
            if (!isStart) return;
            elapsedTime += Time.deltaTime;
            if(PhotonNetwork.IsMasterClient)
            {
                switch (curPhase)
                {
                    case 0:
                        // 4 ������ �� 2���� ����
                        if (elapsedTime > firstSpawnTime)
                        {
                            photonView.RPC(nameof(SpawnFirst), RpcTarget.AllViaServer);
                            ++curPhase;
                        }
                        break;
                    case 1:
                        // ���� 2 ������ �� 1���� ����
                        if (elapsedTime > secondSpawnTime)
                        {
                            photonView.RPC(nameof(SpawnSecond), RpcTarget.AllViaServer);
                            ++curPhase;
                        }
                        break;
                    case 2:
                        GetComponent<MakeHostField>().enabled = false;
                        break;
                }
            }            
        }

        [PunRPC]
        void Init(int i)
        {
            firstSpawnPlace.RemoveAt(i);
        }

        [PunRPC]
        void SpawnFirst()
        {
            transform.GetChild(firstSpawnPlace[0]).GetComponent<HostField>().SetStart(firstSpawnPlace[0]);

            transform.GetChild(firstSpawnPlace[1]).GetComponent<HostField>().SetStart(firstSpawnPlace[1]);

            firstSpawnPlace.RemoveRange(0, 2);
        }

        [PunRPC]
        void SpawnSecond()
        {
            transform.GetChild(firstSpawnPlace[0]).GetComponent<HostField>().SetStart(firstSpawnPlace[0]);
            firstSpawnPlace.Clear();

            ++curPhase;
        }

        public void Infect(int myIndex, int nextTarget)
        {
            photonView.RPC(nameof(InfectRPC), RpcTarget.AllViaServer, myIndex, nextTarget);
        }

        [PunRPC]
        void InfectRPC(int myIndex, int nextTarget)
        {
            List<int> indexs = new() { myIndex, nextTarget };
            StartCoroutine(nameof(WarnInfection), indexs);
        }

        IEnumerator WarnInfection(List<int> indexs)
        {
            float curTime = 0f;
            GameObject nextTargetBeforeObj = transform.GetChild(indexs[0] + indexs[1] -1).gameObject;
            GameObject nextTargetBeforeObj_ready = nextTargetBeforeObj.transform.GetChild(0).gameObject;
            GameObject nextTargetAfterObj = transform.GetChild(indexs[0] + indexs[1]).gameObject;

            HostField nextHostField = nextTargetAfterObj.GetComponent<HostField>();

            if (nextHostField.isPurified)
                yield break;

            bool isStart = false;
            while (curTime<infectTime)
            {
                // ���� Ÿ���� �̹� �����Ǿ� �ְų�, ���� �̹� ��ȭ�Ǿ������� ���� �� �̻� �� ��Ŵ.
                if (!transform.GetChild(indexs[0]).gameObject.activeSelf
                    || !nextTargetBeforeObj.gameObject.activeSelf)
                {
                    nextTargetBeforeObj_ready.SetActive(false);
                    break;
                }
                curTime+= Time.deltaTime;
                if (!isStart)
                {
                    isStart = true;
                    nextTargetBeforeObj_ready.SetActive(true);
                }
                //SoundManager.Instance.Play3D_RPC("ContaminationFieldWarn", audioSource);
                yield return null;
            }
                
            if (transform.GetChild(indexs[0]).gameObject.activeSelf)
            {
                // ���� �ʵ� ���ֱ�                
                nextTargetAfterObj.SetActive(true);
                nextHostField.SetIndex(indexs[0] + indexs[1]);
                nextHostField.DeleteHost(indexs[0]);
            }

            nextTargetBeforeObj_ready.SetActive(false);
            indexs.Clear();
            yield break;
        }

        IEnumerator WaitForPlayer()
        {
            yield return new WaitUntil(() => GameManager.Instance.GetCharOnScene(true) && GameManager.Instance.GetCharOnScene(false));

            if (PhotonNetwork.IsMasterClient)
            {
                // �� �� 1���� �����ϰ� �����
                var random = new System.Random(Guid.NewGuid().GetHashCode());
                randomIndex = random.Next(0, 4);
                photonView.RPC(nameof(Init), RpcTarget.AllViaServer, randomIndex);
                isStart = true;
            }
        }
    }
}
