using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class PhotonGameManager : MonoBehaviour
{
    public static PhotonGameManager instance = null;
    public bool isConnect = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    //void Start()
    //{
    //    //StartCoroutine(CreatePlayer());
    //}
    //
    //IEnumerator CreatePlayer()
    //{
    //    yield return new WaitUntil(() => isConnect);
    //
    //    GameObject playerTemp = PhotonNetwork.Instantiate("Prefab/PhotonTest/Player", Vector3.zero, Quaternion.identity, 0);
    //}
}
