using System;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using LitJson;

namespace JCW.UI.Options.InputBindings
{
    // �÷��̾��� �ൿ
    public enum PlayerAction
    {
        // �̵�
        MoveForward,     MoveBackward,      MoveLeft,       MoveRight,
        Jump,            Dash,              ToggleRun,
        // ����
        Interaction,       Swap,            Fire,            Aim,
        Pause,             Chat,

        END
    }

    public class KeyState
    {
        public KeyState(KeyCode keyCode)
        {
            this.keyCode = keyCode;
        }
        public KeyCode keyCode;
        public bool keyOn = false;
        public bool keyDown = false;
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

    // ����ȭ ������ ����
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
                bindPairs.Add(new BindPair(pair.Key, pair.Value.keyCode));
            }
        }
        public SerializableInputBinding()
        {
            bindPairs = new List<BindPair>();
        }
    }

    // ����ȭ�� �������� �����Ϳ��� ���� ���� ��ųʸ�.
    [Serializable]
    public class InputBinding
    {
        private readonly Dictionary<PlayerAction, KeyState> bindingDict;
        public Dictionary<PlayerAction, KeyState> Bindings => bindingDict;

        // ������ =======================================================
        public InputBinding(bool init = true)
        {            
            bindingDict = new Dictionary<PlayerAction, KeyState>();
            if (init)
                ResetAll();
        }
        public InputBinding(SerializableInputBinding sib)
        {
            // sib�� Ű���� ���� bindingDict�� ����
            bindingDict = new Dictionary<PlayerAction, KeyState>();

            foreach (var pair in sib.bindPairs)
            {
                bindingDict[pair.action].keyCode = pair.code;
            }
        }
        // =============================================================

         // ����� ���Ͽ��� ������ Ű �Ҵ�
        public void ApplyNewBindings(SerializableInputBinding newBinding)
        {
            bindingDict.Clear();
            int i = 0;
            foreach (var pair in newBinding.bindPairs)
            {
                if(bindingDict.Count <=(int) PlayerAction.END)
                    bindingDict.Add((PlayerAction)i++, new KeyState(pair.code));
                else
                    bindingDict[pair.action].keyCode = pair.code;
            }
        }
        
        // ���ε�
        public void Bind(in PlayerAction action, in KeyCode code)
        {
            if(bindingDict.Count < (int)PlayerAction.END)
            {
                bindingDict.Add(action, new KeyState(code));
            }
            else
            {
                switch(code)
                {
                    case KeyCode.Mouse0:
                    case KeyCode.Mouse1:
                    case KeyCode.Escape:
                    case KeyCode.Colon:
                    case KeyCode.Tab:
                        break;
                    default:
                        for (int i = 0 ; i < bindingDict.Count ; ++i)
                        {
                            if (bindingDict[(PlayerAction)i].keyCode == code)
                            {
                                bindingDict[(PlayerAction)i].keyCode = KeyCode.None;
                            }
                        }
                        bindingDict[action].keyCode = code;
                        break;
                }                
            }
            
        }

        // ���ε� �ʱ�ȭ
        public void ResetAll()
        {
            // �̵�
            Bind(PlayerAction.MoveForward, KeyCode.W);
            Bind(PlayerAction.MoveBackward, KeyCode.S);
            Bind(PlayerAction.MoveLeft, KeyCode.A);
            Bind(PlayerAction.MoveRight, KeyCode.D);

            // ���� & �޸���
            Bind(PlayerAction.Jump, KeyCode.Space);
            Bind(PlayerAction.Dash, KeyCode.LeftShift);
            Bind(PlayerAction.ToggleRun, KeyCode.CapsLock);

            // ��ȣ�ۿ�
            Bind(PlayerAction.Interaction, KeyCode.E);
            Bind(PlayerAction.Swap, KeyCode.Tab);

            // ���콺
            Bind(PlayerAction.Fire, KeyCode.Mouse0);
            Bind(PlayerAction.Aim, KeyCode.Mouse1);

            // ����
            Bind(PlayerAction.Pause, KeyCode.Escape);    
            
            Bind(PlayerAction.Chat, KeyCode.Quote);

        }

        public void SaveToFile()
        {
            Debug.Log("�� Ű ���� ����");
            SerializableInputBinding sib = new(this);
            JsonData infoJson = JsonMapper.ToJson(sib);

            File.WriteAllText(Application.dataPath + "/Resources/Options/KeyInputBindings.json", infoJson.ToString());

        }

        public bool LoadFromFile()
        {
            if(!File.Exists(Application.dataPath + "/Resources/Options/KeyInputBindings.json"))
            {
                Debug.Log("Ű �� �ҷ����� ����");
                return false;
            }    

            string jsonString = File.ReadAllText(Application.dataPath + "/Resources/Options/KeyInputBindings.json");
            Debug.Log("Ű �� �ҷ����� ����");

            SerializableInputBinding data = JsonUtility.FromJson<SerializableInputBinding>(jsonString);
            ApplyNewBindings(data);

            return true;
        }
    }

}

