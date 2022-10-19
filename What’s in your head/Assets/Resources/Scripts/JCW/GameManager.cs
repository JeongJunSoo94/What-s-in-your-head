using System.Collections;
using System.Collections.Generic;
using JCW.UI.InGame;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class GameManager : MonoBehaviour
{    
    // ���� bool ���� master client����, ���� bool ���� Nella ĳ��������.    
    [HideInInspector] public Dictionary<bool, bool> characterOwner = new();

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
        photonView.RPC(nameof(Mediate), RpcTarget.AllViaServer, value);
    }

    [PunRPC]
    void Mediate(bool value)
    {
        reviveAllPairs[true].SetRevive(value);
        reviveAllPairs[false].SetRevive(value);
    }

    public void AddReviveAllPair(bool isNellaValue, string hUIOwnerName)
    {
        photonView.RPC(nameof(AddRevive), RpcTarget.AllViaServer, isNellaValue, hUIOwnerName);
    }

    [PunRPC]
    void AddRevive(bool isNellaValue, string hUIOwnerName)
    {
        GameObject go = GameObject.Find(hUIOwnerName);        
        reviveAllPairs.Add(isNellaValue, go.GetComponentInChildren<HealthUI>());
    }

    public void CheckAliveState(bool _isNella, bool _value)
    {
        CheckAlive(_isNella, _value);
        photonView.RPC(nameof(CheckAlive), RpcTarget.Others, _isNella, _value);
    }

    [PunRPC]
    void CheckAlive(bool _isNella, bool _value)
    {
        isAlive[_isNella] = _value;
    }

    public void AddAliveState(bool _isNella, bool _value)
    {
        photonView.RPC(nameof(AddAlive), RpcTarget.AllViaServer, _isNella, _value);
    }

    [PunRPC]
    void AddAlive(bool _isNella, bool _value)
    {
        isAlive.Add(_isNella, _value);
    }
}
