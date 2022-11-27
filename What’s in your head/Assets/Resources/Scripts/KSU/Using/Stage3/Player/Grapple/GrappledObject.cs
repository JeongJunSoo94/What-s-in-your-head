using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.AutoAim.Object
{
    public class GrappledObject : AutoAimTargetObject
    {
        //[SerializeField] GameObject detectingTrigger;
        public GameObject offset;

        protected override void Awake()
        {
            base.Awake();
        }
        void Start()
        {
            StartCoroutine(nameof(WaitForPlayer));
        }

        public Vector3 GetOffsetPosition()
        {
            return offset.transform.position;
        }

        protected IEnumerator WaitForPlayer()
        {
            yield return new WaitUntil(() => GameManager.Instance.GetCharOnScene());

            if (GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient])
                transform.GetChild(2).gameObject.SetActive(false);

            yield break;
        }
    }
}
