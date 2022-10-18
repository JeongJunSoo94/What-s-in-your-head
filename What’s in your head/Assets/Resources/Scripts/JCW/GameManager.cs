using System.Collections;
using System.Collections.Generic;
using JCW.UI.InGame;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class GameManager : MonoBehaviour
{    
    // 좌측 bool 값은 master client인지, 우측 bool 값은 Nella 캐릭터인지.    
    [HideInInspector] public Dictionary<bool, bool> characterOwner = new();
    //[HideInInspector] public readonly List<bool> aliveList = new();

    [HideInInspector] public int curStageIndex = 0;
    [HideInInspector] public Hashtable isAlive = new();
    [HideInInspector] public Dictionary<bool, HealthUI> reviveAllPairs= new();
    public int curSection { get; private set; }
    public int curPlayerHP = 12;

    PhotonView photonView;

    public static GameManager Instance;
    private void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);

        photonView = GetComponent<PhotonView>();

        curStageIndex = 0;
        curSection = 0;        
        //isAlive.Add(characterOwner[PhotonNetwork.IsMasterClient], true);
        //isAlive.Add(!characterOwner[PhotonNetwork.IsMasterClient], true);        
    }
    public void SectionUP() { ++curSection;  }

    public void MediateRevive(bool value)
    {
        photonView.RPC(nameof(MediateRevive_RPC), RpcTarget.AllViaServer, value);
    }

    [PunRPC]
    void MediateRevive_RPC(bool value)
    {
        reviveAllPairs[true].SetRevive(value);
        reviveAllPairs[false].SetRevive(value);
    }

    public void AddReviveAllPair(bool _isNella, GameObject healthUIObj)
    {
        photonView.RPC(nameof(AddReviveAllPair_RPC), RpcTarget.AllViaServer, _isNella, healthUIObj);
    }

    [PunRPC]
    void AddReviveAllPair_RPC(bool _isNella, GameObject healthUIObj)
    {
        reviveAllPairs.Add(_isNella, healthUIObj.GetComponent<HealthUI>());
    }
}
