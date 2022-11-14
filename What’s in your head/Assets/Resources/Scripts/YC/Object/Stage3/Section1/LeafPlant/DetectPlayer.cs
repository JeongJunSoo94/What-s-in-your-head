using JCW.UI.InGame;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YC.CameraManager_;


namespace JCW.UI.InGame.Indicator
{
    [RequireComponent(typeof(PhotonView))]
    public class DetectPlayer : MonoBehaviour
    {
        [Header("콜라이더 범위")] [SerializeField] [Range(0, 100)] float range;
        PhotonView photonView;
        [Header("OneIndicator UI")] [SerializeField] OneIndicator targetIndicator;
        Camera mainCam;

        private void Awake()
        {
            this.transform.localScale = new Vector3(range, range, range);
            photonView = GetComponent<PhotonView>();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Nella"))
            {
                if (photonView.IsMine)
                {
                    if (mainCam == null)
                        mainCam = CameraManager.Instance.cameras[0];
                    targetIndicator.SetUI(true);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Nella"))
            {
                if (photonView.IsMine)
                    targetIndicator.SetUI(false);
            }
        }
    }

}
