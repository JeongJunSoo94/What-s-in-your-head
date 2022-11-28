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
    public enum WeaponName { Mic, Cymbals, Guitar, WaterGun, Grapple, MagnifyingGlass, Null };
    public class SteadyMouseController : PlayerMouseController
    {
        public MagnifyingGlass glass;
        //public SteadyGrappleAction grapple;
        [HideInInspector] public SteadyAutoAimAction autoAimWeapon;

        CameraController cameraController;

        private void Awake()
        {
            cameraController = gameObject.transform.GetComponent<CameraController>();
            cameraMain = cameraController.FindCamera(); // 멀티용

            SetAimWeapon();

            glass.mainCamera = cameraMain; // 멀티용
            if (point == null)
            {
                point = GameObject.FindGameObjectWithTag("SteadyMousePoint");
                glass.mousePoint = point;
                DontDestroyOnLoad(point);
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
                        if (player.playerAnimator.GetBool("isShootingCymbals") && !autoAimWeapon.GetWhetherautoAimObjectActived())
                        {
                            player.characterState.aim = true;
                            return;
                        }

                        if (KeyManager.Instance.GetKey(PlayerAction.Aim)
                            && !player.characterState.swap
                            && !player.characterState.IsJumping
                            && !player.characterState.IsAirJumping
                            && !player.characterState.IsDashing
                            && !player.characterState.IsAirDashing
                            && player.characterState.IsGrounded
                            && !GetComponent<SteadyInteractionState>().isGrappling
                            && !GetComponent<SteadyInteractionState>().isGrabMonster
                            && !player.playerAnimator.GetBool("WasShootingCymbals")
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


        public void ResetWeapon_RPC()
        {
            photonView.RPC(nameof(ResetWeapon), RpcTarget.AllViaServer);
        }

        [PunRPC]

        private void ResetWeapon()
        {
            autoAimWeapon.ResetAutoAimWeapon();
        }

        public void SetAimWeapon()
        {
            SteadyCymbalsAction steadyCymbalsWeapon = GetComponent<SteadyCymbalsAction>();
            SteadyGrappleAction steadyGrappleWeapon = GetComponent<SteadyGrappleAction>();
            if (steadyCymbalsWeapon.enabled)
            {
                autoAimWeapon = GetComponent<SteadyCymbalsAction>();
            }
            else if (steadyGrappleWeapon.enabled)
            {
                autoAimWeapon = GetComponent<SteadyGrappleAction>();
            }
        }

        public void StopBeam()
        {
            notRotatoin = false;
            glass.StopBeam();
        }

        public void Shoot()
        {
            if (photonView.IsMine)
            {
                photonView.RPC(nameof(StartBeam), RpcTarget.AllViaServer);
            }
        }

        [PunRPC]
        public void StartBeam()
        {
            notRotatoin = true;
            glass.Shoot();
        }


        public override void AimUpdate(int type = 0)
        {
            glass.HitLine(type);
        }
        public override bool GetCustomInfo(AutoAimTargetType autoAimTargetType)
        {
            return autoAimWeapon.GetWhetherHit(autoAimTargetType);
        }

        public override void InitMouseController()
        {
            StopBeam();
            cameraController.SetSteadyBeam(false);
        }
    }

}