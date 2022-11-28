using System;
using UnityEngine;
using System.IO;
using LitJson;
using Photon.Pun;
using System.Text;

namespace JCW.Object
{
    [RequireComponent(typeof(PhotonView))]
    public class SavePosition : MonoBehaviour
    {
        private bool firstContact = false;
        PhotonView photonView;
        StringBuilder filePath;
        StringBuilder fileName;

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
            photonView = GetComponent<PhotonView>();
            filePath = new(240, 240);
            fileName = new(300, 300);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Nella") || other.CompareTag("Steady"))
            {
                if (!firstContact)
                {
                    if (!GameManager.Instance.GetCharOnScene())
                        return;
                    if (!other.GetComponent<PlayerState>().isMine)
                        return;
                    firstContact = true;
                    Vector3 pos = transform.position;
                    Quaternion rot = transform.rotation;
                    photonView.RPC(nameof(Check), RpcTarget.AllViaServer, pos, rot);
                }
            }
        }
        [PunRPC]
        public void Check(Vector3 pos, Quaternion rot)
        {
            filePath.Clear();
            firstContact = true;
            PlayerInfo playerTF = new(pos, rot);
            JsonData infoJson = JsonMapper.ToJson(playerTF);

            int curStage = GameManager.Instance.curStageIndex;
            int curStageType = GameManager.Instance.curStageType;

            filePath.Append(Application.streamingAssetsPath);
            filePath.Append("/CheckPointInfo/Stage");
            filePath.Append(curStage);
            filePath.Append("/");
            filePath.Append(curStageType);
            filePath.Append("/");
            ++GameManager.Instance.curSection;

            if (!Directory.Exists(filePath.ToString()))
                Directory.CreateDirectory(filePath.ToString());
            fileName.Clear();

            fileName.Append(filePath.ToString());
            fileName.Append("Section");
            fileName.Append(GameManager.Instance.curSection);
            fileName.Append(".json");
            File.WriteAllText(fileName.ToString(), infoJson.ToString());
            //Debug.Log("체크포인트 저장");
            Destroy(this);
        }
    }
}
