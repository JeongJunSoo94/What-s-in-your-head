using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseDiscovery3D : MonoBehaviour
{
    public List<GameObject> discoveryObject;

    public void OnEnableObject(int index)
    {
        discoveryObject[index].SetActive(true);
    }

    public void OnDisableObject(int index)
    {
        discoveryObject[index].SetActive(false);
    }


}
