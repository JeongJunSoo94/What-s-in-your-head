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

        [Header("����, ������ ����, ���� �� �̵�")] public WeaponInfo[] weaponInfo;

    }

}
