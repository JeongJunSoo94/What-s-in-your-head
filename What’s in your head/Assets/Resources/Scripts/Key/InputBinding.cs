using System;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using LitJson;

namespace JCW.InputBindings
{
    // 플레이어의 행동
    public enum PlayerAction
    {
        MoveForward,
        MoveBackward,
        MoveLeft,
        MoveRight,

        MouseLeft,
        MouseRight,

        UseObject,

        Crouch,
        Dash,
        Jump,

        Pause,
    }


    // 플레이어의 행동과 키 쌍
    [Serializable]
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
    public class SerializableInputBinding
    {
        public List<BindPair> bindPairs;

        // 생성자
        public SerializableInputBinding(InputBinding binding)
        {
            bindPairs = new List<BindPair>();
            foreach (var pair in binding.Bindings)
            {
                bindPairs.Add(new BindPair(pair.Key, pair.Value));
            }
        }
        public SerializableInputBinding()
        {
            bindPairs = new List<BindPair>();
        }
    }

    

    [Serializable]
    public class InputBinding
    {
        // 저장, 불러오기 시 폴더명, 파일명, 확장자, 고유번호
        public string localDirectoryPath = "Resources/KeyInfo/KeySetting"; // "Assets/Settings"
        public string fileName = "InputBindingPreset";
        public string extName = "txt";

        private Dictionary<PlayerAction, KeyCode> bindingDict;
        public Dictionary<PlayerAction, KeyCode> Bindings => bindingDict;

        // 생성자 =======================================================
        public InputBinding(bool init = true)
        {
            bindingDict = new Dictionary<PlayerAction, KeyCode>();
            if (init)
                ResetAll();
        }
        public InputBinding(SerializableInputBinding sib)
        {
            // sib의 키쌍을 현재 bindingDict에 저장
            bindingDict = new Dictionary<PlayerAction, KeyCode>();
            foreach (var pair in sib.bindPairs)
            {
                bindingDict[pair.action] = pair.code;
            }
        }
        // =============================================================

         // 새 바인딩 키 할당
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
                    // 입력받은 키에 해당하는 값이 기존에 있으면 기존 값을 None으로
                    if (pair.Value.Equals(code))
                        bindingDict[pair.Key] = KeyCode.None;
                }
            }
            // 키 설정
            bindingDict[action] = code;
        }

        // 바인딩 초기화
        public void ResetAll()
        {
            Bind(PlayerAction.MouseLeft, KeyCode.Mouse0);
            Bind(PlayerAction.MouseRight, KeyCode.Mouse1);

            Bind(PlayerAction.MoveForward, KeyCode.W);
            Bind(PlayerAction.MoveBackward, KeyCode.S);
            Bind(PlayerAction.MoveLeft, KeyCode.A);
            Bind(PlayerAction.MoveRight, KeyCode.D);

            Bind(PlayerAction.UseObject, KeyCode.E);

            Bind(PlayerAction.Crouch, KeyCode.LeftControl);
            Bind(PlayerAction.Dash, KeyCode.LeftShift);
            Bind(PlayerAction.Jump, KeyCode.Space);
            Bind(PlayerAction.Pause, KeyCode.Escape);            
        }

        public void SaveToFile()
        {
            Debug.Log("새 키 설정 저장");
            SerializableInputBinding sib = new(this);
            JsonData infoJson = JsonMapper.ToJson(sib);

            File.WriteAllText(Application.dataPath + "/Resources/KeyInfo/KeyInputBindings.json", infoJson.ToString());

        }

        public bool LoadFromFile()
        {
            if(!File.Exists(Application.dataPath + "/Resources/KeyInfo/KeyInputBindings.json"))
            {
                Debug.Log("키 값 불러오기 실패");
                return false;
            }    

            string jsonString = File.ReadAllText(Application.dataPath + "/Resources/KeyInfo/KeyInputBindings.json");
            Debug.Log(jsonString);

            SerializableInputBinding data = JsonUtility.FromJson<SerializableInputBinding>(jsonString);
            ApplyNewBindings(data);

            return true;
        }
    }

}

