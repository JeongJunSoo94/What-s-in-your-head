using JJS.Weapon;
using KSU;
using KSU.AutoAim.Player;
using UnityEngine;
using YC.Camera_;
namespace JJS
{
    public class CharacterBuilder : MonoBehaviour
    {
        public bool single;
        public int stage;
        [HideInInspector] public GameObject nella;
        [HideInInspector] public GameObject steady;
        public MouseControllerWeaponData nellaMouseControllerData;
        public MouseControllerWeaponData steadyMouseControllerData;
        public Transform start;
        public Vector3 intervalPos;
        private void Update()
        {
            if (single)
            {
                if (nella == null && steady == null)
                {
                    FindCharacter();
                }
                if (nella != null || steady != null)
                {
                    SetCharacterComponent(nella, nellaMouseControllerData, "Hand_R");
                    SetCharacterComponent(steady, steadyMouseControllerData, "Hand_R");
                    NellaScriptSetActive(nella);
                    SteadyScriptSetActive(steady);
                    SetStartPos();
                    if (stage == 3)
                    {
                        SetUION(nella);
                        SetUION(steady);
                    }
                    gameObject.SetActive(false);
                }
            }
            else
            {
                if (nella == null || steady == null)
                {
                    FindCharacter();
                }
                if (nella != null && steady != null)
                {
                    SetCharacterComponent(nella, nellaMouseControllerData, "Hand_R");
                    SetCharacterComponent(steady, steadyMouseControllerData, "Hand_R");
                    NellaScriptSetActive(nella);
                    SteadyScriptSetActive(steady);
                    SetStartPos();
                    if (stage == 2)
                    {
                        GameManager.Instance.isSideView = true;
                    }
                    else if (stage == 3)
                    {
                        SetUION(nella);
                        SetUION(steady);
                    }
                    gameObject.SetActive(false);
                }
            }
        }

        public void FindCharacter()
        {
            nella = GameObject.FindWithTag("Nella");
            steady = GameObject.FindWithTag("Steady");
        }

        public void SetStartPos()
        {
            Vector3 pos = start.position;
            if (nella != null)
                nella.transform.position = pos - intervalPos;
            if (steady != null)
                steady.transform.position = pos + intervalPos;
        }

        public virtual void SetCharacterComponent(GameObject player, MouseControllerWeaponData data, string findWeaponPath)
        {
            if (player != null)
            {
                player.GetComponent<CameraController>().InitSceneChange();

                PlayerMouseController playerMouse = player.GetComponent<PlayerMouseController>();
                if (playerMouse.GetUseWeapon() != -1)
                    playerMouse.weaponInfo[playerMouse.GetUseWeapon()].weapon.SetActive(false);

                if (data == null)
                {
                    playerMouse.weaponInfo = new PlayerMouseController.WeaponInfo[0];
                    player.GetComponent<PlayerController>().playerMouse = null;
                }
                else
                {
                    int weaponCount = data.weaponInfo.Length;
                    GameObject weaponStorage;
                    SetCharacterGameObject(player, out weaponStorage, findWeaponPath);

                    playerMouse.weaponInfo = new PlayerMouseController.WeaponInfo[weaponCount];
                    for (int i = 0 ; i < playerMouse.weaponInfo.Length ; i++)
                    {
                        GameObject cloneObj;
                        SetWeaponGameObject(weaponStorage, out cloneObj, data.weaponInfo[i].weapon.name);
                        if (cloneObj == null)
                        {
                            cloneObj = CreateWeapon(data.weaponInfo[i].weapon, weaponStorage.transform);
                        }

                        playerMouse.weaponInfo[i].weapon = cloneObj;
                        playerMouse.weaponInfo[i].canAim = data.weaponInfo[i].canAim;
                        playerMouse.weaponInfo[i].canMoveAttack = data.weaponInfo[i].canMoveAttack;
                        playerMouse.weaponInfo[i].canNoAimAttack = data.weaponInfo[i].canNoAimAttack;
                    }
                    if (weaponCount != 0)
                    {
                        playerMouse.weaponInfo[0].weapon.SetActive(true);
                    }
                    player.GetComponent<PlayerController>().playerMouse = playerMouse;
                }
            }
        }

        public void SetCharacterGameObject(GameObject findObject, out GameObject discoverObject, string findName)
        {
            discoverObject = null;
            Transform[] allChildren = findObject.GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                if (child.name == findName)
                {
                    discoverObject = child.gameObject;
                    return;
                }
            }
        }

        public void SetWeaponGameObject(GameObject findObject, out GameObject discoverObject, string findName)
        {
            discoverObject = null;
            Transform findObj = findObject.transform.Find(findName);
            if (findObj != null)
            {
                discoverObject = findObj.gameObject;
            }
        }

        public GameObject CreateWeapon(GameObject weapon, Transform parent)
        {
            GameObject clone = Instantiate(weapon);
            clone.name = weapon.name;
            clone.transform.parent = parent;
            clone.transform.localPosition = weapon.transform.position;
            clone.transform.localRotation = weapon.transform.rotation;
            return clone;
        }
        public void NellaScriptSetActive(GameObject player)
        {
            if (player != null)
            {
                PlayerMouseController playerMouse = player.GetComponent<PlayerMouseController>();
                if (playerMouse.weaponInfo.Length != 0)
                {
                    if (playerMouse.weaponInfo[playerMouse.GetUseWeapon()].weapon.name == "WaterPistol")
                    {
                        player.GetComponent<WaterGun>().InitSpawner();
                    }
                }
            }
        }
        public void SteadyScriptSetActive(GameObject player)
        {
            if (player != null)
            {
                PlayerMouseController playerMouse = player.GetComponent<PlayerMouseController>();

                if (playerMouse.weaponInfo.Length != 0)
                {
                    if (playerMouse.weaponInfo[playerMouse.GetUseWeapon()].weapon.name == "CymbalsPosition")
                    {
                        player.GetComponent<SteadyCymbalsAction>().enabled = true;
                        player.GetComponent<SteadyGrappleAction>().enabled = false;
                    }
                    else if (playerMouse.weaponInfo[playerMouse.GetUseWeapon()].weapon.name == "GrapplePosition")
                    {
                        player.GetComponent<SteadyCymbalsAction>().enabled = false;
                        player.GetComponent<SteadyGrappleAction>().enabled = true;
                    }
                }
                else
                {
                    player.GetComponent<SteadyCymbalsAction>().enabled = false;
                    player.GetComponent<SteadyGrappleAction>().enabled = false;
                }
                if (playerMouse is SteadyMouseController)
                {
                    SteadyMouseController steadyMouse = playerMouse as SteadyMouseController;
                    steadyMouse.SetAimWeapon();
                }
            }
        }

        public void SetUION(GameObject player)
        {
            GameObject UIObject;
            if (player != null)
            {
                SetWeaponGameObject(player,out UIObject, "CurrentStageItem");
                UIObject.SetActive(true);
            }
        }
    }
}