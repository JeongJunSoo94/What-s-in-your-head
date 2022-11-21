using JCW.AudioCtrl;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JCW.Object.Stage1
{
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(AudioSource))]
    public class RotateMiniRail : MonoBehaviour
    {
        [Header("회전 속도")] [SerializeField] float speed = 10f;
        Transform tf;
        PhotonView pv;
        AudioSource audioSource;

        private void Awake()
        {
            tf = this.transform;
            pv = PhotonView.Get(this);
            audioSource = GetComponent<AudioSource>();
            JCW.AudioCtrl.AudioSettings.SetAudio(audioSource, 0.75f, 110f, true);                        
            StartCoroutine(nameof(WaitForPlayer));
        }

        IEnumerator WaitForPlayer()
        {
            yield return new WaitUntil(() => GameManager.Instance.GetCharOnScene(true) && GameManager.Instance.GetCharOnScene(false));
            pv.RPC(nameof(StartFunc), RpcTarget.AllViaServer);
            SoundManager.Instance.PlayIndirect3D_RPC("S1S2_BGM_CarParade", audioSource);

            yield break;
        }

        [PunRPC]
        void StartFunc()
        {
            StartCoroutine(nameof(RotateObj));            
        }

        IEnumerator RotateObj()
        {
            while (true)
            {
                tf.Rotate(Vector3.up, speed * -10 * Time.deltaTime, Space.World);
                yield return null;
            }
        }
    }

}
