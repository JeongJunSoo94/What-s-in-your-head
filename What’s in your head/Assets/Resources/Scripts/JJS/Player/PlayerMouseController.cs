using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using JCW.UI.Options.InputBindings;
namespace JJS
{
    public class PlayerMouseController : MonoBehaviour
    {
        [Serializable]
        public struct WeaponInfo
        {
            public GameObject weapon;
            public bool canAim;
            public bool canNoAimAttack;
            public bool canMoveAttack;
        }


        public GameObject point;
        public Camera cameraMain;
        //public GameObject[] weapon;
        //public bool[] weaponAimCheck;
        [Header("조준, 무조준 공격, 공격 중 이동")] public List<WeaponInfo> weaponInfo;
        public IKController ik;

        public bool ableToLeft;
        public bool ableToRight;

        Ray ray;
        RaycastHit hit;
        private void Awake()
        {
            ableToLeft = false;
            ableToRight = false;
            ik = GetComponent<IKController>();
        }
        public virtual void SetWeaponEnable(int weaponIndex, bool enable)
        {

        }

        public virtual int GetUseWeapon()
        {
            if (weaponInfo.Count != 0)
            {
                for (int i = 0; i < weaponInfo.Count; ++i)
                {
                    if (weaponInfo[i].weapon.activeSelf)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public virtual void AimUpdate(int type = 0)
        {

        }

        public virtual void TopViewUpdate()
        {
            ray = cameraMain.ScreenPointToRay(Input.mousePosition);
            int layerMask = (-1) - (1 << LayerMask.NameToLayer("Player"));
            if (Physics.Raycast(ray, out hit, 100, layerMask, QueryTriggerInteraction.Ignore))
            {
                point.transform.position = hit.point;
            }
        }

        public virtual void WeaponSwap()
        {
            if (weaponInfo.Count != 0)
            {
                for (int i = 0; i < weaponInfo.Count; ++i)
                {
                    weaponInfo[i].weapon.SetActive(!weaponInfo[i].weapon.activeSelf);
                }
            }
        }

        public virtual bool GetCustomInfo()
        {
            return false;
        }

        //public virtual bool InputLeftMouseButton()
        //{
            
        //}

        //public virtual bool InputLeftMouseButtonDown()
        //{

        //}

        //public virtual bool InputRightMouseButton()
        //{
            
        //}

        //public virtual bool InputRightMouseButtonDown()
        //{

        //}
    }

}
