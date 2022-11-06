using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YC.Camera_;
using YC.Camera_Single;
using Photon.Pun;
using JJS.Weapon;
using JCW.UI.Options.InputBindings;
using KSU;
using JCW.AudioCtrl;

namespace JJS
{
    [RequireComponent(typeof(PhotonView))]
    public class NellaMouseController : PlayerMouseController
    {
        public List<Discovery3D> hitObjs;

        public WaterGun gun;
        public Singing singing;

        private void Awake()
        {

            if (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Joined)
            {
                cameraMain = this.gameObject.transform.GetComponent<CameraController>().FindCamera(); // 멀티용
            }
            else
            {
                cameraMain = this.gameObject.transform.GetComponent<CameraController_Single>().FindCamera(); // 싱글용
            }
            wartergunInit();
            photonView = GetComponent<PhotonView>();
            player = GetComponent<PlayerController>();
            canSwap = true;
            canAim = true;
            if (weaponInfo.Length!=0)
                weaponInfo[0].weapon.SetActive(true);
        }

        public void wartergunInit()
        {
            point = GameObject.FindGameObjectWithTag("NellaMousePoint");
            if (gun != null)
            { 
                gun.mainCamera = cameraMain;
                gun.mousePoint = point;
            }
        }

        public override void AimUpdate(int type = 0)
        {
            gun.ShootLine(type);
        }

        private void FixedUpdate()
        {
            InputUpdate();
        }

        public void InputUpdate()
        {
            if (photonView.IsMine)
            {
                if (GetUseWeapon() == -1)
                {
                }
                else
                {
                    if (weaponInfo[GetUseWeapon()].canAim)
                    {
                        if (KeyManager.Instance.GetKey(PlayerAction.Aim)
                            && !player.characterState.swap
                            && !player.characterState.IsJumping
                            && !player.characterState.IsAirJumping
                            && !player.characterState.IsDashing
                            && !player.characterState.IsAirDashing
                            && !player.playerAnimator.GetBool("isDead"))
                        {
                            if (!clickRight)
                            {
                                AimCoroutine();
                                clickRight = true;
                            }
                            player.characterState.aim = true;
                        }
                        else
                        {
                            clickRight = false;
                            player.characterState.aim = false;
                        }

                    }

                    if (player.characterState.aim)
                    {
                        if (!player.characterState.IsJumping && !player.characterState.IsAirJumping
                            && !player.characterState.IsDashing && !player.characterState.IsAirDashing)
                        {
                            if (KeyManager.Instance.GetKey(PlayerAction.Fire) && weaponInfo[GetUseWeapon()].canAim)
                            {
                                if (gun.shootEnable && canAim)
                                {
                                    gun.ShootCoroutineEnable();
                                    photonView.RPC(nameof(SetWeaponEnable), RpcTarget.AllViaServer,true);
                                }
                            }
                            else
                            {
                                if (clickLeft)
                                {
                                    photonView.RPC(nameof(SetWeaponEnable), RpcTarget.AllViaServer,false);
                                }
                            }
                        }
                        else
                        {
                            if (clickLeft)
                            {
                                photonView.RPC(nameof(SetWeaponEnable), RpcTarget.AllViaServer, false);
                            }
                        }
                    }
                    else
                    {
                        if (!player.characterState.IsJumping && !player.characterState.IsAirJumping
                              && !player.characterState.IsDashing && !player.characterState.IsAirDashing)
                        {
                            if (KeyManager.Instance.GetKey(PlayerAction.Fire) && !weaponInfo[GetUseWeapon()].canAim)
                            {
                                photonView.RPC(nameof(SetWeaponEnable), RpcTarget.AllViaServer, true);
                            }
                            else
                            {
                                if (clickLeft)
                                {
                                    photonView.RPC(nameof(SetWeaponEnable), RpcTarget.AllViaServer, false);
                                }
                            }
                        }
                        else
                        {
                            if (clickLeft)
                            {
                                photonView.RPC(nameof(SetWeaponEnable), RpcTarget.AllViaServer, false);
                            }
                        }

                    }
                }
            }
        }

        [PunRPC]
        public override void SetWeaponEnable(bool enable)
        {
            if (weaponInfo[GetUseWeapon()].weapon.name == "WaterPistol")
            {
                if (enable)
                {
                    if (player.characterState.top)
                    {
                        AimUpdate(2);
                    }
                    else
                    {
                        AimUpdate(1);
                    }
                    clickLeft = true;
                    gun.ShootStart();
                }
                else
                    clickLeft = false;
            }
            else if (weaponInfo[GetUseWeapon()].weapon.name == "Mic")
                singing.enabled = clickLeft = enable;
            else
                clickLeft = enable;
        }

        public void OnEnableObject(int index)
        {
            hitObjs[index].gameObject.SetActive(true);
            SoundManager.Instance.PlayEffect_RPC("GuitarAttack");
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
            for (int i = 0; i < hitObjs.Count; i++)
            {
                if (hitObjs[i].gameObject.activeSelf)
                {
                    if (hitObjs[i].targetObj.Count != 0)
                    {
                        for (int j = 0; j < hitObjs[i].HitColliders.Length; j++)
                        {
                            // << : 찬 수정, 부쉬 오브젝트 관련
                            if (hitObjs[i].HitColliders[j].gameObject.CompareTag("Bush"))
                            {
                                hitObjs[i].HitColliders[j].gameObject.SendMessage("Attacked", 0.01f);
                            }
                        }
                    }
                }
            }
        }
    }

}
