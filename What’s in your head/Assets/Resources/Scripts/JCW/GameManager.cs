using System.Collections;
using System.Collections.Generic;
using JCW.UI.InGame;
using Photon.Pun;
using UnityEngine;
using YC.Photon;

[RequireComponent(typeof(PhotonView))]
public class GameManager : MonoBehaviour, IPunObservable
{    
    // 좌측 bool 값은 master client인지, 우측 bool 값은 Nella 캐릭터인지.    
    [HideInInspector] public Dictionary<bool, bool> characterOwner = new();   

    // 현재 스테이지 , 섹션 인덱스
    [HideInInspector] public int curStageIndex = 0;
    [HideInInspector] public int curSection;
    
    // 현재 캐릭터들의 생존 여부
    [HideInInspector] public Hashtable isAlive = new();

    // 좌측 bool 값은 Nella 캐릭터인지, 우측은 해당 캐릭터가 가지고 있는 스크립트
    [HideInInspector] public Dictionary<bool, HealthUI> reviveAllPairs= new();
    [HideInInspector] public Dictionary<bool, CharUI> hpAllPairs= new();

    // Remote인 다른 캐릭터의 위치
    [HideInInspector] public Transform otherPlayerTF;
    // Owner인 내 캐릭터의 위치
    [HideInInspector] public Transform myPlayerTF;

    // 현재 탑뷰인지
    [Header("탑뷰")] public bool isTopView;
    [Header("테스트용")] public bool isTest;

    // 랜덤시드
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

    // 누구 하나 죽었거나, 죽음->부활일 때 작동하는 함수=====================
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


    // HP UI 켜고 끄는 함수===================================================

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

    // 각 캐릭터가 살아있는 지 죽어있는지, 양 컴퓨터의 게임매니저를 통해 값 변경
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

    // 각 캐릭터가 살아있는 지 죽어있는지, 양 컴퓨터의 게임매니저를 통해 추가
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
        // 보내는 사람
        if (stream.IsWriting)
        {
            stream.SendNext(isTopView);
            stream.SendNext(curStageIndex);            
            stream.SendNext(curSection);
        }

        // 받는 사람
        else
        {
            isTopView                                   = (bool)stream.ReceiveNext();
            curStageIndex                               = (int)stream.ReceiveNext();
            curSection                                  = (int)stream.ReceiveNext();
        }
    }
}
