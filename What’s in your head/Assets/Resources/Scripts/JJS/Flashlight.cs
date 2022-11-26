using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using JCW.Spawner;
using JCW.Object;
using JCW.AudioCtrl;
namespace JJS
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(PhotonView))]
    public class Flashlight : MonoBehaviour
    {
        GameObject lightObj;
        public Light spot;
        public Light directional;
        public Light point;
        public ConeFindTarget finder;
        public ConeFindTarget finderSpot;

        public bool sandSackUse;

        public Spawner_Photon spawner;
        public float attackTime;
        public int index;
        public GameObject posList;
        public List<Transform[]> targetList = new();
        public bool canAttack;

        AudioSource audioSource;

        PhotonView photonView;

        Animator nellaAnimator;
        Animator steadyAnimator;

        string death = "Death";
        private void Awake()
        {
            SetCharacterGameObject(gameObject, out lightObj, "Light");
            spot = lightObj.transform.GetChild(0).GetComponent<Light>();
            directional = lightObj.transform.GetChild(1).GetComponent<Light>();
            point = lightObj.transform.GetChild(2).GetComponent<Light>();
            photonView = GetComponent<PhotonView>();
            audioSource = GetComponent<AudioSource>();

            if (sandSackUse)
                InitializedSandSack();
        }

        void Update()
        {
            if (photonView.IsMine)
            {
                if (nellaAnimator == null || steadyAnimator == null)
                {
                    TargetAdd();
                }
                else
                {
                    SandAttackJudgment();
                }
            }
        }

        public void TargetAdd()
        {
            for (int i = 0; i < finder.targetObj.Count; i++)
            {
                if (finder.targetObj[i].CompareTag("Nella"))
                {
                    nellaAnimator = finder.targetObj[i].GetComponent<Animator>();
                }
                else if (finder.targetObj[i].CompareTag("Steady"))
                {
                    steadyAnimator = finder.targetObj[i].GetComponent<Animator>();
                }
            }
        }

        public void SandAttackJudgment()
        {
            if (sandSackUse)
            {
                if ((nellaAnimator.GetCurrentAnimatorStateInfo(0).IsName(death) || steadyAnimator.GetCurrentAnimatorStateInfo(0).IsName(death)) && canAttack)
                {
                    photonView.RPC(nameof(SandAttack), RpcTarget.AllViaServer);
                    canAttack = false;
                }
                else if (!nellaAnimator.GetCurrentAnimatorStateInfo(0).IsName(death) && !steadyAnimator.GetCurrentAnimatorStateInfo(0).IsName(death))
                {
                    canAttack = true;
                }
            }
        }

        public void LightEnable(bool enable)
        {
            lightObj.SetActive(enable);
        }

        public bool TargetCheck()
        {
            return finder.DiscoveryTargetBool() ? true : false;
        }

        public bool SpotTargetCheck()
        {
            return finderSpot.DiscoveryTargetBool() ? true : false;
        }

        public void PlaySound(string soundName)
        {
            SoundManager.Instance.PlayEffect_RPC(soundName);
        }

        public void StopSound()
        {
            SoundManager.Instance.StopEffect_RPC();
        }

        [PunRPC]
        public void SandAttack()
        {
            for (int i = 1; i < targetList[index].Length; i++)
            {
                GameObject bullet = spawner.Respawn(targetList[index][i].position);
                bullet.GetComponent<FallJJS>().Initialized();
                bullet.GetComponent<FallJJS>().StartCoroutineFall(attackTime);
            }
        }


        public void InitializedSandSack()
        {
            SetList(posList);
            canAttack = true;
        }

        public void SetList(GameObject objectList)
        {
            for (int i = 0; i < objectList.transform.childCount; i++)
            {
                Transform[] allChildren = objectList.transform.GetChild(i).GetComponentsInChildren<Transform>();
                targetList.Add(allChildren);
            }
        }

        public void SetLightColor(Color color)
        {
            spot.color = color;
            directional.color = color;
            point.color = color;
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
    }

}
