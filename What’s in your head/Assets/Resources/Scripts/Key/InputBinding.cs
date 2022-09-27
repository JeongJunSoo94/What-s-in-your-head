using System;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using LitJson;

namespace JCW.InputBindings
{
    // �÷��̾��� �ൿ
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


    // �÷��̾��� �ൿ�� Ű ��
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

        // ������
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
        // ����, �ҷ����� �� ������, ���ϸ�, Ȯ����, ������ȣ
        public string localDirectoryPath = "Resources/KeyInfo/KeySetting"; // "Assets/Settings"
        public string fileName = "InputBindingPreset";
        public string extName = "txt";

        private Dictionary<PlayerAction, KeyCode> bindingDict;
        public Dictionary<PlayerAction, KeyCode> Bindings => bindingDict;

        // ������ =======================================================
        public InputBinding(bool init = true)
        {
            bindingDict = new Dictionary<PlayerAction, KeyCode>();
            if (init)
                ResetAll();
        }
        public InputBinding(SerializableInputBinding sib)
        {
            // sib�� Ű���� ���� bindingDict�� ����
            bindingDict = new Dictionary<PlayerAction, KeyCode>();
            foreach (var pair in sib.bindPairs)
            {
                bindingDict[pair.action] = pair.code;
            }
        }
        // =============================================================

         // �� ���ε� Ű �Ҵ�
        public void ApplyNewBindings(SerializableInputBinding newBinding)
        {
            bindingDict.Clear();
            foreach (var pair in newBinding.bindPairs)
            {
                bindingDict[pair.action] = pair.code;
            }
        }
        
        // ���ε�
        public void Bind(in PlayerAction action, in KeyCode code, bool allowOverlap = false)
        {
            if (!allowOverlap && bindingDict.ContainsValue(code))
            {
                var copyDict = new Dictionary<PlayerAction, KeyCode>(bindingDict);
                foreach (var pair in copyDict)
                {
                    // �Է¹��� Ű�� �ش��ϴ� ���� ������ ������ ���� ���� None����
                    if (pair.Value.Equals(code))
                        bindingDict[pair.Key] = KeyCode.None;
                }
            }
            // Ű ����
            bindingDict[action] = code;
        }

        // ���ε� �ʱ�ȭ
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
            Debug.Log("�� Ű ���� ����");
            SerializableInputBinding sib = new(this);
            JsonData infoJson = JsonMapper.ToJson(sib);

            File.WriteAllText(Application.dataPath + "/Resources/KeyInfo/KeyInputBindings.json", infoJson.ToString());

        }

        public bool LoadFromFile()
        {
            if(!File.Exists(Application.dataPath + "/Resources/KeyInfo/KeyInputBindings.json"))
            {
                Debug.Log("Ű �� �ҷ����� ����");
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

