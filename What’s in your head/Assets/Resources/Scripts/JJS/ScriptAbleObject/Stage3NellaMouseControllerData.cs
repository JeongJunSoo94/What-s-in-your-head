using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JJS.Weapon;
using System;
namespace JJS
{
    [CreateAssetMenu(fileName = "Stage3NellaMouseData", menuName = "Scriptable Object/Stage3NellaMouseDataData", order = int.MaxValue)]
    public class Stage3NellaMouseControllerData : ScriptableObject
    {

        public List<Discovery3D> hitObjs;
        [Serializable]
        public struct WeaponInfo
        {
            public GameObject weapon;
            public bool canAim;
            public bool canNoAimAttack;
            public bool canMoveAttack;
        }

        public float layerWeight = 0;

        [Header("조준, 무조준 공격, 공격 중 이동")] public WeaponInfo[] weaponInfo;

        [HideInInspector] public bool afterDelayTime;
        [HideInInspector] public bool ableToRight;

        [HideInInspector] public bool clickLeft;
        [HideInInspector] public bool clickRight;

        public LayerMask mouseLayer;
        public int mouseRayDistance;

        protected Ray ray;
        protected RaycastHit hit;

        public float curCool = 0f;
        public float swapCool = 0.5f;

        public float curAimCool = 0f;
        public float AimCool = 0.5f;

        public bool canSwap;
        public bool canAim;
        public bool notRotatoin;
    }

}
