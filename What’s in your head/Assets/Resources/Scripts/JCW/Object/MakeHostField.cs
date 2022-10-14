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

        int usingCount;
        List<int> firstSpawnPlace;
        float elapsedTime = 0f;

        int curPhase = 0;
        int randomIndex = 0;

        PhotonView photonView;

        private void Awake()
        {
            photonView = this.gameObject.GetComponent<PhotonView>();
            usingCount = this.gameObject.GetComponent<ContaminationFieldSetting>().count;

            int N = 2*usingCount - 1;

            // 스폰될 수 있는 각 꼭지점 정해두기
            firstSpawnPlace = new() { 1, 2*N-1, 2*N*(N-1)+1, 2*N*N-1 };

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
            transform.GetChild(firstSpawnPlace[0]).gameObject.SendMessage("SetIndex", firstSpawnPlace[0]);
            transform.GetChild(firstSpawnPlace[0]).gameObject.SendMessage("SetStart");

            transform.GetChild(firstSpawnPlace[1] - 1).gameObject.SetActive(false);
            transform.GetChild(firstSpawnPlace[1]).gameObject.SetActive(true);
            transform.GetChild(firstSpawnPlace[1]).gameObject.SendMessage("SetIndex", firstSpawnPlace[1]);
            transform.GetChild(firstSpawnPlace[1]).gameObject.SendMessage("SetStart");

            firstSpawnPlace.RemoveRange(0, 2);
            //++curPhase;
        }

        [PunRPC]
        void SpawnSecond()
        {
            transform.GetChild(firstSpawnPlace[0] - 1).gameObject.SetActive(false);
            transform.GetChild(firstSpawnPlace[0]).gameObject.SetActive(true);
            transform.GetChild(firstSpawnPlace[0]).gameObject.SendMessage("SetIndex", firstSpawnPlace[0]);
            transform.GetChild(firstSpawnPlace[0]).gameObject.SendMessage("SetStart");
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
            // 오염 필드 켜주기
            transform.GetChild(myIndex + nextTarget).gameObject.SetActive(true);
            transform.GetChild(myIndex + nextTarget).gameObject.SendMessage("SetIndex", myIndex + nextTarget);
            transform.GetChild(myIndex + nextTarget).gameObject.SendMessage("DeleteHost", myIndex);


            // 기존 필드 꺼주기
            transform.GetChild(myIndex + nextTarget - 1).gameObject.SetActive(false);
        }

        public void SetPurified(int myIndex)
        {
            photonView.RPC(nameof(SetPurifiedRPC), RpcTarget.AllViaServer, myIndex);
        }

        [PunRPC]
        void SetPurifiedRPC(int myIndex)
        {
            transform.GetChild(myIndex - 1).gameObject.SetActive(true);
        }
    }
}
