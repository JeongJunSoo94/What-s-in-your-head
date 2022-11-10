using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSwitch : MonoBehaviour
{
    public GameObject boss;
    public bool enable;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer==LayerMask.NameToLayer("Player"))
            boss?.SetActive(enable);
    }
}
