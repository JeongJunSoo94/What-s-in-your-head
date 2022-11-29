using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.UI.InGame
{
    [RequireComponent(typeof(PhotonView))]
    public class RestartFinalStage : MonoBehaviour
    {

        WaitUntil wu;
        PhotonView pv;

        int playerCount = 0;

        private void Awake()
        {
            pv = GetComponent<PhotonView>();
            wu = new(() => PhotonNetwork.LevelLoadingProgress == 1);
            StartCoroutine(nameof(CheckBoth));
        }

        [PunRPC]
        public void IncreaseCount()
        {
            ++playerCount;
            if(playerCount >= 2 && PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(4 * (GameManager.Instance.curStageIndex - 1) + 1 + GameManager.Instance.curStageType);
            }
        }

        IEnumerator CheckBoth()
        {
            while(PhotonNetwork.LevelLoadingProgress != 1)
            {
                Debug.Log("현재 진행율 : " + PhotonNetwork.LevelLoadingProgress);
                yield return null;
            }
            //yield return wu;
            PhotonNetwork.LevelLoadingProgress = 0;
            pv.RPC(nameof(IncreaseCount), RpcTarget.AllViaServer);
            yield break;
        }



    }

}
