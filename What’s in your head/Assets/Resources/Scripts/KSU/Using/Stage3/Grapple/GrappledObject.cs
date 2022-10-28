using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU
{
    public class GrappledObject : MonoBehaviour
    {
        [SerializeField] GameObject detectingTrigger;
        public GameObject offset;

        public float detectingRange;

        private void Awake()
        {
            StartCoroutine(nameof(WaitForPlayer));
        }

        // Start is called before the first frame update
        void Start()
        {
            detectingTrigger.transform.localScale = new Vector3(1, 1, 1) * (detectingRange * 2f);
        }

        public Vector3 GetOffsetPosition()
        {
            return offset.transform.position;
        }

        protected IEnumerator WaitForPlayer()
        {
            while (GameManager.Instance.characterOwner.Count <= 1)
                yield return new WaitForSeconds(0.2f);

            if (GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient])
                transform.GetChild(2).gameObject.SetActive(false);

            yield break;
        }
    }
}
