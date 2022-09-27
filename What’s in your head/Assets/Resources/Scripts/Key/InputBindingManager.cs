using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace JCW.InputBindings
{
    public class InputBindingManager : MonoBehaviour
    {
        private enum InputState
        {
            None,
            Mouse,
            Keyboard
        }

        public InputBinding _binding = new(false)
        {
            localDirectoryPath = @"Resources/KeyInfo/KeySetting",
            fileName = "InputBindingPreset",
            extName = "txt",
        };

        public Button _saveButton;
        public GameObject _waitingInputScreen;
        public Transform _verticalLayoutTr;
        public GameObject _bindingPairPrefab;

        private List<GameObject> _bindingKeyPairs;
        private Dictionary<PlayerAction, BindingPairUI> _bindingKeyScripts;

        private bool _isListening;
        private PlayerAction _curKeyAction;

        private void Start()
        {
            Init();
            InitButtonListeners();

            LoadKeySetting();
            LoadBindingUI();
        }

        private void Update()
        {
            if (_isListening)
            {
                if (ListenInput(out var keyCode))
                {
                    SetKeyBinding(_curKeyAction, keyCode);
                    _isListening = false;
                }
            }

            _waitingInputScreen.SetActive(_isListening);
        }


        private bool ListenInput(out KeyCode code)
        {
            IEnumerable<KeyCode> keyCodeList =
                Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>();

            foreach (var curCode in keyCodeList)
            {
                if (Input.GetKeyDown(curCode))
                {
                    code = curCode;
                    return true;
                }
            }

            code = KeyCode.None;
            return false;
        }


        private void Init()
        {
            _isListening = false;
            _waitingInputScreen.SetActive(false);

            _bindingKeyPairs = new List<GameObject>();
            _bindingKeyScripts = new Dictionary<PlayerAction, BindingPairUI>();
        }

        private void InitButtonListeners()
        {
            _saveButton.onClick.AddListener(() => {_binding.SaveToFile();});
        }

        private void LoadKeySetting()
        {
            if (_binding.LoadFromFile() == false)
            {
                _binding.ResetAll();
                _binding.SaveToFile();
            }
        }

        private void LoadBindingUI()
        {
            int count = 0;

            // Ȥ�� �ȿ� �����Ͱ� ���� ������ �� ����� �ʱ�ȭ
            foreach (var go in _bindingKeyPairs)
            {
                Destroy(go);
            }
            _bindingKeyScripts.Clear();
            _bindingKeyPairs.Clear();


            foreach (var pair in _binding.Bindings)
            {
                var pairGo = Instantiate(_bindingPairPrefab, _verticalLayoutTr);

                // ��ũ��Ʈ ��������
                var pairUI = pairGo.GetComponent<BindingPairUI>();
                // �ش� ��ũ��Ʈ���� �� Ű �̸� �� Ű�ڵ� ǥ��
                pairUI.InitLabels($"{pair.Key}", $"{pair.Value}");
                pairUI.AddButtonListener(() =>
                {
                    pairUI.Select();
                    _isListening = true;
                    _curKeyAction = pair.Key;
                });

                _bindingKeyScripts.Add(pair.Key, pairUI);
                _bindingKeyPairs.Add(pairGo);
                count++;
            }

            // Ű �ɼ� ������ ����
            _verticalLayoutTr.TryGetComponent(out RectTransform rt);
            if (rt)
            {
                rt.sizeDelta = new Vector2(rt.sizeDelta.x,
                    40 +
                    count * 60 +
                    (count - 1) * 10
                );
            }
        }

        private void SetKeyBinding(PlayerAction action, KeyCode code)
        {
            _binding.Bind(action, code);
            RefreshAllBindingUIs();
        }

        private void RefreshAllBindingUIs()
        {
            foreach (var pair in _binding.Bindings)
            {
                _bindingKeyScripts[pair.Key].SetCodeLabel($"{pair.Value}");
                _bindingKeyScripts[pair.Key].Deselect();
            }
        }
    }
}