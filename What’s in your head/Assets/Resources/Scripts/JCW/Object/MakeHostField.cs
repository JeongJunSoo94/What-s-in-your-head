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
        [Header("===========���� ��===========")]
        [Header("ù ��° ���� �ð�")][SerializeField] float firstSpawnTime = 40f;
        [Header("�� ��° ���� �ð�")][SerializeField] float secondSpawnTime = 120f;

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

            // ������ �� �ִ� �� ������ ���صα�
            firstSpawnPlace = new() { 1, 2*N-1, 2*N*(N-1)+1, 2*N*N-1 };

            if (PhotonNetwork.IsMasterClient)
            {
                // �� �� 1���� �����ϰ� �����
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
            // ���� �ʵ� ���ֱ�
            transform.GetChild(myIndex + nextTarget).gameObject.SetActive(true);
            transform.GetChild(myIndex + nextTarget).gameObject.SendMessage("SetIndex", myIndex + nextTarget);
            transform.GetChild(myIndex + nextTarget).gameObject.SendMessage("DeleteHost", myIndex);


            // ���� �ʵ� ���ֱ�
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
