using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
public class LogWindow : MonoBehaviour
{
    bool use;
    public string m_FilePath1 = @"PlayerLog.txt";
    public string m_FilePath2 = @"PlayerExceptionLog.txt";
    Text logText;
    Text exceptionText;
    ScrollRect scrollRect;
    GameObject logObj;
    GameObject exceptionObj;
    GameObject scroll;
    GameObject canvasObj;
    StreamWriter sw1;
    StreamWriter sw2;
    public GameObject button;
    private void Awake()
    {
        SetGameObject(gameObject,out logObj, "LogText");
        logText = logObj?.GetComponent<Text>();
        SetGameObject(gameObject, out exceptionObj, "ExceptionText");
        exceptionText = exceptionObj?.GetComponent<Text>();
        SetGameObject(gameObject, out scroll, "ScrollView");
        scrollRect = scroll?.GetComponent<ScrollRect>();
        SetGameObject(gameObject, out canvasObj, "Canvas");
        canvasObj.SetActive(false);

        sw1 = new StreamWriter(m_FilePath1, true);
        sw2 = new StreamWriter(m_FilePath2, true);
        Application.logMessageReceived += SetLog;

        //scroll.transform.position= new Vector3(0, 0,0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            use = !use;
            UseWindow(use);
        }
    }

    public void SetLog(string logString, string stackTrace, LogType type)
    {
        string str = "["+DateTime.Now.ToString()+ "]";
        str += logString;
        if (LogType.Exception == type)
        {
            str += stackTrace;
            UseWindow(true);
            logObj.SetActive(false);
            exceptionObj.SetActive(true);
            exceptionText.color = Color.red;
            exceptionText.text = str;
            sw2.WriteLine(str);
        }
        else
        {
            logText.text += str+"\n";
            sw1.WriteLine(str);
        }
        scrollRect.verticalNormalizedPosition = 0.0f;

    }

    void OnDisable()
    {
        Application.logMessageReceived -= SetLog;
        sw1.Flush();
        sw2.Flush();
        sw1.Close();
        sw2.Close();
    }

    public void UseWindow(bool enable)
    {
        canvasObj.SetActive(enable);
        CursorUse(enable);
        use = enable;
    }

    public void CursorUse(bool enable)
    {
        button.SetActive(enable);
        if (enable)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void SetGameObject(GameObject findObject, out GameObject discoverObject, string findName)
    {
        discoverObject = null;
        Transform[] allChildren = findObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.name == findName)
            {
                discoverObject = child.gameObject;
                return;
            }
        }
    }
}
