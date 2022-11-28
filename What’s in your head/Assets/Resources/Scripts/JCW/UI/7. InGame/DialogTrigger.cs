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
        [Header("몇 초 후 뜨게 할지")][SerializeField] [Range(0, 10f)] List<float> etcStartTime;
        [Header("안내 : 실행할 대사 총 개수 및 각각 남아있는 시간")] [SerializeField] List<float> etcRemainTime = new();
        [Space(10f)]
        [Header("실행할 넬라 대사 시작 인덱스 (1~N)")] [SerializeField] [Range(0,50)] int nellaOrder;
        [Header("각 대사 몇 초 후 뜨게 할지")] [SerializeField] [Range(0, 10f)] List<float> nellaStartTime;
        [Header("넬라 : 실행할 대사 총 개수 및 각각 남아있는 시간")] [SerializeField] List<float> nellaRemainTime = new();
        [Space(10f)]
        [Header("실행할 스테디 대사 시작 인덱스 (1~N)")] [SerializeField] [Range(0,50)] int steadyOrder;
        [Header("각 대사 몇 초 후 뜨게 할지")] [SerializeField] [Range(0, 10f)] List<float> steadyStartTime;
        [Header("스테디 : 실행할 대사 총 개수 및 각각 남아있는 시간")] [SerializeField] List<float> steadyRemainTime = new();

        [Header("넬라가 접촉 시 대사 발생")] [SerializeField] bool isNellaTrigger = true;
        [Header("스테디가 접촉 시 대사 발생")] [SerializeField] bool isSteadyTrigger = true;

        PhotonView pv;

        bool isStart = false;
        private void Awake()
        {
            pv = PhotonView.Get(this);

        }

        private void OnTriggerEnter(Collider other)
        {
            if((isNellaTrigger && other.CompareTag("Nella"))
                || (isSteadyTrigger && other.CompareTag("Steady")))
            {
                if(!GameManager.Instance.GetCharOnScene())
                    return;
                if (other.GetComponent<PlayerState>().isMine && !isStart)
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

            if (DialogManager.Instance.isEtcStart)
            {
                DialogManager.Instance.needToEtcBreak = true;
                yield return new WaitUntil(() => DialogManager.Instance.needToEtcBreak == false);
            }
            if (etcStartTime != null && etcStartTime.Count != 0)
                yield return new WaitForSeconds(etcStartTime[0]);
            DialogManager.Instance.isEtcStart = true;

            int i = 0;
            float curTime = 0f;

            DialogManager.Instance.SetEtcDialog(etcOrder);
            while (i < etcRemainTime.Count)
            {
                curTime += Time.deltaTime;
                if (curTime >= etcRemainTime[i])
                {
                    ++i;
                    curTime = 0f;
                    if (i < etcRemainTime.Count)
                    {
                        if (etcStartTime != null && etcStartTime.Count > i)
                        {
                            DialogManager.Instance.etcText1.gameObject.SetActive(false);
                            DialogManager.Instance.etcText2.gameObject.SetActive(false);
                            yield return new WaitForSeconds(etcStartTime[i]);
                        }
                        DialogManager.Instance.SetEtcDialog(etcOrder + i);
                    }
                }
                // 다른 곳에서 접근해서 브레이크 구문을 켰을 시, 현재 코루틴을 종료
                if (DialogManager.Instance.needToEtcBreak)
                {
                    DialogManager.Instance.needToEtcBreak = false;
                    yield break;
                }
                yield return null;
            }
            DialogManager.Instance.etcText1.gameObject.SetActive(false);
            DialogManager.Instance.etcText2.gameObject.SetActive(false);
            DialogManager.Instance.isEtcStart = false;

            yield break;
        }
        IEnumerator StartNellaDialog()
        {
            if (nellaOrder == 0)
                yield break;

            if (DialogManager.Instance.isNellaStart)
            {
                DialogManager.Instance.needToNellaBreak = true;
                yield return new WaitUntil(() => DialogManager.Instance.needToNellaBreak == false);
            }

            if(nellaStartTime != null && nellaStartTime.Count != 0)
                yield return new WaitForSeconds(nellaStartTime[0]);
            DialogManager.Instance.isNellaStart = true;

            int i = 0;
            float curTime = 0f;

            DialogManager.Instance.SetNellaDialog(nellaOrder);
            while (i < nellaRemainTime.Count)
            {                
                curTime += Time.deltaTime;
                if (curTime >= nellaRemainTime[i])
                {
                    ++i;
                    curTime = 0f;
                    if (i < nellaRemainTime.Count)
                    {
                        if (nellaStartTime != null && nellaStartTime.Count > i)
                        {
                            DialogManager.Instance.nellaText1.gameObject.SetActive(false);
                            DialogManager.Instance.nellaText2.gameObject.SetActive(false);
                            yield return new WaitForSeconds(nellaStartTime[i]);
                        }
                        DialogManager.Instance.SetNellaDialog(nellaOrder + i);
                    }
                }
                if (DialogManager.Instance.needToNellaBreak)
                {
                    DialogManager.Instance.needToNellaBreak = false;
                    yield break;
                }
                yield return null;
            }
            DialogManager.Instance.nellaText1.gameObject.SetActive(false);
            DialogManager.Instance.nellaText2.gameObject.SetActive(false);
            DialogManager.Instance.isNellaStart = false;


            yield break;
        }
        IEnumerator StartSteadyDialog()
        {
            if (steadyOrder == 0)
                yield break;

            if (DialogManager.Instance.isSteadyStart)
            {
                DialogManager.Instance.needToSteadyBreak = true;
                yield return new WaitUntil(() => DialogManager.Instance.needToSteadyBreak == false);
            }
            if (steadyStartTime != null && steadyStartTime.Count != 0)
                yield return new WaitForSeconds(steadyStartTime[0]);
            DialogManager.Instance.isSteadyStart = true;

            int i = 0;
            float curTime = 0f;

            DialogManager.Instance.SetSteadyDialog(steadyOrder);
            while (i < steadyRemainTime.Count)
            {
                curTime += Time.deltaTime;
                if(curTime >= steadyRemainTime[i])
                {
                    ++i;
                    curTime = 0f;
                    if (i < steadyRemainTime.Count)
                    {
                        if (steadyStartTime != null && steadyStartTime.Count > i)
                        {
                            DialogManager.Instance.steadyText1.gameObject.SetActive(false);
                            DialogManager.Instance.steadyText2.gameObject.SetActive(false);
                            yield return new WaitForSeconds(steadyStartTime[i]);
                        }
                        DialogManager.Instance.SetSteadyDialog(steadyOrder + i);
                    }
                }
                if (DialogManager.Instance.needToSteadyBreak)
                {
                    DialogManager.Instance.needToSteadyBreak = false;
                    yield break;
                }
                yield return null;
            }
            DialogManager.Instance.steadyText1.gameObject.SetActive(false);
            DialogManager.Instance.steadyText2.gameObject.SetActive(false);
            DialogManager.Instance.isSteadyStart = false;

            yield break;
        }
    }

}
