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
                point = GameObject.FindGameObjectWithTag("SteadyMousePoint");
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
            Vector3 mousePos = Input.mousePosition;
            float x = mousePos.x * (1 - cameraMain.rect.width);
            mousePos.x += x;
            ray = cameraMain.ScreenPointToRay(mousePos);
            int layerMask = (-1) - (1 << LayerMask.NameToLayer("Player"));
            if (Physics.Raycast(ray, out hit, 100, layerMask, QueryTriggerInteraction.Ignore))
            {
                point.transform.position = hit.point;
            }
            else
            {
                Vector3 dir = cameraMain.transform.forward;
                point.transform.position = glass.startPos.transform.position + dir * glass.maxDistance;
            }
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
