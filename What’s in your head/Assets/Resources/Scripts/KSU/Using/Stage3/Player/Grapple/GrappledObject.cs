using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.AutoAim.Object
{
    public class GrappledObject : AutoAimTargetObject
    {
        [SerializeField] GameObject detectingTrigger;
        public GameObject offset;

        // Start is called before the first frame update
        void Start()
        {
            detectingTrigger.transform.localScale = new Vector3(1, 1, 1) * (detectingUIRange * 2f);
            StartCoroutine(nameof(WaitForPlayer));
        }

        public Vector3 GetOffsetPosition()
        {
            return offset.transform.position;
        }

        protected IEnumerator WaitForPlayer()
        {
            yield return new WaitUntil(() => GameManager.Instance.characterOwner.Count>=2);

            if (GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient])
                transform.GetChild(2).gameObject.SetActive(false);

            yield break;
        }
    }
}
