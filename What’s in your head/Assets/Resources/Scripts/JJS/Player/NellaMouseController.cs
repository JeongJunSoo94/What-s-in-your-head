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
            cameraMain = this.gameObject.transform.GetComponent<CameraController>().FindCamera(); // ��Ƽ��
            gun.mainCamera = cameraMain; // ��Ƽ��
            if (point == null)
            {
                point = GameObject.FindGameObjectWithTag("NellaMousePoint");
                gun.mousePoint = point;
            }
        }

        // << : ������Ʈ �浹 üũ ���� ����
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
                            // << : �ڶ� ��Ÿ ���� ����޽��� ���� YC (��Ÿ�� �ν��� �浹�� �ν����� ����޽���(�ν� ����Ʈ �� ����)
                            if (hitObjs[i].HitColliders[j].gameObject.CompareTag("Bush"))
                            {
                                hitObjs[i].HitColliders[j].gameObject.SendMessage("Attacked", 0.3f);
                            }                         
                        }
                    }
                }
            }
        }
    }

}
