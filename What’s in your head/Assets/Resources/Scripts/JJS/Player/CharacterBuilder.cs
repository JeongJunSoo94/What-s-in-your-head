using JCW.Network;
using JJS.Weapon;
using KSU;
using KSU.AutoAim.Player;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using YC.Camera_;
namespace JJS
{
    public class CharacterBuilder : MonoBehaviour
    {
        public int stage;
        [HideInInspector] public GameObject nella;
        [HideInInspector] public GameObject steady;
        public MouseControllerWeaponData nellaMouseControllerData;
        public MouseControllerWeaponData steadyMouseControllerData;
        public Transform start;
        public Vector3 intervalPos;

        bool funcStart = false;
        bool single;

        private void Awake()
        {
            single = GameManager.Instance.isTest;
        }


        private void Update()
        {
            if (GameManager.Instance.curStageType == 1 || GameManager.Instance.curStageType == 2)
            {
                if(!funcStart && PhotonNetwork.InRoom)
                {
                    Debug.Log("캐릭터 만들어주기");
                    funcStart = true;
                    PhotonManager.Instance.MakeCharacter(start.position, intervalPos);
                    StartCoroutine(nameof(WaitForPlayers));
                }                
            }
        }

        IEnumerator WaitForPlayers()
        {
            if(single)
                yield return new WaitUntil(() => GameManager.Instance.GetCharOnScene(true) || GameManager.Instance.GetCharOnScene(false));
            else
                yield return new WaitUntil(() => GameManager.Instance.GetCharOnScene(true) && GameManager.Instance.GetCharOnScene(false));
            GameObject nella = null;
            GameObject steady = null;
            if (GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient])
            {
                if(GameManager.Instance.myPlayerTF)
                    nella = GameManager.Instance.myPlayerTF.gameObject;
                if(GameManager.Instance.otherPlayerTF)
                    steady = GameManager.Instance.otherPlayerTF.gameObject;                
            }
            else
            {
                if (GameManager.Instance.myPlayerTF)
                    steady = GameManager.Instance.myPlayerTF.gameObject;
                if (GameManager.Instance.otherPlayerTF)
                    nella = GameManager.Instance.otherPlayerTF.gameObject;
            }

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

            yield break;
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
                    for (int i = 0; i < playerMouse.weaponInfo.Length; i++)
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
            Transform[] allChildrens = findObject.GetComponentsInChildren<Transform>();
            foreach (Transform childs in allChildrens)
            {
                if (childs.name == findName)
                {
                    discoverObject = childs.gameObject;
                    return;
                }
                else
                {
                    Transform findObj = childs.transform.Find(findName);
                    if (findObj != null)
                    {
                        discoverObject = findObj.gameObject;
                    }
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
                SetCharacterGameObject(player, out UIObject, "CurrentStageItem");
                UIObject.SetActive(true);
            }
        }
    }
}