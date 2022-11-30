using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using UnityEngine.UI;
using JCW.AudioCtrl;

namespace JCW.Dialog
{
    public class csvReader
    {
        static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
        static char[] TRIM_CHARS = { '\"' };

        static StringBuilder filePath = new(240, 240);

        public static List<Dictionary<string, object>> Read(string file)
        {
            filePath.Clear();
            filePath.Append(Application.streamingAssetsPath);            
            filePath.Append("/Dialogs/");            
            filePath.Append(file);
            filePath.Append(".csv");
            TextAsset data = new(System.IO.File.ReadAllText(filePath.ToString()));
            var list = new List<Dictionary<string, object>>();
            var lines = Regex.Split(data.text, LINE_SPLIT_RE);

            if (lines.Length <= 1) return list;

            var header = Regex.Split(lines[0], SPLIT_RE);
            for (var i = 1 ; i < lines.Length ; i++)
            {

                var values = Regex.Split(lines[i], SPLIT_RE);
                if (values.Length == 0 || values[0] == "") continue;

                var entry = new Dictionary<string, object>();
                for (var j = 0 ; j < header.Length && j < values.Length ; j++)
                {
                    string value = values[j];
                    value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                    object finalvalue = value;
                    if (int.TryParse(value, out int n))
                    {
                        finalvalue = n;
                    }
                    else if (float.TryParse(value, out float f))
                    {
                        finalvalue = f;
                    }
                    entry[header[j]] = finalvalue;
                }
                list.Add(entry);
            }
            return list;
        }
    }

    enum DialogType { NELLA, STEADY, ETC };

    public class DialogManager : MonoBehaviour
    {
        [HideInInspector] public static DialogManager Instance = null;
        //해당 스테이지, 해당 섹션의 대사 전부
        List<string> nellaDialogs = new();      List<string> nellaVoices = new();  
        List<string> steadyDialogs = new();     List<string> steadyVoices = new();
        List<string> etcDialogs = new();        List<string> etcVoices = new();

        // 파싱해서 가져온 데이터들. 위의 리스트들에 넣어줄 용도.
        List<Dictionary<string, object>> parsingString = null; 

        // 해당 파일이 존재하는 지 여부를 체크하기 위한 변수
        StringBuilder existCheck = null;

        // UI를 통해 한 줄 씩 띄워줄 문자열
        StringBuilder etcDialog = null;   
        StringBuilder nellaDialog = null; 
        StringBuilder steadyDialog = null;

        public Text etcText1;           public Text etcText2;   
        public Text nellaText1;         public Text nellaText2; 
        public Text steadyText1;        public Text steadyText2;

        [HideInInspector] public Text etcRealText1;       [HideInInspector] public Text etcRealText2;
        [HideInInspector] public Text nellaRealText1;     [HideInInspector] public Text nellaRealText2;
        [HideInInspector] public Text steadyRealText1;      [HideInInspector] public Text steadyRealText2;


        [HideInInspector] public bool isEtcStart = false;
        [HideInInspector] public bool isNellaStart = false;
        [HideInInspector] public bool isSteadyStart = false;
        [HideInInspector] public bool needToEtcBreak = false;
        [HideInInspector] public bool needToNellaBreak = false;
        [HideInInspector] public bool needToSteadyBreak = false;

        [HideInInspector] public bool isSubOn = true;
        [HideInInspector] public bool isSubBGOn = true;

        List<Image> subBackGround = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
                etcRealText1    = etcText1.transform.GetChild(1).GetComponent<Text>();
                etcRealText2    = etcText2.transform.GetChild(1).GetComponent<Text>();
                nellaRealText1  = nellaText1.transform.GetChild(1).GetComponent<Text>();
                nellaRealText2  = nellaText2.transform.GetChild(1).GetComponent<Text>();
                steadyRealText1 = steadyText1.transform.GetChild(1).GetComponent<Text>();
                steadyRealText2 = steadyText2.transform.GetChild(1).GetComponent<Text>();

