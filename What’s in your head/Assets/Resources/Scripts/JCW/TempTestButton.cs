using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempTestButton : MonoBehaviour
{
    int order = 1;
    public Text text;
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => 
        {
            text.text = JCW.Dialog.DialogManager.Instance.ReadDialogs(1, order++);
        });
    }
}
