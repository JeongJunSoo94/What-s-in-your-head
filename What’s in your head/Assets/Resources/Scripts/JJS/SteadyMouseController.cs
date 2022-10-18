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
        public GameObject leftWeapon;
        public GameObject rightWeapon;
        public MagnifyingGlass glass;
        private void Awake()
        {
            //if (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Joined)
            //    glass.mainCamera = this.gameObject.transform.GetComponent<CameraController>().FindCamera(); // 멀티용
            //else
            //    glass.mainCamera = this.gameObject.transform.GetComponent<CameraController_Single>().FindCamera(); // 싱글용

        }
        public override void CheckLeftClick(int enable)
        {
            leftWeapon.SetActive(enable == 1);
        }

        public override void CheckRightClick(int enable)
        {
            rightWeapon.SetActive(enable == 1);
        }
    }

}
