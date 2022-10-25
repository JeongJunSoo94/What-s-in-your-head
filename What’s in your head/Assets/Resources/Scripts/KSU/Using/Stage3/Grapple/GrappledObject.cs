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
            //if (GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient])
            //    transform.GetChild(2).gameObject.SetActive(false);
        }

        // Start is called before the first frame update
        void Start()
        {
            detectingTrigger.transform.localScale = new Vector3(1, 1, 1) * (detectingRange * 2f);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public Vector3 GetOffsetPosition()
        {
            return offset.transform.position;
        }
    }
}
