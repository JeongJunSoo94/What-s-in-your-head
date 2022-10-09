using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.Options.InputBindings;
namespace JJS
{
    public class NellaMouseController : PlayerMouseController
    {
        public List<Discovery3D> hitObjs;

        public WaterGun gun;
        private void Update()
        {
            TargetUpdate();
        }

        public override void CheckLeftClick()
        {
            if (ITT_KeyManager.Instance.GetKey(PlayerAction.Fire))
            {
                leftOn = true;
            }
            else
            {
                leftOn = false;
            }
        }

        public override void CheckRightClick()
        {
            if (ITT_KeyManager.Instance.GetKey(PlayerAction.Aim))
            {
                rightOn = true;
            }
            else
            {
                rightOn = false;
            }
        }

        public override void CheckLeftDownClick()
        {
            if (ITT_KeyManager.Instance.GetKeyDown(PlayerAction.Fire))
            {
                leftDown = true;
            }
            else
            {
                leftDown = false;
            }
        }

        public override void CheckRightDownClick()
        {
            if (ITT_KeyManager.Instance.GetKeyDown(PlayerAction.Aim))
            {
                rightDown = true;
            }
            else
            {
                rightDown = false;
            }
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
