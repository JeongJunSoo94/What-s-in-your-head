using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;
using System.IO;
using LitJson;
using JCW.AudioCtrl;
using YC.CameraManager_;

namespace JCW.UI.Options
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
        [Header("게임 탭")][SerializeField] private GameObject gameTab;
        [Header("카메라 탭")][SerializeField] private GameObject camTab;
        [Header("사운드 탭")][SerializeField] private GameObject soundTab;

        private readonly Dictionary<GameObject, string> gameContents = new();
        private readonly Dictionary<GameObject, string> camContents = new();
        private readonly Dictionary<GameObject, string> soundContents = new();

        SerializableDatas setValue = new();

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
        }

        private void OnEnable()
        {
            LoadFromFile();
            setValue.data.Clear();
        }

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                SaveToFile();
            });
        }

        void SetData(Dictionary<GameObject, string> _contents, string type)
        {
            int index = 0;
            foreach (GameObject tabObj in _contents.Keys)
            {
                GameObject funcObj = tabObj.transform.GetChild(2).gameObject;
                if (funcObj.name == "Function")
                {
                    string value = funcObj.GetComponent<Text>().text == "끄기" ? "0" : "1";
                    if(type == "Camera" && index == 0)
                        CameraManager.Instance.Option_SetShake(value == "1");
                    setValue.data.Add(new OptionData(_contents[tabObj], value, false));
                }

                else // slider
                {
                    switch(type)
                    {
                        case "Game":
                            break;

                        case "Camera":
                            {
                                switch (index)
                                {
                                    case 1:
                                        // 카메라 기본 감도 여부 넣어주기
                                        CameraManager.Instance.Option_SetSensitivity(funcObj.GetComponent<Slider>().value, CameraManager.Instance.sholderSensitivitySaved);
                                        break;
                                    case 2:
                                        // 카메라 조준 감도 여부 넣어주기
                                        CameraManager.Instance.Option_SetSensitivity(CameraManager.Instance.backSensitivitySaved, funcObj.GetComponent<Slider>().value);
                                        break;
                                }
                            }
                            break;

                        case "Sound":
                            {
                                switch (index)
                                {
                                    case 0:
                                        SoundManager.Instance.audioSources[(int)Sound.TOTAL].volume = funcObj.GetComponent<Slider>().value * 0.01f;
                                        break;
                                    case 1:
                                        SoundManager.Instance.audioSources[(int)Sound.BGM].volume = SoundManager.Instance.audioSources[(int)Sound.TOTAL].volume * (funcObj.GetComponent<Slider>().value * 0.01f);
                                        break;
                                    case 2:
                                        float volume = SoundManager.Instance.audioSources[(int)Sound.TOTAL].volume * (funcObj.GetComponent<Slider>().value * 0.01f);
                                        SoundManager.Instance.audioSources[(int)Sound.EFFECT].volume = volume;
                                        SoundManager.Instance.audioSources[(int)Sound.DISTANCE].volume = volume;
                                        SoundManager.Instance.audioSources[(int)Sound.UI].volume = volume;
                                        break;
                                }
                            }
                            break;
                    }
                    setValue.data.Add(new OptionData(_contents[tabObj], (funcObj.GetComponent<Slider>().value).ToString(), true));
                }
                ++index;
            }
        }

        public void SaveToFile(bool init = false)
        {
            setValue.data.Clear();
            SetData(gameContents, "Game");
            SetData(camContents, "Camera");
            SetData(soundContents, "Sound");

            JsonData infoJson = JsonMapper.ToJson(setValue);

            // 폴더 없으면 만들기
            if (!Directory.Exists(Application.dataPath + "/Resources/Options/"))
                Directory.CreateDirectory(Application.dataPath + "/Resources/Options/");
                        
            if(init)
                File.WriteAllText(Application.dataPath + "/Resources/Options/OptionValuesInit.json", infoJson.ToString());
            File.WriteAllText(Application.dataPath + "/Resources/Options/OptionValues.json", infoJson.ToString());

            // 저장된 자료들 초기화
            setValue.data.Clear();
        }

        public void LoadFromFile(bool init = false)
        {
            string file = init ? "/Resources/Options/OptionValuesInit.json" : "/Resources/Options/OptionValues.json";
            if (!File.Exists(Application.dataPath + file))
            {
                SaveToFile(true);
                return;
            }

            string jsonString = File.ReadAllText(Application.dataPath + file);
            

            setValue = new SerializableDatas();
            setValue = JsonUtility.FromJson<SerializableDatas>(jsonString);
            GetData(setValue);
            if (init)
                SaveToFile();

        }

        void GetData(SerializableDatas _value)
        {
            int index = GetFromContents(_value, gameContents, gameTab, 0, "Game");
            index = GetFromContents(_value, camContents, camTab, index, "Camera");
            GetFromContents(_value, soundContents, soundTab, index, "Sound");
        }

        int GetFromContents(SerializableDatas _value, Dictionary<GameObject, string> contents, GameObject _tab, int index = 0, string type = "Sound")
        {
            int typeContentsIndex = 0;
            foreach (GameObject Setting in contents.Keys)
            {
                if (_value.data[index].tabName == _tab.transform.parent.gameObject.name)
                {
                    GameObject funcObj = Setting.transform.GetChild(2).gameObject;
                    if (_value.data[index].isSlider == false)
                    {
                        bool isOn = int.Parse(_value.data[index].value) == 1;
                        switch (type)
                        {
                            case "Game":
                                break;
                            case "Camera":
                                if (type == "Camera" && index == 0)
                                    CameraManager.Instance.Option_SetShake(isOn);
                                break;
                        }
                        funcObj.GetComponent<Text>().text = isOn ? "켜기" : "끄기";
                    }
                    else if (_value.data[index].isSlider == true)
                    {
                        float value = float.Parse(_value.data[index].value);
                        switch (type)
                        {
                            case "Sound":
                                {
                                    switch(typeContentsIndex)
                                    {
                                        case 0:
                                            SoundManager.Instance.audioSources[(int)Sound.TOTAL].volume = value * 0.01f;
                                            break;
                                        case 1:
                                            SoundManager.Instance.audioSources[(int)Sound.BGM].volume = SoundManager.Instance.audioSources[(int)Sound.TOTAL].volume * (value * 0.01f);
                                            break;
                                        case 2:
                                            SoundManager.Instance.audioSources[(int)Sound.EFFECT].volume = SoundManager.Instance.audioSources[(int)Sound.TOTAL].volume * (value * 0.01f);
                                            SoundManager.Instance.audioSources[(int)Sound.UI].volume = SoundManager.Instance.audioSources[(int)Sound.TOTAL].volume * (value * 0.01f);
                                            SoundManager.Instance.audioSources[(int)Sound.DISTANCE].volume = SoundManager.Instance.audioSources[(int)Sound.TOTAL].volume * (value * 0.01f);
                                            break;
                                    }
                                }
                                break;
                            case "Camera":
                                {
                                    switch (typeContentsIndex)
                                    {
                                        case 1:
                                            // 카메라 기본 감도 여부 넣어주기
                                            CameraManager.Instance.Option_SetSensitivity(value, CameraManager.Instance.sholderSensitivitySaved);
                                            break;
                                        case 2:
                                            // 카메라 조준 감도 여부 넣어주기
                                            CameraManager.Instance.Option_SetSensitivity(CameraManager.Instance.backSensitivitySaved, value);
                                            break;
                                    }
                                }
                                break;
                        }
                        funcObj.GetComponent<Slider>().value = value;
                    }
                    else
                        Debug.Log("에러에러에러");

                    ++index;
                    ++typeContentsIndex;
                }
                else
                    break;                
            }
            return index;
        }



    }
}

