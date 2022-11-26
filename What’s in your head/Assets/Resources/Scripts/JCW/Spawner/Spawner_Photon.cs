using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.Spawner
{
    public class Spawner_Photon : Spawner
    {
        [Header("���� �ν��Ͻ�ȭ �� ������ ���")] [SerializeField] string prefabDirectory = "Prefabs/JCW/Object/Stage2/SandSackBullet";
        GameObject spawned = null;
        WaitUntil wu;
        private void Awake()
        {
            if(prefabDirectory == null)
                prefabDirectory = "Prefabs/JCW/Object/Stage2/SandSackBullet";
            wu = new(()=> spawned != null);
        }
        override public void SpawnInit()
        {
            if (objQueue != null)
                return;
            objQueue = new Queue<GameObject>();
            for (int i = 0 ; i < count ; ++i)
            {
                GameObject spawned = PhotonNetwork.Instantiate(prefabDirectory, this.transform.position, this.transform.rotation);
                spawned.SetActive(false);
                objQueue.Enqueue(spawned);
            }
        }
    }

}
