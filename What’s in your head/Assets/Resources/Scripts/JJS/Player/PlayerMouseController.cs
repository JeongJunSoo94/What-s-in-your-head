using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
namespace JJS
{
    public class PlayerMouseController : MonoBehaviour
    {
        public GameObject point;
        public Camera cameraMain;
        public GameObject[] Weapon;
        public bool[] WeaponAimCheck;
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
            if (Weapon.Length != 0)
            {
                for (int i = 0; i < Weapon.Length; ++i)
                {
                    if (Weapon[i].activeSelf)
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
            if (Weapon.Length != 0)
            {
                for (int i = 0; i < Weapon.Length; ++i)
                {
                    Weapon[i].SetActive(!Weapon[i].activeSelf);
                }
            }
        }

    }

}
