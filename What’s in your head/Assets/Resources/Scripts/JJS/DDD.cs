using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JJS;
public class DDD : MonoBehaviour
{
    Text bulletCount; 
    // Start is called before the first frame update
    void Start()
    {
        bulletCount = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        bulletCount.text = this.gameObject.transform.root.GetComponent<NellaMouseController>().bulletCount.ToString();
    }
}