                subBackGround.Add(etcText1.transform.GetChild(0).GetComponent<Image>());
                subBackGround.Add(nellaText1.transform.GetChild(0).GetComponent<Image>());
                subBackGround.Add(steadyText1.transform.GetChild(0).GetComponent<Image>());
                subBackGround.Add(etcText2.transform.GetChild(0).GetComponent<Image>());
                subBackGround.Add(nellaText2.transform.GetChild(0).GetComponent<Image>());
                subBackGround.Add(steadyText2.transform.GetChild(0).GetComponent<Image>());

            }
            else
                Destroy(this.gameObject);
        }
        // 스테이지 시작 시 불러주면 될듯.
        public void SetDialogs(string fileName)
        {
            if (existCheck == null)
                existCheck = new(240,240);
            else
                existCheck.Clear();

            existCheck.Append(Application.streamingAssetsPath + "/Dialogs/");
            existCheck.Append(fileName);
            existCheck.Append(".csv");

            if (!File.Exists(existCheck.ToString()))
            {
                //Debug.Log("불러올 " + existCheck + " 파일이 없습니다");
                return;
            }

            parsingString = csvReader.Read(fileName);
            if (fileName.Contains("_N"))
            {
                for (int i=0 ; i<parsingString.Count ; ++i)
                {
                    nellaDialogs.Add(parsingString[i]["Script"].ToString());
                    nellaVoices.Add(parsingString[i]["Voice"].ToString());
                }
            }
            else if (fileName.Contains("_S"))
            {
                for (int i = 0 ; i < parsingString.Count ; ++i)
                {
                    steadyDialogs.Add(parsingString[i]["Script"].ToString());
                    steadyVoices.Add(parsingString[i]["Voice"].ToString());
                }
            }
            else if (fileName.Contains("_E"))
            {
                for (int i = 0 ; i < parsingString.Count ; ++i)
                {
                    etcDialogs.Add(parsingString[i]["Script"].ToString());
                    etcVoices.Add(parsingString[i]["Voice"].ToString());
                }
            }
            parsingString.Clear();
            parsingString = null;
        }

        // 다른 곳에서 대사 스크립트를 읽을 때 사용, 현재 몇번째의 라인을 사용하면 되는지..
        public string ReadDialogs(int type, int order)
        {
            if (order == 0)
                return null;
            switch (type)
            {
                case (int)DialogType.NELLA :
                    if(!nellaVoices[order-1].Equals("--"))
                        SoundManager.Instance.PlayEffect(nellaVoices[order-1]);
                    return nellaDialogs[order - 1];
                case (int)DialogType.STEADY :
                    if (!steadyVoices[order-1].Equals("--"))
                        SoundManager.Instance.PlayEffect(steadyVoices[order-1]);
                    return steadyDialogs[order - 1];
                case (int)DialogType.ETC :
                    if (!etcVoices[order-1].Equals("--"))
                        SoundManager.Instance.PlayEffect(etcVoices[order-1]);
                    return etcDialogs[order - 1];
                default:
                    break;
            }

            return null;
        }
        // 스테이지 종료 시 불러주면 될듯.
        public void ResetDialogs()
        {
            nellaDialogs.Clear();
            steadyDialogs.Clear();
            etcDialogs.Clear();
            nellaVoices.Clear();
            steadyVoices.Clear();
            etcVoices.Clear();

            etcText1.gameObject.SetActive(false);
            etcText2.gameObject.SetActive(false);
            nellaText1.gameObject.SetActive(false);
            nellaText2.gameObject.SetActive(false);
            steadyText1.gameObject.SetActive(false);
            steadyText2.gameObject.SetActive(false);

            if (parsingString != null && parsingString.Count > 0)
            {
                parsingString.Clear();
                parsingString = null;
            }

            return;
        }


        public bool isSet()
        {
            return steadyDialogs.Count != 0;
        }

        void SetSubtitleBG(bool isSubBGOn)
        {
            for (int i = 0 ; i < subBackGround.Count ; ++i)
            {
                subBackGround[i].enabled = isSubBGOn;
            }
        }

        public void SetEtcDialog(int order)
        {
            if (!isSubOn)
                return;
            SetSubtitleBG(isSubBGOn);
            if (etcDialog == null)
            {
                etcDialog = new(ReadDialogs((int)DialogType.ETC, order), 120);
                etcDialog.Insert(0, "- ");
            }
            else
            {
                etcDialog.Clear();
                etcDialog.Append("- ");
                etcDialog.Append(ReadDialogs((int)DialogType.ETC, order));
            }
            etcText1.text = etcDialog.ToString();
            etcText2.text = etcText1.text;
            etcRealText1.text = etcText1.text;
            etcRealText2.text = etcText1.text;
            etcText1.gameObject.SetActive(true);
            etcText2.gameObject.SetActive(true);
        }

        public void SetNellaDialog(int order)
        {
            if (!isSubOn)
                return;
            SetSubtitleBG(isSubBGOn);
            if (nellaDialog == null)
            {
                nellaDialog = new(ReadDialogs((int)DialogType.NELLA, order), 120);
                nellaDialog.Insert(0, "- ");
            }
            else
            {
                nellaDialog.Clear();
                nellaDialog.Append("- ");
                nellaDialog.Append(ReadDialogs((int)DialogType.NELLA, order));
            }
            nellaText1.text = nellaDialog.ToString();
            nellaText2.text = nellaText1.text;
            nellaRealText1.text = nellaText1.text;
            nellaRealText2.text = nellaText1.text;
            nellaText1.gameObject.SetActive(true);
            nellaText2.gameObject.SetActive(true);
        }

        public void SetSteadyDialog(int order)
        {
            if (!isSubOn)
                return;
            SetSubtitleBG(isSubBGOn);
            if (steadyDialog == null)
            {
                steadyDialog = new(ReadDialogs((int)DialogType.STEADY, order), 120);
                steadyDialog.Insert(0, "- ");
            }
            else
            {
                steadyDialog.Clear();
                steadyDialog.Append("- ");
                steadyDialog.Append(ReadDialogs((int)DialogType.STEADY, order));
            }
            steadyText1.text = steadyDialog.ToString();
            steadyText2.text = steadyText1.text;
            steadyRealText1.text = steadyText1.text;
            steadyRealText2.text = steadyText1.text;
            steadyText1.gameObject.SetActive(true);
            steadyText2.gameObject.SetActive(true);
        }

        public void CleanSet()
        {
            etcText1.gameObject.SetActive(false);
            etcText2.gameObject.SetActive(false);
            nellaText1.gameObject.SetActive(false);
            nellaText2.gameObject.SetActive(false);
            steadyText1.gameObject.SetActive(false);
            steadyText2.gameObject.SetActive(false);
        }

        public void GoUp()
        {
            nellaText1.rectTransform.anchoredPosition = new Vector2(nellaText1.rectTransform.anchoredPosition.x, nellaText1.rectTransform.anchoredPosition.y + 100f);
            nellaText2.rectTransform.anchoredPosition = new Vector2(nellaText2.rectTransform.anchoredPosition.x, nellaText2.rectTransform.anchoredPosition.y + 100f);
            steadyText1.rectTransform.anchoredPosition = new Vector2(steadyText1.rectTransform.anchoredPosition.x, steadyText1.rectTransform.anchoredPosition.y + 100f);
            steadyText2.rectTransform.anchoredPosition = new Vector2(steadyText2.rectTransform.anchoredPosition.x, steadyText2.rectTransform.anchoredPosition.y + 100f);
        }

    }

}
