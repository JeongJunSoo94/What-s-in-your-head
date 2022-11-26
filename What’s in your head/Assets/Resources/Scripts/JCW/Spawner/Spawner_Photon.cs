using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.Spawner
{
    public class Spawner_Photon : Spawner
    {
        override public void SpawnInit()
        {
            if (objQueue != null)
                return;
            objQueue = new Queue<GameObject>();
            for (int i = 0; i < transform.childCount; ++i)
            {
                objQueue.Enqueue(transform.GetChild(i).gameObject);
            }
        }
    }

}
