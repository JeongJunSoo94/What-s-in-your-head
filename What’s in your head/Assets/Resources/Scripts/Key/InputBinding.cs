using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using LitJson;

namespace JCW.InputBindings
{
    public enum PlayerAction
    {
        MoveForward,
        MoveBackward,
        MoveLeft,
        MoveRight,

        MouseLeft,
        MouseRight,

        Crouch,
        Dash,
        Jump,
    }

    [Serializable]
    public class SerializableInputBinding
    {
        public BindPair[] bindPairs;

        // 생성자
        public SerializableInputBinding(InputBinding binding)
        {
            int len = binding.Bindings.Count;
            int index = 0;

            bindPairs = new BindPair[len];

            foreach (var pair in binding.Bindings)
            {
                bindPairs[index++] = new BindPair(pair.Key, pair.Value);
            }
        }
    }

    public class BindPair
    {
        public PlayerAction action;
        public KeyCode code;

        public BindPair(PlayerAction _action, KeyCode _code)
        {
            action = _action;
            code = _code;
        }
    }

    [Serializable]
    public class InputBinding
    {
        // 저장, 불러오기 시 폴더명, 파일명, 확장자, 고유번호
        public string localDirectoryPath = "Settings"; // "Assets/Settings"
        public string fileName = "InputBindingPreset";
        public string extName = "txt";
        public string id = "001";

        private Dictionary<PlayerAction, KeyCode> bindingDict;
        public Dictionary<PlayerAction, KeyCode> Bindings => bindingDict;

        // 생성자
        public InputBinding(bool init = true)
        {
            bindingDict = new Dictionary<PlayerAction, KeyCode>();
            if (init)
                ResetAll();
        }

        public InputBinding(SerializableInputBinding sib)
        {
            bindingDict = new Dictionary<PlayerAction, KeyCode>();
            foreach (var pair in sib.bindPairs)
            {
                bindingDict[pair.action] = pair.code;
            }
        }

        // 새 바인딩 키 할당
        public void ApplyNewBindings(InputBinding newBinding)
        {
            bindingDict = new Dictionary<PlayerAction, KeyCode>(newBinding.bindingDict);
        }

        public void ApplyNewBindings(SerializableInputBinding newBinding)
        {
            bindingDict.Clear();
            foreach (var pair in newBinding.bindPairs)
            {
                bindingDict[pair.action] = pair.code;
            }
        }

        // 바인드
        public void Bind(in PlayerAction action, in KeyCode code, bool allowOverlap = false)
        {
            if (!allowOverlap && bindingDict.ContainsValue(code))
            {
                var copyDict = new Dictionary<PlayerAction, KeyCode>(bindingDict);
                foreach (var pair in copyDict)
                {
                    if (pair.Value.Equals(code))
                        bindingDict[pair.Key] = KeyCode.None;
                }
            }
            bindingDict[action] = code;
        }

        public void ResetAll()
        {
            Bind(PlayerAction.MouseLeft, KeyCode.Mouse0);
            Bind(PlayerAction.MouseRight, KeyCode.Mouse1);

            Bind(PlayerAction.MoveForward, KeyCode.W);
            Bind(PlayerAction.MoveBackward, KeyCode.S);
            Bind(PlayerAction.MoveLeft, KeyCode.A);
            Bind(PlayerAction.MoveRight, KeyCode.D);

            Bind(PlayerAction.Crouch, KeyCode.LeftControl);
            Bind(PlayerAction.Dash, KeyCode.LeftShift);
            Bind(PlayerAction.Jump, KeyCode.Space);
        }

        public void SaveToFile()
        {
            SerializableInputBinding sib = new(this);
            JsonData infoJson = JsonMapper.ToJson(sib);

            File.WriteAllText(Application.dataPath + "/Resources/KeyInfo/KeyInputBindings.json", infoJson.ToString());

        }

        public bool LoadFromFile()
        {
            if (File.Exists(Application.dataPath + "/Resources/KeyInfo/KeyInputBindings.json"))
            {
                string jsonString = File.ReadAllText(Application.dataPath + "/Resources/KeyInfo/KeyInputBindings.json");
                Debug.Log(jsonString);

                JsonData data = JsonMapper.ToObject(jsonString);

                return true;
            }

            return false;
        }
    }

}

