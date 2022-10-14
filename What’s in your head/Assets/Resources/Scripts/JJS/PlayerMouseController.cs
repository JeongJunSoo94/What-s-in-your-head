using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
public class PlayerMouseController : MonoBehaviour
{

    public GameObject[] Weapon;

    public bool ableToLeft;
    public bool ableToRight;
    private void Awake()
    {
        ableToLeft = false;
        ableToRight = false;
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
