using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JJS.Weapon;
using System;
namespace JJS
{
    [CreateAssetMenu(fileName = "MouseControllerWeaponData", menuName = "Scriptable Object/MouseControllerWeaponData", order = int.MaxValue)]
    public class MouseControllerWeaponData : ScriptableObject
    {
        [Serializable]
        public struct WeaponInfo
        {
            public GameObject weapon;
            public bool canAim;
            public bool canNoAimAttack;
            public bool canMoveAttack;
        }

        [Header("조준, 무조준 공격, 공격 중 이동")] public WeaponInfo[] weaponInfo;

    }

}
