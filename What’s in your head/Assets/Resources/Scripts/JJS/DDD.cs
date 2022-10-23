using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JJS.Weapon;
public class DDD : MonoBehaviour
{
    Text bulletCount;
    public WaterGun waterGun;
    // Start is called before the first frame update
    void Start()
    {
        bulletCount = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        bulletCount.text = waterGun.bulletCount.ToString();
    }
}
