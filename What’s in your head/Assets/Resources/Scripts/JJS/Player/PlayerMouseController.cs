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

        [HideInInspector] public bool ableToLeft;
        [HideInInspector] public bool ableToRight;

        [HideInInspector] public bool clickLeft;
        [HideInInspector] public bool clickRight;

        public LayerMask mouseLayer;
        public int mouseRayDistance;

        protected Ray ray;
        protected RaycastHit hit;

        public bool canSwap;
        public float curCool=0f;
        public float swapCool=0.5f;

        private void Awake()
        {
            ableToLeft = false;
            ableToRight = false;
            canSwap = true;
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
            if (Physics.Raycast(ray, out hit, mouseRayDistance, mouseLayer, QueryTriggerInteraction.Ignore))
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

        public void SwapCoroutine()
        {
            if (canSwap)
            { 
                StartCoroutine(SwapCoolTime());
            }
        }



        IEnumerator SwapCoolTime()
        {
            canSwap = false;
            while (curCool < swapCool)
            {
                curCool += 0.01f;
                yield return new WaitForSeconds(0.01f);
            }
            curCool = 0;
            canSwap = true;
            yield break;
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
