using JCW.UI.InGame;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YC.CameraManager_;


namespace JCW.UI.InGame.Indicator
{
    public class DetectPlayer : MonoBehaviour
    {
        [Header("콜라이더 범위")] [SerializeField] [Range(0, 100)] float range;
        [Header("OneIndicator UI")] [SerializeField] OneIndicator targetIndicator;
        Camera mainCam;
        PlayerState nellaState;

        private void Awake()
        {
            this.transform.localScale = new Vector3(range, range, range);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Nella"))
            {
                if(nellaState == null)
                    nellaState = other.GetComponent<PlayerState>();
                if (nellaState.isMine)
                {
                    if (mainCam == null)
                        mainCam = CameraManager.Instance.cameras[0];
                    targetIndicator.SetUI(true);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Nella"))
            {
                if (nellaState.isMine)
                    targetIndicator.SetUI(false);
            }
        }
    }

}
