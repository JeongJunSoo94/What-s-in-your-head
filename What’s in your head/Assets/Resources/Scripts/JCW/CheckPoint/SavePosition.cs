using System;
using UnityEngine;
using System.IO;
using LitJson;
using Photon.Pun;

namespace JCW.Object
{
    [RequireComponent(typeof(PhotonView))]
    public class SavePosition : MonoBehaviour
    {
        private bool firstContact = false;
        PhotonView photonView;

        [Serializable]
        public class PlayerInfo
        {
            public double[] position;
            public double[] rotation;

            public PlayerInfo(GameObject _other)
            {
                position = new double[3] { (double)_other.transform.position.x, (double)_other.transform.position.y, (double)_other.transform.position.z };
                rotation = new double[4] { (double)_other.transform.rotation.x, (double)_other.transform.rotation.y, (double)_other.transform.rotation.z, (double)_other.transform.rotation.w };
            }

            public PlayerInfo(Vector3 _pos, Quaternion _rot)
            {
                position = new double[3] { (double)_pos.x, (double)_pos.y, (double)_pos.z };
                rotation = new double[4] { (double)_rot.x, (double)_rot.y, (double)_rot.z, (double)_rot.w };
            }
        }

        private void Awake()
        {
            photonView = PhotonView.Get(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Nella") || other.CompareTag("Steady"))
            {
                if (!firstContact)
                {                    
                    if (PhotonNetwork.PlayerList.Length < 2)
                        return;
                    firstContact = true;
                    if (!other.GetComponent<PlayerState>().isMine)
                        return;
                    Vector3 pos = transform.position;
                    Quaternion rot = transform.rotation;                    
                    photonView.RPC(nameof(Check), RpcTarget.AllViaServer, pos, rot);
                }
            }
        }
        [PunRPC]
        private void Check(Vector3 pos, Quaternion rot)
        {
            firstContact = true;
            PlayerInfo playerTF = new(pos, rot);
            JsonData infoJson = JsonMapper.ToJson(playerTF);

            int curStage = GameManager.Instance.curStageIndex;
            int curStageType = GameManager.Instance.curStageType;
            ++GameManager.Instance.curSection;

            string path = Application.dataPath + "/Resources/CheckPointInfo/Stage" + curStage + "/" + curStageType + "/";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            File.WriteAllText(path + "Section" +GameManager.Instance.curSection +".json", infoJson.ToString());            
            //Debug.Log("체크포인트 저장");
            Destroy(this);
        }
    }
}

