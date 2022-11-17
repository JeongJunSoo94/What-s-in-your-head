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
    // ���� bool ���� master client����, ���� bool ���� Nella ĳ��������.    
    [HideInInspector] public Dictionary<bool, bool> characterOwner = new();   

    // ���� �������� �ε���
    [HideInInspector] public int curStageIndex = 0; // �⺻�� 0

    // ���� 0~3 => Intro / Section1 / Section2 / Outro
    [HideInInspector] public int curStageType = 0; // �⺻�� 1

    // üũ����Ʈ ����
    [HideInInspector] public int curSection = 0;
    
    // ���� ĳ���͵��� ���� ����
    [HideInInspector] public Hashtable isAlive = new();

    // ���� bool ���� Nella ĳ��������, ������ �ش� ĳ���Ͱ� ������ �ִ� ��ũ��Ʈ
    [HideInInspector] public Dictionary<bool, HealthUI> reviveAllPairs= new();
    [HideInInspector] public Dictionary<bool, CharUI> hpAllPairs= new();

    // Remote�� �ٸ� ĳ������ ��ġ
    [HideInInspector] public Transform otherPlayerTF;
    // Owner�� �� ĳ������ ��ġ
    [HideInInspector] public Transform myPlayerTF;

    // PauseUI
    [Header("�Ͻ����� UI ������")] public GameObject pauseUI = null;

    // ���� ž������
    [Header("ž��")] public bool isTopView;
    [Header("���̵��")] public bool isSideView;
    [Header("�׽�Ʈ��")] public bool isTest;

    // �����õ�
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
            stream.SendNext(curStageType);            
            stream.SendNext(curSection);
        }

        // �޴� ���
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
