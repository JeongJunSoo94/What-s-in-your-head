using System.Collections.Generic;
using UnityEngine;

namespace JCW.UI.Options.InputBindings
{
    public class ITT_KeyManager : MonoBehaviour
    {
        private Dictionary<PlayerAction, KeyState> curKeySet;

        // 싱글톤
        private static ITT_KeyManager sInstance;
        public static ITT_KeyManager Instance
        {
            get
            {
                if (sInstance == null)
                {
                    GameObject newGameObject = new("_KeyManager");
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

        // 옵션에서 설정한 키로 키 매니저의 키 값을 변경.
        public void KeySet(InputBinding _keySet)
        {
            curKeySet = _keySet.Bindings;
        }

        // 키 값 초기화
        void KeyInit()
        {
            curKeySet = new InputBinding(true).Bindings;
        }

        void Update()
        {
            if(Time.timeScale != 0.0f)
                KeyManager();
        }

        void KeyManager()
        {
            foreach(PlayerAction act in curKeySet.Keys)
                KeyInputCheck(Input.GetKey(curKeySet[act].keyCode), curKeySet[act]);
        }

        void KeyInputCheck(bool _isActiveKey, KeyState _keyState)
        {
            if (_isActiveKey)
            {
                _keyState.keyDown = !_keyState.keyOn;
                _keyState.keyOn = true;
            }
            else
            {
                _keyState.keyOn = false;
                _keyState.keyDown = false;
            }
        }

        public bool GetKey(PlayerAction actionCode)
        {
            return curKeySet[actionCode].keyOn;
        }

        public bool GetKeyDown(PlayerAction actionCode)
        {
            return curKeySet[actionCode].keyDown;
        }
    }
}
