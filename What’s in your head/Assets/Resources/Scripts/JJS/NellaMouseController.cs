using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.Options.InputBindings;
namespace JJS
{
    public class NellaMouseController : PlayerMouseController
    {
        public List<Discovery3D> hitObjs;

        public GameObject leftWeapon;
        public GameObject rightWeapon;
        public WaterGun gun;
        private void Update()
        {
            TargetUpdate();
        }


        public override void CheckLeftClick(bool enable)
        {
            leftWeapon.SetActive(enable);
        }

        public override void CheckRightClick(bool enable)
        {
            rightWeapon.SetActive(enable);
        }

        public void Shoot()
        {
            gun.Shoot();
        }

        public void OnEnableObject(int index)
        {
            hitObjs[index].gameObject.SetActive(true);
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
