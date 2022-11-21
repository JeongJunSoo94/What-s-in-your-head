using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyThis : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.DonDestroy(this.gameObject);
    }
}
