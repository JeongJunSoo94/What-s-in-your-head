using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace YC.DebugbyText
{ 
    public class Debuger : MonoBehaviour
    {
        [SerializeField] Text nellaText;
        [SerializeField] Text steadyText;
        [SerializeField] Text nellaIsMineText;
        [SerializeField] Text steadyIsMineText;


        GameObject nellaInstance = null;
        GameObject steadyInstance = null;

        void Update()
        {
            SetInstance();
        }

        void SetInstance()
        {
            if (!nellaInstance)
            {
                nellaInstance = GameObject.Find("Nella(Clone)");
            }
            else
            {
                SetText(nellaInstance, nellaText);
            }


            if (!steadyInstance) steadyInstance = GameObject.Find("Steady(Clone)");
            else SetText(steadyInstance, steadyText);
        }

        void SetText(GameObject obj, Text txt)
        {
            txt.text = string.Format("Name : {0}\nPos : {1}", obj.gameObject.name, obj.transform.position);
                
            if (!obj.GetComponent<PhotonView>().IsMine) return;

            if (obj.name == "Nella(Clone)")
            {
                nellaIsMineText.text = string.Format("[Is Mine]");
            }
            else if (obj.name == "Steady(Clone)")
            {
                steadyIsMineText.text = string.Format("[Is Mine]");
            }
        }
    }
}

