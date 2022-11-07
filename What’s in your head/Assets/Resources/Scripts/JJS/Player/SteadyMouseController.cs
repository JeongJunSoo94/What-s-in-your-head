using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YC.Camera_;
using YC.Camera_Single;
using Photon.Pun;
using JJS.Weapon;
using KSU;
using JCW.UI.Options.InputBindings;
using KSU.AutoAim.Player;

namespace JJS
{
    public class SteadyMouseController : PlayerMouseController
    {
        public MagnifyingGlass glass;
        public SteadyGrappleAction grapple;

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
            canAim = true;
            player = GetComponent<PlayerController>();
        }

        private void Update()
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
                            && !GetComponent<SteadyInteractionState>().isGrappling
                            && !GetComponent<SteadyInteractionState>().isGrabMonster
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
            }
                
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
        public override bool GetCustomInfo(AutoAimTargetType grappleTargetType)
        {
            return grapple.GetWhetherHit(grappleTargetType);
        }

       
    }

}
