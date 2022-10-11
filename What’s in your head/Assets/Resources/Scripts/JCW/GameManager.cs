using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 현재 사용하지 않아서 삭제 예정 ==================
    [HideInInspector] public GameObject player1;
    [HideInInspector] public GameObject player2;
    [HideInInspector] public GameObject base_main;
    // ============================================
    
    // 좌측 bool 값은 master client인지, 우측 bool 값은 Nella 캐릭터인지.    
    [HideInInspector] public Dictionary<bool, bool> characterOwner = new();

    public int curStageIndex = 1;

    public static GameManager Instance = null;
    private void Awake()
    {
        if (Instance == null)
        {
            curStageIndex = 1;
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if(Instance != this)
            Destroy(this.gameObject);
    }

}
