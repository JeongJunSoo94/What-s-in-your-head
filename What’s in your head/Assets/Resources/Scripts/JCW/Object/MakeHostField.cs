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
        [Header("깜빡거리는 시간")][SerializeField] float flickTime = 1f;
        [Header("주위 필드 감염 소요 시간")][SerializeField] float infectTime = 5f;
        [Header("감염 가능 여부")][SerializeField] bool canInfect = true;

        int usingCount;
        List<int> firstSpawnPlace;
        float elapsedTime = 0f;

        int curPhase = 0;
        int randomIndex = 0;

        PhotonView photonView;
        AudioSource audioSource;

        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
            if (!canInfect)
            {
                this.enabled = false;
                return;
            }
            usingCount = GetComponent<ContaminationFieldSetting>().count;

            // 스폰될 수 있는 각 꼭지점 정해두기
            firstSpawnPlace = new() { 1, 2*usingCount-1, 2*usingCount*(usingCount-1)+1, 2*usingCount*usingCount-1 };

            if (PhotonNetwork.IsMasterClient)
            {
                // 그 중 1군데 랜덤하게 지우기
                var random = new System.Random(Guid.NewGuid().GetHashCode());
                randomIndex = random.Next(0, 4);
                photonView.RPC(nameof(Init), RpcTarget.AllViaServer, randomIndex);
            }
            audioSource = GetComponent<AudioSource>();
            Debug.Log(Vector3.Distance(this.transform.position, transform.GetChild(0).position) + 30f);
            AudioCtrl.AudioSettings.SetAudio(audioSource, 1f, Vector3.Distance(this.transform.position, transform.GetChild(0).position) + 30f);
        }

        void Update()
        {
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

        public void SetPurified(int myIndex)
        {
            photonView.RPC(nameof(SetPurifiedRPC), RpcTarget.AllViaServer, myIndex);
        }

        [PunRPC]
        void SetPurifiedRPC(int myIndex)
        {
            Debug.Log("정화시킬 인덱스 : " + myIndex);
            transform.GetChild(myIndex).GetComponent<HostField>().Purified();
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
                curTime+= flickTime / 2f;
                if(!isStart)
                {
                    isStart = true;
                    nextTargetBeforeObj_ready.SetActive(true);
                }
                SoundManager.Instance.Play3D_RPC("ContaminationFieldWarn", audioSource);
                yield return new WaitForSeconds(0.5f);
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
    }
}
