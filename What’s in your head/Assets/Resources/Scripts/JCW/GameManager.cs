using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public GameObject player1;
    [HideInInspector] public GameObject player2;
    [HideInInspector] public GameObject base_main;

    public int currentStageIndex = 1;
    public string stopPlayerName = "";
    public bool isPlaying = false;

    public static GameManager Instance = null;
    private void Awake()
    {
        if (Instance == null)
        {
            currentStageIndex = 1;
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if(Instance != this)
            Destroy(this.gameObject);
    }

}
