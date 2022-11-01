using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    ConeFindTarget find;
    private void Awake()
    {
        find=GetComponent<ConeFindTarget>();
    }
    private void Update()
    {
        
    }
}
