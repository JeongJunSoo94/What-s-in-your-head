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

        string watergunName = "WaterPistol";
        string micName = "Mic";

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

            if (point == null)
            {
                point = GameObject.FindGameObjectWithTag("NellaMousePoint");
                DontDestroyOnLoad(point);
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
            gun = GetComponent<WaterGun>();
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

        private void Update()
        {
            if (GameManager.Instance.curPlayerHP <= 0
                && GameManager.Instance.curStageIndex == 2)
            {
                if(photonView.IsMine)
                    photonView.RPC(nameof(RPCInitSinging), RpcTarget.AllViaServer);
            }
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
                            && player.characterState.IsGrounded
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
                            && !player.characterState.IsDashing && !player.characterState.IsAirDashing && !player.playerAnimator.GetBool("isDead"))
                        {
                            if (KeyManager.Instance.GetKey(PlayerAction.Fire) && weaponInfo[GetUseWeapon()].canAim && GameManager.Instance.curPlayerHP > 0)
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
                              && !player.characterState.IsDashing && !player.characterState.IsAirDashing
                              && !player.playerAnimator.GetBool("isDead"))
                        {
                            if (KeyManager.Instance.GetKey(PlayerAction.Fire) && !weaponInfo[GetUseWeapon()].canAim && GameManager.Instance.curPlayerHP>0)
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
            if (weaponInfo[GetUseWeapon()].weapon.name == watergunName)
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
            else if (weaponInfo[GetUseWeapon()].weapon.name == micName)
                singing.enabled = clickLeft = enable;
            else
                clickLeft = enable;
        }

        public void OnEnableObject(int index)
        {
            if (hitObjs[index].gameObject.activeSelf)
                hitObjs[index].gameObject.SetActive(false);
            hitObjs[index].gameObject.SetActive(true);
            switch (index)
            {
                case 0:
                    SoundManager.Instance.PlayEffect_RPC("S3_NellaGuitarAttack1");
                    break;
                case 1:
                    SoundManager.Instance.PlayEffect_RPC("S3_NellaGuitarAttack2");
                    break;
                case 2:
                    SoundManager.Instance.PlayEffect_RPC("S3_NellaGuitarAttack3");
                    break;
            }
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

        public void RPCInitSinging()
        {
            singing.InitSinging();
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

                            if (hitObjs[i].HitColliders[j].gameObject.CompareTag("PoisonSnake") || hitObjs[i].HitColliders[j].gameObject.CompareTag("TrippleHeadSnake"))
                            {
                                hitObjs[i].HitColliders[j].gameObject.SendMessage("GetDamage", 3);
                            }
                        }
                    }
                }
            }
        }
    }

}
