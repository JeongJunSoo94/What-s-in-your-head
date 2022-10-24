using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YC.Camera_;
using YC.Camera_Single;
using Photon.Pun;
using JJS.Weapon;
using JCW.UI.Options.InputBindings;
using KSU;
namespace JJS
{
    [RequireComponent(typeof(PhotonView))]
    public class NellaMouseController : PlayerMouseController
    {
        public List<Discovery3D> hitObjs;

        public WaterGun gun;

        PhotonView photonView;

        PlayerController player;

        private void Awake()
        {
            if (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Joined)
            {
                gun.mainCamera = this.gameObject.transform.GetComponent<CameraController>().FindCamera(); // 멀티용
                cameraMain = this.gameObject.transform.GetComponent<CameraController>().FindCamera(); // 멀티용
            }
            else
            {
                gun.mainCamera = this.gameObject.transform.GetComponent<CameraController_Single>().FindCamera(); // 싱글용
                cameraMain = this.gameObject.transform.GetComponent<CameraController_Single>().FindCamera(); // 싱글용
            }
            if (point == null)
            {
                point = GameObject.FindGameObjectWithTag("NellaMousePoint");
                gun.mousePoint = point;
            }
            photonView = GetComponent<PhotonView>();
            player = GetComponent<PlayerController>();
        }

        public override void AimUpdate(int type=0)
        {
            Vector3 mousePos = Input.mousePosition;
            float x = mousePos.x * (1-cameraMain.rect.width);
            mousePos.x -= x;
            ray = cameraMain.ScreenPointToRay(mousePos);
            int layerMask = (-1) - (1 << LayerMask.NameToLayer("Player"));
            if (Physics.Raycast(ray, out hit, 100, layerMask, QueryTriggerInteraction.Ignore))
            {
                point.transform.position = hit.point;
            }
            else
            {
                Vector3 dir = cameraMain.transform.forward;
                point.transform.position = gun.startPos.transform.position + dir * gun.shootMaxDistance;
            }
            gun.ShootLine(type);
        }

        private void Update()
        {
            if(photonView.IsMine)
            {
                if (player.characterState.aim)
                {
                    if (KeyManager.Instance.GetKey(PlayerAction.Fire))
                    {
                        photonView.RPC(nameof(SetWeaponEnable), RpcTarget.AllViaServer, 0, true);
                        clickLeft = true;
                    }
                    else
                    {
                        photonView.RPC(nameof(SetWeaponEnable), RpcTarget.AllViaServer, 0, false);
                        clickLeft = false;
                    }
                }
                else
                {
                    photonView.RPC(nameof(SetWeaponEnable), RpcTarget.AllViaServer, 0, false);
                    clickLeft = false;
                }
            }
            
            //SetWeaponEnable(GetPlayerController(animator).playerMouse.GetUseWeapon(), false)
        }

        [PunRPC]
        public override void SetWeaponEnable(int weaponIndex,bool enable)
        {
            //if (weaponInfo.Count != 0)
            //{
            //    weaponInfo[weaponIndex].weapon.SetActive(enable);
            //}
            if (enable)
            {
                gun.ShootStart();
            }
            else
            {
                gun.ShootStop();
            }
        }

        //public void Shoot()
        //{
        //    gun.Shoot();
        //    bulletCount++;
        //}

        public void OnEnableObject(int index)
        {
            hitObjs[index].gameObject.SetActive(true);
            TargetUpdate();
        }

        public void OnDisableObject(int index)
        {
            hitObjs[index].gameObject.SetActive(false);
        }

        public void AttackTime()
        {
            ableToLeft = !ableToLeft;
        }


        void TargetUpdate()
        {
            for (int i = 0; i < hitObjs.Count; i++)
            {
                if (hitObjs[i].gameObject.activeSelf)
                {
                    if (hitObjs[i].targetObj.Count != 0)
                    {
                        for (int j = 0; j < hitObjs[i].HitColliders.Length; j++)
                        {
                            hitObjs[i].HitColliders[j].gameObject.SetActive(false);
                            //hitObjs[i].HitColliders[j].gameObject.SendMessage("");

                        }
                    }
                }
            }
        }
    }

}
