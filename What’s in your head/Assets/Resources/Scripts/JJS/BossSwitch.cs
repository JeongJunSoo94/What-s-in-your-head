using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSwitch : MonoBehaviour
{
    public GameObject boss;
    public bool enable;

    private void OnTriggerEnter(Collider other)
    {
        boss?.SetActive(enable);
    }
}
