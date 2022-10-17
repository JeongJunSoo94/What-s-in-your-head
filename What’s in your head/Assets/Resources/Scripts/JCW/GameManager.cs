using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviour
{    
    // ���� bool ���� master client����, ���� bool ���� Nella ĳ��������.    
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
