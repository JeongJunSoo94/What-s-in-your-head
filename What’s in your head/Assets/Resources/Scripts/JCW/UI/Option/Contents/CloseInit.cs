using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace JCW.UI.Options
{
    public class CloseInit : MonoBehaviour
    {
        private GameObject initWarning = null;
        private Button thisButton = null;
        private void Awake()
        {
            thisButton = this.gameObject.GetComponent<Button>();
            initWarning = this.transform.parent.gameObject.transform.parent.gameObject;
            thisButton.onClick.AddListener(() =>
            {
                initWarning.SetActive(false);
            });            
        }
    }
}