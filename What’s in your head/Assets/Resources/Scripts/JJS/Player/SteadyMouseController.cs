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
        //public SteadyGrappleAction grapple;
        private void Awake()
        {
            cameraMain = this.gameObject.transform.GetComponent<CameraController>().FindCamera(); // 멀티용
            glass.mainCamera = cameraMain; // 멀티용
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
