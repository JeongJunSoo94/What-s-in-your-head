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
            canSwap = true;

            player = GetComponent<PlayerController>();
        }

        public override void AimUpdate(int type=0)
        {
            gun.ShootLine(type);
        }
      
        private void FixedUpdate()
        {
            if (photonView.IsMine)
            {
                if (player.characterState.aim)
                {
                    if (!player.characterState.IsJumping && !player.characterState.IsAirJumping
                        && !player.characterState.IsDashing && !player.characterState.IsAirDashing)
                    {
                        if (KeyManager.Instance.GetKey(PlayerAction.Fire) && GetUseWeapon() == 0)
                        {
                            photonView.RPC(nameof(SetWeaponEnable), RpcTarget.AllViaServer, 0, true);
                          
                        }
                        else
                        {
                            if (clickLeft)
                            {
                                photonView.RPC(nameof(SetWeaponEnable), RpcTarget.AllViaServer, 0, false);
                            }
                        }
                    }
                    else
                    {
                        if (clickLeft)
                        {
                            photonView.RPC(nameof(SetWeaponEnable), RpcTarget.AllViaServer, 0, false);
                            clickLeft = false;
                        }
                    }
                }
                else
                {
                    if (clickLeft)
                    {
                        photonView.RPC(nameof(SetWeaponEnable), RpcTarget.AllViaServer, 0, false);
                        clickLeft = false;
                    }
                }

                //if (player.characterState.swap)
                //{
                //    if (player.characterState.top)
                //    {
                //        if (player.characterState.aim)
                //        {
                //            photonView.RPC(nameof(WeaponSwap), RpcTarget.AllViaServer);
                //        }
                //    }
                //    else if (!player.characterState.aim)
                //    {
                //        photonView.RPC(nameof(WeaponSwap), RpcTarget.AllViaServer);
                //    }
                //}
            }

            //SetWeaponEnable(GetPlayerController(animator).playerMouse.GetUseWeapon(), false)
        }


        [PunRPC]
        public override void SetWeaponEnable(int weaponIndex,bool enable)
        {

            if (enable)
            {
                if (player.characterState.top)
                {
                    AimUpdate(2);
                }
                clickLeft = true;
                gun.ShootStart();
            }
            else
            {
                gun.ShootStop();
                clickLeft = false;
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
        }

        public void OnDisableObject(int index)
        {
            hitObjs[index].gameObject.SetActive(false);
        }

        public void AfterDelayTime(int index)
        {
            afterDelayTime = !afterDelayTime;
            OnDisableObject(index);
        }


        public void TargetUpdate()
        {
            Debug.Log("CHECK!");

            for (int i = 0; i < hitObjs.Count; i++)
            {
                Debug.Log("ㅇㅇㅇㅇ");
                if (hitObjs[i].gameObject.activeSelf)
                {
                    Debug.Log("부딪힌 오브젝트 켜져있음");
                    if (hitObjs[i].targetObj.Count != 0)
                    {
                        Debug.Log("부딪힌 오브젝트 개수 1개 이상");
                        for (int j = 0; j < hitObjs[i].HitColliders.Length; j++)
                        {
                            Debug.Log("부쉬와 충돌 체크");
                            // << : 찬 수정, 부쉬 오브젝트 관련
                            if (hitObjs[i].HitColliders[j].gameObject.CompareTag("Bush"))
                            {
                                Debug.Log("부쉬와 충돌");
                                hitObjs[i].HitColliders[j].gameObject.SendMessage("Attacked", 0.01f);
                            }
                        }
                    }
                }
            }
        }
    }

}
