using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{    
    // ���� bool ���� master client����, ���� bool ���� Nella ĳ��������.    
    [HideInInspector] public Dictionary<bool, bool> characterOwner = new();

    public int curStageIndex = 1;
    // �̱���
    private static GameManager sInstance;
    public static GameManager Instance
    {
        get
        {
            if (sInstance == null)
            {
                GameObject newGameObject = new("_GameManager");
                sInstance = newGameObject.AddComponent<GameManager>();
            }
            return sInstance;
        }
    }
    private void Awake()
    {
        curStageIndex = 1;
        DontDestroyOnLoad(this.gameObject);
    }

}
