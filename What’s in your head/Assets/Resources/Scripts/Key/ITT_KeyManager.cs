using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum KeyName { W, S, D, A, F, E, LeftShift, CapsLock, Space, Mouse0 }

public class ITT_KeyManager : MonoBehaviour
{
    // ΩÃ±€≈Ê
    private static ITT_KeyManager sInstance;
    public static ITT_KeyManager Instance
    {
        get
        {
            if (sInstance == null)
            {
                GameObject newGameObject = new("_ITT_KeyManager");
                sInstance = newGameObject.AddComponent<ITT_KeyManager>();
            }
            return sInstance;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        KeyInit();
    }

    public class KeyInput
    {
        public KeyInput(KeyCode keyCode)
        {
            this.keyCode = keyCode;
        }
        public KeyCode keyCode;
        public bool keyOn = false;
        public bool keyDown = false;
    }

    public List<KeyInput> keyList= new();

    void KeyInit()
    {
        keyList.Add(new KeyInput(KeyCode.W));
        keyList.Add(new KeyInput(KeyCode.S));
        keyList.Add(new KeyInput(KeyCode.D));
        keyList.Add(new KeyInput(KeyCode.A));
        keyList.Add(new KeyInput(KeyCode.F));
        keyList.Add(new KeyInput(KeyCode.E));
        keyList.Add(new KeyInput(KeyCode.LeftShift));
        keyList.Add(new KeyInput(KeyCode.CapsLock));
        keyList.Add(new KeyInput(KeyCode.Space));
        keyList.Add(new KeyInput(KeyCode.Mouse0));
    }

    void Update()
    {
        KeyManager();
    }

    void KeyManager()
    {
        for (int i =0;i< keyList.Count;i++ )
        {
            KeyInputCheck(Input.GetKey(keyList[i].keyCode), keyList[i]);
        }

    }

    void KeyInputCheck(bool getKey,KeyInput keyInput)
    {
        if (getKey)
        {
            if (!keyInput.keyOn)
            {
                keyInput.keyDown = true;
            }
            else
            {
                keyInput.keyDown = false;
            }
            keyInput.keyOn = true;
        }
        else
        {
            keyInput.keyOn = false;
            keyInput.keyDown = false;
        }
    }

    public bool GetKey(KeyName keyCode)
    {
        return keyList[(int)keyCode].keyOn;
    }

    public bool GetKeyDown(KeyName keyCode)
    {
        return keyList[(int)keyCode].keyDown;
    }
}
