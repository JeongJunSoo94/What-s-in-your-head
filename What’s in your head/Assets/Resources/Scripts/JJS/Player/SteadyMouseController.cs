using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YC.Camera_;
using YC.Camera_Single;
using Photon.Pun;
using JJS.Weapon;
namespace JJS
{
    public class SteadyMouseController : PlayerMouseController
    {
        public MagnifyingGlass glass;
        private void Awake()
        {
            if (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Joined)
            {
                glass.mainCamera = this.gameObject.transform.GetComponent<CameraController>().FindCamera(); // ��Ƽ��
                cameraMain = this.gameObject.transform.GetComponent<CameraController>().FindCamera(); // ��Ƽ��
            }
            else
            {
                glass.mainCamera = this.gameObject.transform.GetComponent<CameraController_Single>().FindCamera(); // �̱ۿ�
                cameraMain = this.gameObject.transform.GetComponent<CameraController_Single>().FindCamera(); // �̱ۿ�
            }
            if (point == null)
            {
                point = Resources.Load("Prefabs/JJS/SteadyMousePoint") as GameObject;
                Instantiate(point);
                glass.mousePoint = point;
            }
        }
        public override void AimUpdate(int type = 0)
        {
            glass.HitLine();
        }
    }

}
