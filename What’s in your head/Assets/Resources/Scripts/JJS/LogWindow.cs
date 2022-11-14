using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Text;

namespace JJS
{ 
    public class LogWindow : MonoBehaviour
    {
        bool use;
        public string m_FilePath1 = "PlayerLog.txt";
        public string m_FilePath2 = "PlayerExceptionLog.txt";
        public Text logText;
        public Text exceptionText;
        public ScrollRect scrollRect;
        public GameObject logObj;
        public GameObject exceptionObj;
        public GameObject scroll;
        public GameObject canvasObj;
        public StreamWriter sw1;
        public StreamWriter sw2;
        public GameObject button;
        readonly StringBuilder changedStr = new(500);
        readonly StringBuilder allLogText = new(100000);

        private void Awake()
        {
            SetGameObject(gameObject,out logObj, "LogText");
            logText = logObj?.GetComponent<Text>();
            SetGameObject(gameObject, out exceptionObj, "ExceptionText");
            exceptionText = exceptionObj?.GetComponent<Text>();
            SetGameObject(gameObject, out scroll, "ScrollView");
            scrollRect = scroll?.GetComponent<ScrollRect>();
            SetGameObject(gameObject, out canvasObj, "Canvas");
            SetGameObject(gameObject, out button, "Button");
            canvasObj.SetActive(false);

            Application.logMessageReceived += SetLog;
            CanvasScaler canvasScaler = canvasObj.GetComponent<CanvasScaler>();
            canvasScaler.referenceResolution =new Vector2(Screen.width, Screen.height);
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
            sw1 = new StreamWriter(m_FilePath1, true);
            sw2 = new StreamWriter(m_FilePath2, true);
            changedStr.Clear();
            changedStr.AppendFormat("[{0}]{1}\n", DateTime.Now.ToString(),logString);
            //string str = "["+DateTime.Now.ToString()+ "]";
            if (LogType.Exception == type)
            {
                changedStr.Append(stackTrace);
                UseWindow(true);
                logObj.SetActive(false);
                exceptionObj.SetActive(true);
                exceptionText.color = Color.red;
                exceptionText.text = changedStr.ToString();
                sw2.WriteLine(changedStr);
            }
            else
            {
                if (allLogText.Length >= 1000000)
                {
                    allLogText.Clear();
                }
                allLogText.Append(changedStr);
                logText.text = allLogText.ToString();
                sw1.Write(changedStr);
            }
            scrollRect.verticalNormalizedPosition = 0.0f;
            sw1.Close();
            sw2.Close();
        }

        void OnDisable()
        {
            Application.logMessageReceived -= SetLog;
            //sw1 = new StreamWriter(m_FilePath1, true);
            //sw2 = new StreamWriter(m_FilePath2, true);
            //sw1.Flush();
            //sw2.Flush();
            //sw1.Close();
            //sw2.Close();
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
}
