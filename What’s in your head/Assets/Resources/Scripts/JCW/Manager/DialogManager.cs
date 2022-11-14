using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using UnityEngine.UI;

namespace JCW.Dialog
{
    public class csvReader
    {
        static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
        static char[] TRIM_CHARS = { '\"' };

        static StringBuilder filePath = new(100, 200);

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
        //�ش� ��������, �ش� ������ ��� ����
        List<string> nellaDialogs = new();
        List<string> steadyDialogs = new();
        List<string> etcDialogs = new();

        // �Ľ��ؼ� ������ �����͵�. ���� ����Ʈ�鿡 �־��� �뵵.
        List<Dictionary<string, object>> parsingString = null; 

        // �ش� ������ �����ϴ� �� ���θ� üũ�ϱ� ���� ����
        StringBuilder existCheck = null;

        // UI�� ���� �� �� �� ����� ���ڿ�
        StringBuilder etcDialog = null;
        StringBuilder nellaDialog = null;
        StringBuilder steadyDialog = null;

        public Text etcText1;       public Text etcText2;
        public Text nellaText1;     public Text nellaText2;
        public Text steadyText1;    public Text steadyText2;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
                Destroy(this.gameObject);
        }
        // �������� ���� �� �ҷ��ָ� �ɵ�.
        public void SetDialogs(string fileName)
        {
            if (existCheck == null)
                existCheck = new(100,100);
            else
                existCheck.Clear();

            existCheck.Append(Application.streamingAssetsPath + "/Dialogs/");
            existCheck.Append(fileName);
            existCheck.Append(".csv");

            if (!File.Exists(existCheck.ToString()))
            {
                Debug.Log("�ҷ��� " + existCheck + " ������ �����ϴ�");
                return;
            }

            parsingString = csvReader.Read(fileName);
            if (fileName.Contains("_N"))
            {
                for (int i=0 ; i<parsingString.Count ; ++i)
                {
                    nellaDialogs.Add(parsingString[i]["Script"].ToString());
                }
            }
            else if (fileName.Contains("_S"))
            {
                for (int i = 0 ; i < parsingString.Count ; ++i)
                {
                    steadyDialogs.Add(parsingString[i]["Script"].ToString());
                }
            }
            else if (fileName.Contains("_E"))
            {
                for (int i = 0 ; i < parsingString.Count ; ++i)
                {
                    etcDialogs.Add(parsingString[i]["Script"].ToString());
                }
            }
            parsingString.Clear();
            parsingString = null;
        }

        // �ٸ� ������ ��� ��ũ��Ʈ�� ���� �� ���, ���� ���°�� ������ ����ϸ� �Ǵ���..
        public string ReadDialogs(int type, int order)
        {
            if (order == 0)
                return null;
            switch (type)
            {
                case (int)DialogType.NELLA :
                    return nellaDialogs[order - 1];
                case (int)DialogType.STEADY :
                    return steadyDialogs[order - 1];
                case (int)DialogType.ETC :
                    return etcDialogs[order - 1];
                default:
                    break;
            }

            return null;
        }
        // �������� ���� �� �ҷ��ָ� �ɵ�.
        public void ResetDialogs()
        {
            nellaDialogs.Clear();
            steadyDialogs.Clear();
            etcDialogs.Clear();

            if(parsingString.Count > 0)
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

        public void SetEtcDialog(int order)
        {
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
            etcText1.enabled = true;
            etcText2.enabled = true;
        }

        public void SetNellaDialog(int order)
        {
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
            nellaText1.enabled = true;
            nellaText2.enabled = true;
        }

        public void SetSteadyDialog(int order)
        {
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
            steadyText1.enabled = true;
            steadyText2.enabled = true;
        }

        public void CleanSet()
        {
            etcText1.enabled = false;
            etcText2.enabled = false;
            nellaText1.enabled = false;
            nellaText2.enabled = false;
            steadyText1.enabled = false;
            steadyText2.enabled = false;
        }

    }

}
