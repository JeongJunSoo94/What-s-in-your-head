using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YC.Camera_;
using YC.Camera_Single;
using Photon.Pun;
using JJS.Weapon;
using KSU;
using JCW.UI.Options.InputBindings;

namespace JJS
{
    public class SteadyMouseController : PlayerMouseController
    {
        public MagnifyingGlass glass;
        public SteadyGrappleAction grapple;

        PlayerController player;
        //public SteadyGrappleAction grapple;
        private void Awake()
        {
            cameraMain = this.gameObject.transform.GetComponent<CameraController>().FindCamera(); // 멀티용
            glass.mainCamera = cameraMain; // 멀티용
            if (point == null)
            {
                point = GameObject.FindGameObjectWithTag("SteadyMousePoint");
                glass.mousePoint = point;
            }
            canSwap = true;
            photonView = GetComponent<PhotonView>();

            player = GetComponent<PlayerController>();
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                if (player.characterState.aim)
                {
                    if (KeyManager.Instance.GetKey(PlayerAction.Fire) && GetUseWeapon() == 1)
                    {
                        clickLeft = true;
                    }
                    else
                    {
                        clickLeft = false;
                    }
                }
                else
                {
                    clickLeft = false;
                }
            }

            //SetWeaponEnable(GetPlayerController(animator).playerMouse.GetUseWeapon(), false)
        }

        public void StopBeam()
        {
            notRotatoin = false;
            glass.StopBeam();
        }
        public void Shoot()
        {
            notRotatoin = true;
            glass.Shoot();
        }
        public override void AimUpdate(int type = 0)
        {
            glass.HitLine(type);
        }

        public override bool GetCustomInfo()
        {
            return grapple.GetWhetherHit();
        }

        //public virtual bool InputLeftMouseButton()
        //{
        //    if (KeyManager.Instance.GetKey(PlayerAction.Fire))
        //    {

        //    }
        //}

        //public virtual bool InputLeftMouseButtonDown()
        //{
        //    if (KeyManager.Instance.GetKeyDown(PlayerAction.Fire))
        //    {

        //    }
        //}

        //public virtual bool InputRightMouseButton()
        //{
        //    if (KeyManager.Instance.GetKeyDown(PlayerAction.Fire))
        //    {

        //    }
        //}

        //public virtual bool InputRightMouseButtonDown()
        //{
        //    if (KeyManager.Instance.GetKeyDown(PlayerAction.Fire))
        //    {

        //    }
        //}
    }

}
