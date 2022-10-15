using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{    
    // 좌측 bool 값은 master client인지, 우측 bool 값은 Nella 캐릭터인지.    
    [HideInInspector] public Dictionary<bool, bool> characterOwner = new();

    public int curStageIndex = 1;
    // 싱글톤
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
