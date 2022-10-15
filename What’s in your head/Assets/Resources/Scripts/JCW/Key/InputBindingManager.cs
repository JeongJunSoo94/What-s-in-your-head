using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace JCW.UI.Options.InputBindings
{
    public class InputBindingManager : MonoBehaviour
    {
        public InputBinding _binding = new(false);

        public Button _saveButton;
        public Button _resetButton;
        public Button _backButton;
        public GameObject _thisUI;
        public GameObject _waitingInputScreen;
        public Transform _verticalLayoutTr;
        public GameObject _bindingPairPrefab;

        private List<GameObject> _bindingKeyPairs;
        private Dictionary<PlayerAction, BindingPairUI> _bindingKeyScripts;

        private bool _isListening;
        private PlayerAction _curKeyAction;

        // �̱���
        private static InputBindingManager sInstance;
        public static InputBindingManager Instance
        {
            get
            {
                if (sInstance == null)
                {
                    GameObject newGameObject = new("_InputBindingManager");
                    sInstance = newGameObject.AddComponent<InputBindingManager>();                    
                }
                return sInstance;
            }
        }

        private void Start()
        {
            Init();
            InitButtonListeners();

            LoadKeySetting();
            LoadBindingUI();
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
            _backButton.onClick.AddListener(() => 
            { 
                // �ڷΰ��� ��, ���Ͽ��� �ٽ� �ε� �� ����.
                _binding.LoadFromFile(); 
                KeyManager.Instance.KeySet(_binding); 
                RefreshAllBindingUIs(); TurnOff();
            });
            _resetButton.onClick.AddListener(() => 
            { 
                _binding.ResetAll(); 
                _binding.SaveToFile();
                RefreshAllBindingUIs();
                KeyManager.Instance.KeySet(_binding);
            });
        }

        // �̸� ������ Ű���� ���Ͽ��� �ҷ�����
        private void LoadKeySetting()
        {
            if (_binding.LoadFromFile() == false)
            {
                if (!Directory.Exists(Application.dataPath + "/Resources/Options/"))
                    Directory.CreateDirectory(Application.dataPath + "/Resources/Options/");
                _binding.ResetAll();
                _binding.SaveToFile();
                KeyManager.Instance.KeySet(_binding);
            }
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
                // Pause�� ä��Ű�� ��� �����
                if (pair.Key == PlayerAction.Pause || pair.Key == PlayerAction.Chat)
                    continue;
                var pairGo = Instantiate(_bindingPairPrefab, _verticalLayoutTr);

                // ���콺 ���� / �������� ���, Ŭ�� ���ϰ� ����.
                if(pair.Key == PlayerAction.Fire || pair.Key == PlayerAction.Aim)
                {
                    GameObject _buttonObj = pairGo.transform.GetChild(1).gameObject;
                    //_buttonObj.GetComponent<Button>().interactable = false;
                    _buttonObj.GetComponent<Image>().color = new Color(0, 0, 0, 255);
                }

                // ��ũ��Ʈ ��������
                var pairUI = pairGo.GetComponent<BindingPairUI>();
                // �ش� ��ũ��Ʈ���� �� Ű �̸� �� Ű�ڵ� ǥ��
                pairUI.InitLabels($"{pair.Key}", $"{pair.Value.keyCode}");
                pairUI.AddButtonListener(() =>
                {
                    if (pair.Key != PlayerAction.Fire && pair.Key != PlayerAction.Aim)
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
                    10 +
                    count * 60 +
                    (count - 1) * 10
                );
            }
        }
        // Ű ����
        private void SetKeyBinding(PlayerAction action, KeyCode code)
        {
            switch(action)
            {
                case PlayerAction.Fire:
                case PlayerAction.Aim:
                case PlayerAction.Chat:
                case PlayerAction.Pause:
                    break;
                default:
                    _binding.Bind(action, code);
                    break;
            }            
            RefreshAllBindingUIs();
        }

        // ȭ�鿡 ���̴� ���ε� Ű ���ΰ�ħ
        private void RefreshAllBindingUIs()
        {
            foreach (var pair in _binding.Bindings)
            {
                if (pair.Key == PlayerAction.Pause || pair.Key == PlayerAction.Chat)
                    continue;
                _bindingKeyScripts[pair.Key].SetCodeLabel($"{pair.Value.keyCode}");                
            }
        }

        // ���� �ɼ� ����
        private void TurnOff()
        {
            _thisUI.SetActive(false);
        }
    }
}