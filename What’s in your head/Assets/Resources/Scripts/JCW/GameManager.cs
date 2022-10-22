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

    // ���� �������� , ���� �ε���
    [HideInInspector] public int curStageIndex = 0;
    [HideInInspector] public int curSection { get; private set; }
    
    // ���� ĳ���͵��� ���� ����
    [HideInInspector] public Hashtable isAlive = new();

    // ���� bool ���� Nella ĳ��������, ������ �ش� ĳ���Ͱ� ������ �ִ� ��ũ��Ʈ
    [HideInInspector] public Dictionary<bool, HealthUI> reviveAllPairs= new();
    [HideInInspector] public Dictionary<bool, CharUI> hpAllPairs= new();

    // Remote�� �ٸ� ĳ������ ��ġ
    [HideInInspector] public Transform otherPlayerTF;

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
    }
    public void SectionUP() { ++curSection;  }

    // ���� �ϳ� �׾��ų�, ����->��Ȱ�� �� �۵��ϴ� �Լ�
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

    // HP UI �Ѱ� ���� �Լ�

    public void MediateHP(bool value)
    {
        photonView.RPC(nameof(MediateHP_RPC), RpcTarget.AllViaServer, value);
    }

    [PunRPC]
    void MediateHP_RPC(bool value)
    {
        hpAllPairs[true].SetHP(value);
        hpAllPairs[false].SetHP(value);
    }

    // �� ĳ���Ͱ� ����ִ� �� �׾��ִ���, �� ��ǻ���� ���ӸŴ����� ���� �� ����
    public void SetAliveState(bool _isNella, bool _value)
    {
        SetAlive(_isNella, _value);
        photonView.RPC(nameof(SetAlive), RpcTarget.Others, _isNella, _value);
    }

    [PunRPC]
    void SetAlive(bool _isNella, bool _value)
    {
        isAlive[_isNella] = _value;
    }

    // �� ĳ���Ͱ� ����ִ� �� �׾��ִ���, �� ��ǻ���� ���ӸŴ����� ���� �߰�
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
