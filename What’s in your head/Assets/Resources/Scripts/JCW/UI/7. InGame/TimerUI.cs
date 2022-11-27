using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace JCW.UI.InGame
{
    [RequireComponent(typeof(PhotonView))]
    public class TimerUI : MonoBehaviour, IPunObservable
    {
        [Header("타이머 시간 (단위:분)")] [SerializeField] [Range(0,9)] float time = 5f;
        [Header("포탈 오브젝트")] [SerializeField] GameObject portal;
        StringBuilder stringBuilder;
        Text textUI;

        int second = 0;
        int minute = 0;
        int convertTime = 0;

        float checkTime = 0;

        PhotonView pv;
        bool isEnd = false;
        bool isStart = false;

        private void Awake()
        {
            pv = GetComponent<PhotonView>();
            textUI = transform.GetChild(0).GetComponent<Text>();
            stringBuilder = new(15,15);
            convertTime = (int)(time * 60f);
            second = (int)(convertTime % 60f);
            minute = (int)(convertTime / 60f);

            if (minute < 10)
                stringBuilder.Append("0");
            stringBuilder.Append(minute.ToString());
            stringBuilder.Append(":");
            if (second < 10)
                stringBuilder.Append("0");
            stringBuilder.Append(second.ToString());
            textUI.text = stringBuilder.ToString();
            StartCoroutine(nameof(WaitForPlayers));
        }


        void Update()
        {
            if (!isStart || isEnd)
                return;

            if(pv.IsMine)
            {
                checkTime += Time.deltaTime;
                if (checkTime >= 1f)
                {
                    stringBuilder.Clear();
                    checkTime = 0f;
                    if (second <= 0)
                    {
                        minute -= 1;
                        second = 59;
                    }
                    else
                        second -= 1;

                    if (minute < 10)
                        stringBuilder.Append("0");
                    stringBuilder.Append(minute.ToString());
                    stringBuilder.Append(":");
                    if (second < 10)
                        stringBuilder.Append("0");
                    stringBuilder.Append(second.ToString());
                    textUI.text = stringBuilder.ToString();
                }
                if(minute == 0 && second == 0)
                {
                    isEnd = true;
                    pv.RPC(nameof(TurnOnPortal), RpcTarget.AllViaServer);
                }
            }
            else
            {
                stringBuilder.Clear();
                if (minute < 10)
                    stringBuilder.Append("0");
                stringBuilder.Append(minute.ToString());
                stringBuilder.Append(":");
                if (second < 10)
                    stringBuilder.Append("0");
                stringBuilder.Append(second.ToString());
                textUI.text = stringBuilder.ToString();
            }
            
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.IsWriting)
            {
                stream.SendNext(second);
                stream.SendNext(minute);
            }
            else
            {
                second = (int)stream.ReceiveNext();
                minute = (int)stream.ReceiveNext();
            }
        }

        [PunRPC]
        void TurnOnPortal()
        {
            isEnd = true;
            portal.SetActive(true);
        }

        IEnumerator WaitForPlayers()
        {
            yield return new WaitUntil(() => GameManager.Instance.GetCharOnScene());
            isStart = true;
            yield break;
        }


    }

}
