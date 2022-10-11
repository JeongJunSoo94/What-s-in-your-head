using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ���� ������� �ʾƼ� ���� ���� ==================
    [HideInInspector] public GameObject player1;
    [HideInInspector] public GameObject player2;
    [HideInInspector] public GameObject base_main;
    // ============================================
    
    // ���� bool ���� master client����, ���� bool ���� Nella ĳ��������.    
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
