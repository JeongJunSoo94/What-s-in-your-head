using System;
using UnityEngine;
using System.IO;
using LitJson;
using Photon.Pun;

namespace JCW.Object
{
    [RequireComponent(typeof(PhotonView))]
    public class SavePosition : MonoBehaviour, IPunObservable
    {
        private bool firstContact = false;

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

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Nella") || other.CompareTag("Steady"))
            {
                if (!firstContact)
                {
                    firstContact = true;
                    Vector3 pos = other.gameObject.transform.position;
                    Quaternion rot = other.gameObject.transform.rotation;
                    GetComponent<PhotonView>().RPC(nameof(Check), RpcTarget.AllViaServer, pos, rot);
                }
            }
        }
        [PunRPC]
        private void Check(Vector3 pos, Quaternion rot)
        {
            ++GameManager.Instance.curSection;
            Debug.Log("체크포인트 접촉 : " + GameManager.Instance.curSection);
            PlayerInfo playerTF = new(pos, rot);
            JsonData infoJson = JsonMapper.ToJson(playerTF);

            int curStage = GameManager.Instance.curStageIndex;
            if (!Directory.Exists(Application.dataPath + "/Resources/CheckPointInfo/Stage" + curStage + "/"))
                Directory.CreateDirectory(Application.dataPath + "/Resources/CheckPointInfo/Stage" + curStage + "/");
            File.WriteAllText(Application.dataPath + "/Resources/CheckPointInfo/Stage" + curStage + "/Section" +GameManager.Instance.curSection +".json", infoJson.ToString());
            Debug.Log("체크포인트 저장");
            Destroy(this);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(firstContact);
            }
            else
            {
                this.firstContact = (bool)stream.ReceiveNext();
            }
        }
    }
}

