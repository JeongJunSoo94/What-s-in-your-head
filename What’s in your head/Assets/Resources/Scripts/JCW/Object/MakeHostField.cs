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
        [Header("===========���� ��===========")]
        [Header("ù ��° ���� �ð�")][SerializeField] float firstSpawnTime = 40f;
        [Header("�� ��° ���� �ð�")][SerializeField] float secondSpawnTime = 120f;
        [Header("�����Ÿ��� �ð�")][SerializeField] float flickTime = 1f;
        [Header("���� �ʵ� ���� �ҿ� �ð�")][SerializeField] float infectTime = 5f;
        [Header("���� ���� ����")][SerializeField] bool canInfect = true;

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

            // ������ �� �ִ� �� ������ ���صα�
            firstSpawnPlace = new() { 1, 2*usingCount-1, 2*usingCount*(usingCount-1)+1, 2*usingCount*usingCount-1 };

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
            Debug.Log("��ȭ��ų �ε��� : " + myIndex);
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
                // ���� �̹� ��ȭ�Ǿ����� ���� �� �̻� �Ƚ�Ŵ
                // �̹� ������ �� ������ ���� �Ƚ�Ŵ
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
                // ���� �ʵ� ���ֱ�                
                nextTargetAfterObj.SetActive(true);
                nextHostField.SetIndex(indexs[0] + indexs[1]);
                nextHostField.DeleteHost(indexs[0]);


                // ���� �ʵ� ���ֱ�
                nextTargetBeforeObj.gameObject.SetActive(false);
            }            
        }
    }
}
