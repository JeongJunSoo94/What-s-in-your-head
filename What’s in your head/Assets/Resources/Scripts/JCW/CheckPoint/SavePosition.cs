using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;

public class SavePosition : MonoBehaviour
{
    [SerializeField]    private int nthCheckPoint = 0;
    [SerializeField]    private GameObject player1 = null;
    [SerializeField]    private GameObject player2 = null;

    private ItTakesTwoPlayerControler player1_script = null;
    private ItTakesTwoPlayerControler player2_script = null;

    [Serializable]
    public class PlayerInfo
    {        
        public double[] position;
        public double[] rotation;

        public PlayerInfo(GameObject _other)
        {
            position = new double[3];
            rotation = new double[4];

            position[0] = (double)_other.transform.position.x;
            position[1] = (double)_other.transform.position.y;
            position[2] = (double)_other.transform.position.z;
            rotation[0] = (double)_other.transform.rotation.x;
            rotation[1] = (double)_other.transform.rotation.y;
            rotation[2] = (double)_other.transform.rotation.z;
            rotation[3] = (double)_other.transform.rotation.w;


        }
    }

    private void Start()
    {
        player1_script = player1.GetComponent<ItTakesTwoPlayerControler>();
        player2_script = player2.GetComponent<ItTakesTwoPlayerControler>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (player1 == other.gameObject && player1_script.CPcount == nthCheckPoint)
            {
                ++player1_script.CPcount;
                Check(other.gameObject);
            }
            else if (player2 == other && player2_script.CPcount == nthCheckPoint)
            {
                ++player2_script.CPcount;
                Check(other.gameObject);
            }
        }
    }

    private void Check(GameObject other)
    {
        Debug.Log("체크포인트 접촉 : " + other.name);
        PlayerInfo playerTF = new PlayerInfo(other);
        JsonData infoJson = JsonMapper.ToJson(playerTF);
        if (!Directory.Exists(Application.dataPath + "/Resources/CheckPointInfo/"))
            Directory.CreateDirectory(Application.dataPath + "/Resources/CheckPointInfo/");
        File.WriteAllText(Application.dataPath + "/Resources/CheckPointInfo/" +other.name + "TF" +nthCheckPoint +".json", infoJson.ToString());
        Debug.Log("체크포인트 저장");
    }
}
