using System.Collections;
using System.Collections.Generic;
using JCW.UI;
using JCW.UI.InGame;
using JCW.UI.Options.InputBindings;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using YC.Photon;

[RequireComponent(typeof(PhotonView))]
public class GameManager : MonoBehaviour, IPunObservable
{    
    // 좌측 bool 값은 master client인지, 우측 bool 값은 Nella 캐릭터인지.    
    [HideInInspector] public Dictionary<bool, bool> characterOwner = new();   

    // 현재 스테이지 인덱스
    [HideInInspector] public int curStageIndex = 0; // 기본값 0

    // 보통 0~3 => Intro / Section1 / Section2 / Outro
    [HideInInspector] public int curStageType = 0; // 기본값 1

    // 체크포인트 순서
    [HideInInspector] public int curSection = 0;
    
    // 현재 캐릭터들의 생존 여부
    [HideInInspector] public Hashtable isAlive = new();

    // 좌측 bool 값은 Nella 캐릭터인지, 우측은 해당 캐릭터가 가지고 있는 스크립트
    [HideInInspector] public Dictionary<bool, HealthUI> reviveAllPairs= new();
    [HideInInspector] public Dictionary<bool, CharUI> hpAllPairs= new();

    // Remote인 다른 캐릭터의 위치
    [HideInInspector] public Transform otherPlayerTF;
    // Owner인 내 캐릭터의 위치
    [HideInInspector] public Transform myPlayerTF;

    // PauseUI
    [Header("일시정지 UI 프리팹")] public GameObject pauseUI = null;

    // 현재 탑뷰인지
    [Header("탑뷰")] public bool isTopView;
    [Header("사이드뷰")] public bool isSideView;
    [Header("테스트용")] public bool isTest;

    // 랜덤시드
    [HideInInspector] public int randomSeed { get; private set; }

    List<Object> stayingOnSceneList = new();

    public int curPlayerHP = 12;
    public int aliceHP = 30;

    PhotonView photonView;

    public static GameManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            pauseUI = pauseUI == null ? Resources.Load<GameObject>("Prefabs/JCW/UI/InGame/PauseUI") : Instantiate(pauseUI);
            DontDestroyOnLoad(pauseUI);
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);

        photonView = GetComponent<PhotonView>();
        curStageIndex = 0;
        curStageType = 0;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
            isTopView = !isTopView;
        if (Input.GetKeyDown(KeyCode.Keypad2))
            isSideView = !isSideView;
        if (KeyManager.Instance.GetKeyDown(PlayerAction.Pause))
        {
            if(SceneManager.GetActiveScene().name != "MainTitle")
            {
                if (!pauseUI.activeSelf)
                    pauseUI.SetActive(true);
                else
                    PauseManager.Instance.CloseUI();
            }            
        }
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
            stream.SendNext(curStageType);            
            stream.SendNext(curSection);
        }

        // 받는 사람
        else
        {
            isTopView     = (bool)stream.ReceiveNext();
            curStageIndex = (int)stream.ReceiveNext();
            curStageType  = (int)stream.ReceiveNext();
            curSection    = (int)stream.ReceiveNext();
        }
    }

    public void GoMainMenu_RPC()
    {
        photonView.RPC(nameof(GoMainMenu), RpcTarget.AllViaServer);
    }
    [PunRPC]
    void GoMainMenu()
    {
        pauseUI.SetActive(false);
        curStageIndex = 0;
        curStageType = 0;
        LoadingUI.Instance.gameObject.SetActive(true);
        DestroyStayingObj();
    }

    public void DonDestroy(Object obj)
    {
        DontDestroyOnLoad(obj);
        stayingOnSceneList.Add(obj);
    }

    public void DestroyStayingObj_RPC()
    {
        photonView.RPC(nameof(DestroyStayingObj), RpcTarget.AllViaServer);
    }
    [PunRPC]
    public void DestroyStayingObj()
    {
        for (int i=0 ; i<stayingOnSceneList.Count ; ++i)
        {
            Destroy(stayingOnSceneList[i]);
        }
        stayingOnSceneList.Clear();
    }
}
