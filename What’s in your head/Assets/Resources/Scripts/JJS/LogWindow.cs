using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
public class LogWindow : MonoBehaviour
{
    public string m_FilePath = @"PlayerLog.txt";
    Text logText;
    Text exceptionText;
    ScrollRect scrollRect;
    GameObject logObj;
    GameObject exceptionObj;
    GameObject scroll;
    GameObject canvasObj;
    StreamWriter sw;
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

        sw = new StreamWriter(m_FilePath, true);
        Application.logMessageReceived += SetLog;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            OnConsole();
        }
    }

    void OnConsole()
    {
        GameManager.Instance.isPause = !GameManager.Instance.isPause;
        GameManager.Instance.PauseActive(GameManager.Instance.isPause);
        UseWindow(GameManager.Instance.isPause);
    }


    public void SetLog(string logString, string stackTrace, LogType type)
    {
        string str = "["+DateTime.Now.ToString()+ "]";
        str += logString;
        str += stackTrace;
        logText.text += str+ "\n";
        sw.WriteLine(str);
        scrollRect.verticalNormalizedPosition = 0.0f;
        if (LogType.Exception == type)
        {
            OnConsole();
            logObj.SetActive(false);
            exceptionObj.SetActive(true);
            exceptionText.text= str;
        }
    }

    void OnDisable()
    {
        Application.logMessageReceived -= SetLog;
        sw.Close();
    }

    public void UseWindow(bool enable)
    {
        canvasObj.SetActive(enable);
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
