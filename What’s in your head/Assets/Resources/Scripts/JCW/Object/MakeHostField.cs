using JCW.AudioCtrl;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.Object
{
    [RequireComponent(typeof(PhotonView))]
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
            transform.GetChild(firstSpawnPlace[0] - 1).gameObject.SetActive(false);
            transform.GetChild(firstSpawnPlace[0]).gameObject.SetActive(true);
            transform.GetChild(firstSpawnPlace[0]).gameObject.SendMessage("SetStart", firstSpawnPlace[0]);

            transform.GetChild(firstSpawnPlace[1] - 1).gameObject.SetActive(false);
            transform.GetChild(firstSpawnPlace[1]).gameObject.SetActive(true);
            transform.GetChild(firstSpawnPlace[1]).gameObject.SendMessage("SetStart", firstSpawnPlace[1]);

            firstSpawnPlace.RemoveRange(0, 2);
            //++curPhase;
        }

        [PunRPC]
        void SpawnSecond()
        {
            transform.GetChild(firstSpawnPlace[0] - 1).gameObject.SetActive(false);
            transform.GetChild(firstSpawnPlace[0]).gameObject.SetActive(true);
            transform.GetChild(firstSpawnPlace[0]).gameObject.SendMessage("SetStart", firstSpawnPlace[0]);
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
            SoundManager.Instance.PlayEffect_RPC("ContaminationFieldPurified");
            transform.GetChild(myIndex).gameObject.GetComponent<HostField>().enabled = false;
            transform.GetChild(myIndex).gameObject.SetActive(false);
            transform.GetChild(myIndex - 1).gameObject.SetActive(true);
        }

        IEnumerator WarnInfection(List<int> indexs)
        {
            float curTime = 0f;
            GameObject nextTargetBeforeObj = transform.GetChild(indexs[0] + indexs[1] -1).gameObject;
            GameObject nextTargetAfterObj = transform.GetChild(indexs[0] + indexs[1]).gameObject;

            HostField nextHostField = nextTargetAfterObj.GetComponent<HostField>();

            if (nextHostField.isPurified)
                yield break;

            Color color1 = new(1f,1f,1f,1f);
            Color color2 = new(1f,0.7f,0.7f,1f);
            bool isChange = false;
            Material mat = nextTargetBeforeObj.GetComponent<MeshRenderer>().material;
            while (curTime<infectTime)
            {
                // 내가 이미 정화되었으면 감염 더 이상 안시킴
                // 이미 오염된 적 있으면 감염 안시킴
                if (!transform.GetChild(indexs[0]).gameObject.activeSelf
                    || !nextTargetBeforeObj.gameObject.activeSelf)
                {                    
                    mat.color = color1;
                    yield break;
                }
                curTime+= flickTime / 2f;
                mat.color = isChange ? color1 : color2;
                isChange = !isChange;
                SoundManager.Instance.PlayEffect_RPC("ContaminationFieldWarn");
                yield return new WaitForSeconds(0.5f);
            }
                
            if (transform.GetChild(indexs[0]).gameObject.activeSelf)
            {
                // 오염 필드 켜주기                
                nextTargetAfterObj.SetActive(true);
                nextHostField.SetIndex(indexs[0] + indexs[1]);
                nextHostField.DeleteHost(indexs[0]);


                // 기존 필드 꺼주기
                nextTargetBeforeObj.gameObject.SetActive(false);
            }            
        }
    }
}
