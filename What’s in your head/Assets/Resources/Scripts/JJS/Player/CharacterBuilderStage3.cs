using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JJS.Weapon;
using KSU;
namespace JJS
{ 
    public class CharacterBuilderStage3 : CharacterBuilder
    {
        public GameObject warterGunObj;
        public GameObject guitarObj;
        public GameObject warterGun;
        public GameObject ikLeft;
        public GameObject ikRight;

        public Stage3NellaMouseControllerData stage3NellaMouseControllerData;

        public NellaMouseController3 mouse;
        public GameObject hand;
        public GameObject warterObj;
        public GameObject leftTarget;
        private void Start()
        {
            SetCharacter();
        }
        public override void SetCharacterComponent()
        {
            if (!nella.GetComponent<IKController>())
            { 
                nella.AddComponent<IKController>();
            }
            SetCharacterGameObject(nella,out hand, "Hand_R");
            
            mouse = nella.AddComponent<NellaMouseController3>();

            GameObject instanceWarterGun = Instantiate(warterGun);
            instanceWarterGun.transform.parent = nella.transform;
            WaterGun warterCS = instanceWarterGun.GetComponent<WaterGun>();
            mouse.gun = warterCS;
            mouse.weaponInfo = new PlayerMouseController.WeaponInfo[2];
         
            warterObj = Instantiate(warterGunObj);
            warterObj.transform.parent = hand.transform;
            warterObj.transform.localPosition = warterGunObj.transform.position;
            warterObj.transform.localRotation = warterGunObj.transform.rotation;
            mouse.weaponInfo[0].weapon = warterObj;
            mouse.weaponInfo[0].canAim = true;
            mouse.weaponInfo[0].canMoveAttack = true;
            mouse.weaponInfo[0].canNoAimAttack = false;


            SetCharacterGameObject(warterObj, out leftTarget, "TargetLeftIK");
            mouse.ik = nella.GetComponent<IKController>();
            GameObject ikLeftInstance = Instantiate(ikLeft);
            ikLeftInstance.AddComponent<TransfomFollow>();
            ikLeftInstance.GetComponent<TransfomFollow>().target = leftTarget;
            ikLeftInstance.transform.parent = nella.transform;
            mouse.ik.leftHandFollowObj = ikLeftInstance.transform;
            mouse.ik.leftHandFollowObj.localPosition = ikLeft.transform.position;
            mouse.ik.leftHandFollowObj.localRotation = ikLeft.transform.rotation;

            GameObject ikRightInstance = Instantiate(ikRight);
            ikRightInstance.transform.parent = nella.transform;
            mouse.ik.rightHandFollowObj = ikRightInstance.transform;
            mouse.ik.rightHandFollowObj.localPosition = ikRight.transform.position;
            mouse.ik.rightHandFollowObj.localRotation = ikRight.transform.rotation;

            GameObject muzzle = new GameObject("WaterMuzzlePos");
            muzzle.transform.parent = nella.transform;
            mouse.ik.hitpos = muzzle.transform;

            GameObject muzzlePos;
            SetCharacterGameObject(warterObj, out muzzlePos, "muzzlePos");

            warterCS.targetIK = ikRightInstance;
            warterCS.gunDirection = muzzle;

            GameObject hitPos = new GameObject("WaterHitPos");
            warterCS.layerMask = 1;
            hitPos.transform.parent = nella.transform;
            warterCS.hitPos = hitPos;
            warterCS.weapon = ikRightInstance;
            warterCS.startPos = muzzlePos;
            mouse.wartergunInit();

            GameObject guitar = Instantiate(guitarObj);
            guitar.transform.parent = hand.transform;
            guitar.transform.localPosition = guitarObj.transform.position;
            guitar.transform.localRotation = guitarObj.transform.rotation;
            mouse.weaponInfo[1].weapon = guitar;
            mouse.weaponInfo[1].canAim = false;
            mouse.weaponInfo[1].canMoveAttack = false;
            mouse.weaponInfo[1].canNoAimAttack = true;

            nella.GetComponent<PlayerController>().playerMouse = mouse;
        }
        

        public override void SetCharacter()
        {
            FindCharacter();
            
            SetCharacterComponent();
        }
    }
}
