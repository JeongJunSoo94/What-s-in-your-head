using System;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using LitJson;

namespace JCW.UI.Options.InputBindings
{
    // 플레이어의 행동
    public enum PlayerAction
    {
        // 이동
        MoveForward,     MoveBackward,      MoveLeft,       MoveRight,
        Jump,            Dash,              ToggleRun,
        // 조작
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

    // 직렬화 가능한 버젼
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
                bindPairs.Add(new BindPair(pair.Key, pair.Value.keyCode));
            }
        }
        public SerializableInputBinding()
        {
            bindPairs = new List<BindPair>();
        }
    }

    // 직렬화는 못하지만 에디터에서 쓰기 좋은 딕셔너리.
    [Serializable]
    public class InputBinding
    {
        private readonly Dictionary<PlayerAction, KeyState> bindingDict;
        public Dictionary<PlayerAction, KeyState> Bindings => bindingDict;

        // 생성자 =======================================================
        public InputBinding(bool init = true)
        {            
            bindingDict = new Dictionary<PlayerAction, KeyState>();
            if (init)
                ResetAll();
        }
        public InputBinding(SerializableInputBinding sib)
        {
            // sib의 키쌍을 현재 bindingDict에 저장
            bindingDict = new Dictionary<PlayerAction, KeyState>();

            foreach (var pair in sib.bindPairs)
            {
                bindingDict[pair.action].keyCode = pair.code;
            }
        }
        // =============================================================

         // 저장된 파일에서 가져온 키 할당
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
        
        // 바인드
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

        // 바인딩 초기화
        public void ResetAll()
        {
            // 이동
            Bind(PlayerAction.MoveForward, KeyCode.W);
            Bind(PlayerAction.MoveBackward, KeyCode.S);
            Bind(PlayerAction.MoveLeft, KeyCode.A);
            Bind(PlayerAction.MoveRight, KeyCode.D);

            // 점프 & 달리기
            Bind(PlayerAction.Jump, KeyCode.Space);
            Bind(PlayerAction.Dash, KeyCode.LeftShift);
            Bind(PlayerAction.ToggleRun, KeyCode.CapsLock);

            // 상호작용
            Bind(PlayerAction.Interaction, KeyCode.E);
            Bind(PlayerAction.Swap, KeyCode.Tab);

            // 마우스
            Bind(PlayerAction.Fire, KeyCode.Mouse0);
            Bind(PlayerAction.Aim, KeyCode.Mouse1);

            // 히든
            Bind(PlayerAction.Pause, KeyCode.Escape);    
            
            Bind(PlayerAction.Chat, KeyCode.Quote);

        }

        public void SaveToFile()
        {
            Debug.Log("새 키 설정 저장");
            SerializableInputBinding sib = new(this);
            JsonData infoJson = JsonMapper.ToJson(sib);

            File.WriteAllText(Application.dataPath + "/Resources/Options/KeyInputBindings.json", infoJson.ToString());

        }

        public bool LoadFromFile()
        {
            if(!File.Exists(Application.dataPath + "/Resources/Options/KeyInputBindings.json"))
            {
                Debug.Log("키 값 불러오기 실패");
                return false;
            }    

            string jsonString = File.ReadAllText(Application.dataPath + "/Resources/Options/KeyInputBindings.json");
            Debug.Log("키 값 불러오기 성공");

            SerializableInputBinding data = JsonUtility.FromJson<SerializableInputBinding>(jsonString);
            ApplyNewBindings(data);

            return true;
        }
    }

}

