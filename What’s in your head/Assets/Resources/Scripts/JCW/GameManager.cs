using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviour
{    
    // 좌측 bool 값은 master client인지, 우측 bool 값은 Nella 캐릭터인지.    
    [HideInInspector] public Dictionary<bool, bool> characterOwner = new();

    [HideInInspector] public int curStageIndex = 0;
    [HideInInspector] public bool isAlive;

    private int _curSection = 0;
    public int curSection { get { return _curSection; } private set { _curSection = value; } }

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

        curStageIndex = 0;
        curSection = 0;
    }
    public void SectionUP() { ++curSection;  }

}
