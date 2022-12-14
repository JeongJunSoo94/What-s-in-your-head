using System.Collections;
using System.Collections.Generic;
using JCW.AudioCtrl;
using JCW.Dialog;
using JCW.Network;
using JCW.UI;
using JCW.UI.InGame;
using JCW.UI.Options.InputBindings;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using YC.YC_CameraManager;
using YC.Photon;

[RequireComponent(typeof(PhotonView))]
public class GameManager : MonoBehaviour, IPunObservable
{    
    // 좌측 bool 값은 master client인지, 우측 bool 값은 Nella 캐릭터인지.    
    [HideInInspector] public Dictionary<bool, bool> characterOwner = new();   

    // 현재 스테이지 인덱스
    public int curStageIndex = 0; // 기본값 0

    // 보통 0~3 => Intro / Section1 / Section2 / Outro
    public int curStageType = 0; // 기본값 1

    // 체크포인트 순서
    [HideInInspector] public int curSection = 0;
    
    // 현재 캐릭터들의 생존 여부
    [HideInInspector] public Hashtable isAlive = new();

    // 좌측 bool 값은 Nella 캐릭터인지, 우측은 해당 캐릭터가 가지고 있는 스크립트
    [HideInInspector] public Dictionary<bool, HealthUI> healthUIPairs= new();
    [HideInInspector] public Dictionary<bool, CharUI> charUIPairs= new();

    // Remote인 다른 캐릭터의 위치
    [HideInInspector] public Transform otherPlayerTF;
    // Owner인 내 캐릭터의 위치
    [HideInInspector] public Transform myPlayerTF;

    [HideInInspector] public List<bool> isCharOnScene = new();

    // PauseUI
    [Header("일시정지 UI 프리팹")] public GameObject pauseUI = null;

    // 현재 탑뷰인지
    [Header("탑뷰")] public bool isTopView;
    [Header("사이드뷰")] public bool isSideView;
    [Header("테스트용")] public bool isTest;

    // 랜덤시드
    [HideInInspector] public int randomSeed { get; private set; }

    readonly List<Object> stayingOnSceneList = new();

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
            pauseUI.SetActive(false);
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);

        photonView = GetComponent<PhotonView>();
        //curStageIndex = 0;
        //curStageType = 0;
        isCharOnScene.Add(false);
        isCharOnScene.Add(false);
    }
    private void Update()
    {        
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
    public void MediateRevive_RPC(bool value)
    {
        healthUIPairs[true].SetRevive(value);
        healthUIPairs[false].SetRevive(value);
    }
    // ====================================================================


    // HP UI 켜고 끄는 함수===================================================

    public void MediateHP(bool value)
    {
        photonView.RPC(nameof(MediateHP_RPC),RpcTarget.AllViaServer, value);
        //StartCoroutine(WaitForHP(value));        
    }

    [PunRPC]
    void MediateHP_RPC(bool value)
    {
        StartCoroutine(WaitForHP(value));
    }

    IEnumerator WaitForHP(bool value)
    {
        yield return new WaitUntil(() => GetCharOnScene());
        yield return new WaitUntil(() => charUIPairs.Count == 2);
        charUIPairs[true].SetHP(value);
        charUIPairs[false].SetHP(value);
        yield break;
    }

    //[PunRPC]
    //void MediateHP_RPC(bool value)
    //{
    //    charUIPairs[true].SetHP(value);
    //    charUIPairs[false].SetHP(value);
    //}
    //=======================================================================

    // 각 캐릭터가 살아있는 지 죽어있는지, 양 컴퓨터의 게임매니저를 통해 값 변경
    public void SetAliveState(bool _isNella, bool _value)
    {
        SetAlive(_isNella, _value);
        photonView.RPC(nameof(SetAlive), RpcTarget.Others, _isNella, _value);
    }

    [PunRPC]
    public void SetAlive(bool _isNella, bool _value)
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
        if(isAlive.Count < 2)
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
            stream.SendNext(aliceHP);
        }

        // 받는 사람
        else
        {
            isTopView     = (bool)stream.ReceiveNext();
            curStageIndex = (int)stream.ReceiveNext();
            curStageType  = (int)stream.ReceiveNext();
            curSection    = (int)stream.ReceiveNext();
            aliceHP       = (int)stream.ReceiveNext();
        }
    }

    public bool GetCharOnScene(bool isMaster)
    {
        return isCharOnScene[isMaster ? 0 : 1];
    }

    public bool GetCharOnScene()
    {
        return myPlayerTF != null && otherPlayerTF != null;
    }

    public void GoMainMenu_RPC()
    {
        photonView.RPC(nameof(GoMainMenu), RpcTarget.AllViaServer);
    }
    [PunRPC]
    void GoMainMenu()
    {
        Destroy(PhotonManager.Instance.gameObject);
        Destroy(CameraManager.Instance.gameObject);
        Destroy(DialogManager.Instance.gameObject);
        Destroy(SoundManager.Instance.gameObject);
        Destroy(pauseUI);
        Destroy(LoadingUI.Instance.gameObject);
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(0);        
        Destroy(GameManager.Instance.gameObject);
    }

    public void LoadCheckPoint()
    {
        photonView.RPC(nameof(LoadCP), RpcTarget.AllViaServer);
    }

    [PunRPC]
    void LoadCP()
    {
        myPlayerTF.GetComponent<KSU.PlayerController>().Resurrect();
    }

    public void ResetDefault_RPC()
    {
        photonView.RPC(nameof(ResetDefault), RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void ResetDefault()
    {
        otherPlayerTF = null;
        myPlayerTF = null;
        isTopView = false;
        isSideView = false;
        curPlayerHP = 12;
        aliceHP = 30;
        isAlive.Clear();
        charUIPairs.Clear();
        healthUIPairs.Clear();
        SoundManager.Instance.dict3D.Clear();
        SoundManager.Instance.StopAllSound();
        curSection = 0;
    }
}
