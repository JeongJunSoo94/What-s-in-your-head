using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;
using System.IO;
using LitJson;

namespace JCW.Options
{
    [Serializable]
    public class OptionData
    {
        public string tabName;
        public string value;
        public bool isSlider;

        public OptionData(string _name, string _val, bool _isSlider)
        {
            tabName = _name;
            value = _val;
            isSlider = _isSlider;
        }
    }

    [Serializable]
    public class SerializableDatas
    {
        public List<OptionData> data;
        public SerializableDatas()
        {
            data = new List<OptionData>();
        }
    }

    public class ApplySettings : MonoBehaviour
    {
        [SerializeField] private GameObject gameTab;
        [SerializeField] private GameObject camTab;
        [SerializeField] private GameObject soundTab;

        //private List<GameObject> gameContents = new List<GameObject>();
        //private List<GameObject> camContents = new List<GameObject>();
        //private List<GameObject> soundContents = new List<GameObject>();

        private Dictionary<GameObject, string> gameContents = new Dictionary<GameObject, string>();
        private Dictionary<GameObject, string> camContents = new Dictionary<GameObject, string>();
        private Dictionary<GameObject, string> soundContents = new Dictionary<GameObject, string>();

        SerializableDatas setValue = new SerializableDatas();

        private void Awake()
        {
            for (int i = 0 ; i < gameTab.transform.childCount ; ++i)
            { 
                gameContents.Add(gameTab.transform.GetChild(i).gameObject, "Game");
            }
            for (int i = 0 ; i < camTab.transform.childCount ; ++i)
            {
                camContents.Add(camTab.transform.GetChild(i).gameObject, "Camera");
            }
            for (int i = 0 ; i < soundTab.transform.childCount ; ++i)
            {
                soundContents.Add(soundTab.transform.GetChild(i).gameObject, "Sound");
            }

            //LoadFromFile();
            //setValue.data.Clear();
        }

        private void OnEnable()
        {
            Debug.Log("켜졌습니당");
            LoadFromFile();
            setValue.data.Clear();
        }

        void Start()
        {
            this.gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                SaveToFile();
            });
        }

        void SetData(Dictionary<GameObject, string> _contents)
        {
            foreach (GameObject tabObj in _contents.Keys)
            {
                GameObject funcObj = tabObj.transform.GetChild(2).gameObject;
                if (funcObj.name == "Function")
                {
                    string value = funcObj.GetComponent<Text>().text == "끄기" ? "0" : "1";
                    setValue.data.Add(new OptionData(_contents[tabObj], value, false));
                }

                else // slider
                    setValue.data.Add(new OptionData(_contents[tabObj], (funcObj.GetComponent<Slider>().value).ToString(), true));
            }
        }

        private void SaveToFile(bool init = false)
        {
            SetData(gameContents);
            SetData(camContents);
            SetData(soundContents);

            Debug.Log("옵션 세팅값 저장");

            JsonData infoJson = JsonMapper.ToJson(setValue);
            if (!Directory.Exists(Application.dataPath + "/Resources/Options/"))
                Directory.CreateDirectory(Application.dataPath + "/Resources/Options/");
                        
            if(init)
                File.WriteAllText(Application.dataPath + "/Resources/Options/OptionValuesInit.json", infoJson.ToString());
            File.WriteAllText(Application.dataPath + "/Resources/Options/OptionValues.json", infoJson.ToString());

            // 저장된 자료들 초기화
            setValue.data.Clear();
        }

        private void LoadFromFile()
        {

            if (!File.Exists(Application.dataPath + "/Resources/Options/OptionValues.json"))
            {
                Debug.Log("옵션 세팅값 불러오기 실패. 초기 데이터 저장");
                SaveToFile(true);
                return;
            }
            else
                Debug.Log("옵션 세팅값 불러오기 성공");

            string jsonString = File.ReadAllText(Application.dataPath + "/Resources/Options/OptionValues.json");
            

            setValue = new SerializableDatas();
            setValue = JsonUtility.FromJson<SerializableDatas>(jsonString);
            GetData(setValue);

        }

        void GetData(SerializableDatas _value)
        {
            int index = GetFromContents(_value, gameContents, gameTab);
            index = GetFromContents(_value, camContents, camTab, index);
            GetFromContents(_value, soundContents, soundTab, index);
        }

        int GetFromContents(SerializableDatas _value, Dictionary<GameObject, string> contents, GameObject _tab, int index = 0)
        {
            foreach (GameObject Setting in contents.Keys)
            {
                if (_value.data[index].tabName == _tab.transform.parent.gameObject.name)
                {
                    GameObject funcObj = Setting.transform.GetChild(2).gameObject;
                    if (funcObj.name == "Function" && _value.data[index].isSlider == false)
                    {
                        funcObj.GetComponent<Text>().text = int.Parse(_value.data[index].value) == 0 ? "끄기" : "켜기";
                    }
                    else if (funcObj.name == "Slider" && _value.data[index].isSlider == true)
                    {
                        Debug.Log(_value.data[index].value);
                        funcObj.GetComponent<Slider>().value = float.Parse(_value.data[index].value);
                    }
                    else
                        Debug.Log("에러에러에러");

                    ++index;
                }
                else
                    break;
            }
            return index;
        }



    }
}

