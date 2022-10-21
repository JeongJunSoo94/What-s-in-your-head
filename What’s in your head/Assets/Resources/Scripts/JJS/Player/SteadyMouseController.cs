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
                glass.mainCamera = this.gameObject.transform.GetComponent<CameraController>().FindCamera(); // 멀티용
                cameraMain = this.gameObject.transform.GetComponent<CameraController>().FindCamera(); // 멀티용
            }
            else
            {
                glass.mainCamera = this.gameObject.transform.GetComponent<CameraController_Single>().FindCamera(); // 싱글용
                cameraMain = this.gameObject.transform.GetComponent<CameraController_Single>().FindCamera(); // 싱글용
            }
            if (point == null)
            {
                point = GameObject.FindGameObjectWithTag("NellaMousePoint");
                glass.mousePoint = point;
            }
        }
        public void StopBeam()
        {
            glass.StopBeam();
        }
        public void Shoot()
        {
            glass.Shoot();
        }
        public override void AimUpdate(int type = 0)
        {
            glass.HitLine();
        }
    }

}
