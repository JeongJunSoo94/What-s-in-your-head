using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;
using System.IO;
using LitJson;
using JCW.AudioCtrl;

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
        [Header("���� ��")][SerializeField] private GameObject gameTab;
        [Header("ī�޶� ��")][SerializeField] private GameObject camTab;
        [Header("���� ��")][SerializeField] private GameObject soundTab;

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
                    string value = funcObj.GetComponent<Text>().text == "����" ? "0" : "1";
                    //if(type == "Camera" && index == 0)
                        //CameraManager.Instance.isShake = value == "0" ? false : true;
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
                                    case 0:
                                        // ī�޶� ��鸲 ���� �־��ֱ�
                                        // CameraManager.Instance.isShake = 
                                        break;
                                    case 1:
                                        // ī�޶� �⺻ ���� ���� �־��ֱ�
                                        // CameraManager.Instance. = 
                                        break;
                                    case 2:
                                        // ī�޶� ���� ���� ���� �־��ֱ�
                                        // CameraManager.Instance. = 
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

        private void SaveToFile(bool init = false)
        {
            setValue.data.Clear();
            SetData(gameContents, "Game");
            SetData(camContents, "Camera");
            SetData(soundContents, "Sound");

            Debug.Log("�ɼ� ���ð� ����");

            JsonData infoJson = JsonMapper.ToJson(setValue);

            // ���� ������ �����
            if (!Directory.Exists(Application.dataPath + "/Resources/Options/"))
                Directory.CreateDirectory(Application.dataPath + "/Resources/Options/");
                        
            if(init)
                File.WriteAllText(Application.dataPath + "/Resources/Options/OptionValuesInit.json", infoJson.ToString());
            File.WriteAllText(Application.dataPath + "/Resources/Options/OptionValues.json", infoJson.ToString());

            // ����� �ڷ�� �ʱ�ȭ
            setValue.data.Clear();
        }

        private void LoadFromFile(bool init = false)
        {
            string file = init ? "/Resources/Options/OptionValuesInit.json" : "/Resources/Options/OptionValues.json";
            if (!File.Exists(Application.dataPath + file))
            {
                Debug.Log("�ɼ� ���ð� �ҷ����� ����. �ʱ� ������ ����");
                SaveToFile(true);
                return;
            }
            else
                Debug.Log("�ɼ� ���ð� �ҷ����� ����");

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
                        bool isOff = int.Parse(_value.data[index].value) == 0;
                        switch (type)
                        {
                            case "Game":
                                break;
                            case "Camera":
                                //if(typeContentsIndex == 0)
                                    //CameraManager.Instance.isShake = isOff;
                                break;
                        }
                        funcObj.GetComponent<Text>().text = isOff ? "����" : "�ѱ�";
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
                                            // ī�޶� �⺻ ���� ���� �־��ֱ�
                                            // CameraManager.Instance. = 
                                            break;
                                        case 2:
                                            // ī�޶� ���� ���� ���� �־��ֱ�
                                            // CameraManager.Instance. = 
                                            break;
                                    }
                                }
                                break;
                        }
                        funcObj.GetComponent<Slider>().value = value;
                    }
                    else
                        Debug.Log("������������");

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

