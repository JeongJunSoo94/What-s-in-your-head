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
        [Header("===========시작 후===========")]
        [Header("첫 번째 스폰 시간")][SerializeField] float firstSpawnTime = 40f;
        [Header("두 번째 스폰 시간")][SerializeField] float secondSpawnTime = 120f;
        [Header("주위 필드 감염 소요 시간")][SerializeField] float infectTime = 5f;
        [Header("감염 가능 여부")][SerializeField] bool canInfect = true;

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

            // 스폰될 수 있는 각 꼭지점 정해두기
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
                        // 4 꼭지점 중 2군데 생성
                        if (elapsedTime > firstSpawnTime)
                        {
                            photonView.RPC(nameof(SpawnFirst), RpcTarget.AllViaServer);
                            ++curPhase;
                        }
                        break;
                    case 1:
                        // 남은 2 꼭지점 중 1군데 생성
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
                // 다음 타겟이 이미 오염되어 있거나, 내가 이미 정화되어있으면 감염 더 이상 안 시킴.
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
                // 오염 필드 켜주기                
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
                // 그 중 1군데 랜덤하게 지우기
                var random = new System.Random(Guid.NewGuid().GetHashCode());
                randomIndex = random.Next(0, 4);
                photonView.RPC(nameof(Init), RpcTarget.AllViaServer, randomIndex);
                isStart = true;
            }
        }
    }
}
