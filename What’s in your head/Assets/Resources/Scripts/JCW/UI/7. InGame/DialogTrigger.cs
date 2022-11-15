using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace JCW.Dialog
{
    [RequireComponent(typeof(PhotonView))]
    public class DialogTrigger : MonoBehaviour
    {
        [Header("=======재생 안할 시 0으로 세팅=======")]
        [Header("실행할 안내 대사 시작 인덱스 (1~N)")] [SerializeField] [Range(0,50)] int etcOrder;
        [Header("몇 초 후 뜨게 할지")][SerializeField] [Range(0, 5f)] float etcStartTime;
        [Header("안내 : 실행할 대사 총 개수 및 각각 남아있는 시간")] [SerializeField] List<float> etcRemainTime = new();
        [Space(10f)]
        [Header("실행할 넬라 대사 시작 인덱스 (1~N)")] [SerializeField] [Range(0,50)] int nellaOrder;
        [Header("몇 초 후 뜨게 할지")] [SerializeField] [Range(0, 5f)] float nellaStartTime;
        [Header("넬라 : 실행할 대사 총 개수 및 각각 남아있는 시간")] [SerializeField] List<float> nellaRemainTime = new();
        [Space(10f)]
        [Header("실행할 스테디 대사 시작 인덱스 (1~N)")] [SerializeField] [Range(0,50)] int steadyOrder;
        [Header("몇 초 후 뜨게 할지")] [SerializeField] [Range(0, 5f)] float steadyStartTime;
        [Header("스테디 : 실행할 대사 총 개수 및 각각 남아있는 시간")] [SerializeField] List<float> steadyRemainTime = new();


        PhotonView pv;


        //WaitForSeconds ws = null;
        WaitForSeconds etcDelayTime     = null;
        WaitForSeconds nellaDelayTime   = null;
        WaitForSeconds steadyDelayTime  = null;

        bool isStart = false;
        private void Awake()
        {
            pv = PhotonView.Get(this);
            etcDelayTime    = new(etcStartTime);
            nellaDelayTime  = new(nellaStartTime);
            steadyDelayTime = new(steadyStartTime);

        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Nella") || other.CompareTag("Steady"))
            {
                if(PhotonNetwork.PlayerList.Length < 2
                    || !DialogManager.Instance.isSet())
                    return;
                if (!isStart)
                {                    
                    isStart = true;
                    pv.RPC(nameof(StartDialog), RpcTarget.AllViaServer);
                }
            }
        }

        [PunRPC]
        public void StartDialog()
        {
            isStart = true;
            DialogManager.Instance.CleanSet();
            StartCoroutine(nameof(StartEtcDialog));
            StartCoroutine(nameof(StartNellaDialog));
            StartCoroutine(nameof(StartSteadyDialog));
        }

        IEnumerator StartEtcDialog()
        {
            if (etcOrder == 0)
                yield break;

            yield return etcDelayTime;
            
            for ( int i=0 ; i<etcRemainTime.Count ; ++i)
            {
                DialogManager.Instance.SetEtcDialog(etcOrder + i);
                yield return new WaitForSeconds(etcRemainTime[i]);
                DialogManager.Instance.etcText1.enabled = false;
                DialogManager.Instance.etcText2.enabled = false;
            }

            yield break;
        }
        IEnumerator StartNellaDialog()
        {
            if (nellaOrder == 0)
                yield break;

            yield return nellaDelayTime;
            for (int i = 0 ; i < nellaRemainTime.Count ; ++i)
            {
                DialogManager.Instance.SetNellaDialog(nellaOrder + i);
                yield return new WaitForSeconds(nellaRemainTime[i]);
                DialogManager.Instance.nellaText1.enabled = false;
                DialogManager.Instance.nellaText2.enabled = false;
            }

            yield break;
        }
        IEnumerator StartSteadyDialog()
        {
            if (steadyOrder == 0)
                yield break;

            yield return steadyDelayTime;
            for (int i = 0 ; i < steadyRemainTime.Count ; ++i)
            {
                DialogManager.Instance.SetSteadyDialog(steadyOrder + i);
                yield return new WaitForSeconds(steadyRemainTime[i]);
                DialogManager.Instance.steadyText1.enabled = false;
                DialogManager.Instance.steadyText2.enabled = false;
            }

            yield break;
        }
    }

}
