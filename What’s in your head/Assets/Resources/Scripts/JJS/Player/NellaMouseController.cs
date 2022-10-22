using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YC.Camera_;
using YC.Camera_Single;
using Photon.Pun;
using JJS.Weapon;

namespace JJS
{
    public class NellaMouseController : PlayerMouseController
    {
        public List<Discovery3D> hitObjs;

        public WaterGun gun;

        public int bulletCount=0;

        


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
        }

        // << : 오브젝트 충돌 체크 위해 수정
        private void Update()
        {
            TargetUpdate();
        }

        public override void AimUpdate(int type=0)
        {
            gun.ShootLine(type);
        }

        public override void SetWeaponEnable(int weaponIndex,bool enable)
        {
            if (weaponInfo.Count != 0)
            {
                weaponInfo[weaponIndex].weapon.SetActive(enable);
            }
        }

        public void Shoot()
        {
            gun.Shoot();
            bulletCount++;
        }

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
                            // << : 넬라 기타 어택 센드메시지 수정 YC
                            if (hitObjs[i].HitColliders[j].gameObject.layer == LayerMask.NameToLayer("Platform"))
                            {
                                hitObjs[i].HitColliders[j].gameObject.SendMessage("Attacked");
                            }
                        }
                    }
                }
            }
        }
    }

}
