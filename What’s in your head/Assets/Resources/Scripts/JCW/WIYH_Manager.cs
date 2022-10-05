using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WIYH_Manager : MonoBehaviour
{
    [HideInInspector] public GameObject player1;
    [HideInInspector] public GameObject player2;
    [HideInInspector] public GameObject base_main;

    public static WIYH_Manager Instance = null;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            base_main = this.gameObject;
            DontDestroyOnLoad(this.gameObject);
        }
        else if(Instance != this)
            Destroy(this.gameObject);
    }

}
