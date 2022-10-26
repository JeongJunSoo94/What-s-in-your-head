using System.Collections;
using System.Collections.Generic;
using JCW.UI.InGame;
using Photon.Pun;
using UnityEngine;
using YC.Photon;

[RequireComponent(typeof(PhotonView))]
public class GameManager : MonoBehaviour, IPunObservable
{    
    // ���� bool ���� master client����, ���� bool ���� Nella ĳ��������.    
    [HideInInspector] public Dictionary<bool, bool> characterOwner = new();   

    // ���� �������� , ���� �ε���
    [HideInInspector] public int curStageIndex = 0;
    [HideInInspector] public int curSection;
    
    // ���� ĳ���͵��� ���� ����
    [HideInInspector] public Hashtable isAlive = new();

    // ���� bool ���� Nella ĳ��������, ������ �ش� ĳ���Ͱ� ������ �ִ� ��ũ��Ʈ
    [HideInInspector] public Dictionary<bool, HealthUI> reviveAllPairs= new();
    [HideInInspector] public Dictionary<bool, CharUI> hpAllPairs= new();

    // Remote�� �ٸ� ĳ������ ��ġ
    [HideInInspector] public Transform otherPlayerTF;
    // Owner�� �� ĳ������ ��ġ
    [HideInInspector] public Transform myPlayerTF;

    // ���� ž������
    [Header("ž��")] public bool isTopView;
    [Header("�׽�Ʈ��")] public bool isTest;

    // �����õ�
    [HideInInspector] public int randomSeed { get; private set; }

    public int curPlayerHP = 12;

    PhotonView photonView;

    public static GameManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);

        photonView = GetComponent<PhotonView>();

        //{
        //    GameObject obj = new GameObject("_GameManager");
        //    photonView = obj.AddComponent<PhotonView>();
        //    photonView.ViewID = PhotonNetwork.AllocateViewID(0);
        //    photonView.observableSearch = PhotonView.ObservableSearch.AutoFindAll;
        //}

        curStageIndex = 0;
        curSection = 0;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad0))
            isTopView = !isTopView;
    }

    public void SetRandomSeed()
    {
        photonView.RPC(nameof(SetRandomSeed_RPC), RpcTarget.AllViaServer, Random.Range(0, 2147483640));
    }

    [PunRPC]
    void SetRandomSeed_RPC(int seed)
    {
        if(PhotonNetwork.IsMasterClient)
            randomSeed = seed;
    }

    public void SectionUP() { ++curSection;  }

    // ���� �ϳ� �׾��ų�, ����->��Ȱ�� �� �۵��ϴ� �Լ�=====================
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
    // ====================================================================


    // HP UI �Ѱ� ���� �Լ�===================================================

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
    //=======================================================================

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
    //==========================================================================

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
    //===========================================================================

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // ������ ���
        if (stream.IsWriting)
        {
            stream.SendNext(isTopView);
            stream.SendNext(curStageIndex);            
            stream.SendNext(curSection);
        }

        // �޴� ���
        else
        {
            isTopView                                   = (bool)stream.ReceiveNext();
            curStageIndex                               = (int)stream.ReceiveNext();
            curSection                                  = (int)stream.ReceiveNext();
        }
    }
}
