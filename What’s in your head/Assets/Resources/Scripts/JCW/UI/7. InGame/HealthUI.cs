using System.Collections;
using System.Collections.Generic;
using KSU;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using YC.CameraManager_;
using YC.Photon;

namespace JCW.UI.InGame
{
    public enum BgState { NORMAL, DAMAGED, DANGEROUS, END }
    public enum HpState { DAMAGED, NORMAL, HEAL, END }

    public class HPandTime<HP, Time>
    {
        public HP hp;
        public Time curTime;
        public HPandTime(HP _hp, Time _time) { hp = _hp; curTime = _time; }
    }
    [RequireComponent(typeof(PhotonView))]
    public class HealthUI : MonoBehaviour, IPunObservable
    {
        [Header("최대 체력 (기본값 : 12)")] [SerializeField] int maxHP = 12;
        [Header("N초 후 회복")] [SerializeField] float healSecond;
        [Header("N초 후 게이지 깎임")] [SerializeField] float damageSecond;


        // 각각 HP UI와 누구 하나 죽었을 때 나올 UI
        GameObject charHpUI;
        GameObject reviveUI;
        readonly List<Image> bgImages = new();
        readonly List<Image> hpImages = new();

        float healTime;

        int previousHP;
        int curHP;

        bool isNella;

        PhotonView photonView;
        readonly List<HPandTime<int, float>> damageList = new();

        void Awake()
        {
            photonView = GetComponent<PhotonView>();

            isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];

            if (photonView.IsMine)
                GameManager.Instance.reviveAllPairs.Add(isNella, this);
            else
                GameManager.Instance.reviveAllPairs.Add(!isNella, this);
  

            charHpUI = transform.GetChild(0).gameObject;
            charHpUI.SetActive(true);
            reviveUI = transform.GetChild(1).gameObject;

            // 배경과, HP를 미리 할당
            for (int i = 0 ; i < (int)BgState.END ; ++i)
            {
                bgImages.Add(charHpUI.transform.GetChild(i).GetComponent<Image>());
                hpImages.Add(charHpUI.transform.GetChild((int)HpState.END).GetChild(i).GetComponent<Image>());
            }
            curHP = GameManager.Instance.curPlayerHP;
            previousHP = curHP;
        }

        private void OnEnable()
        {
            transform.parent.parent.GetComponent<PlayerController>().isOn_HP_UI = true;
        }

        private void OnDisable()
        {
            transform.parent.parent.GetComponent<PlayerController>().isOn_HP_UI = false;
        }

        void Update()
        {
            if (!photonView.IsMine || !(bool)GameManager.Instance.isAlive[isNella])
                return;
            // 테스트용 >>============================================================
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
                GameManager.Instance.curPlayerHP -= 4;
            // 테스트용 <<============================================================

            // 기존 HP와 현재 HP 값이 달라졌을 때
            if (curHP != GameManager.Instance.curPlayerHP)
            {
                // 실행 중일수도 있는 회복 코루틴 중지.
                healTime = 0f;
                StopCoroutine(nameof(Cure));

                curHP = GameManager.Instance.curPlayerHP;
                // 사망 시
                if (curHP <= 0)
                {
                    // 현재 캐릭터가 넬라라면 넬라의 살아있음을 false로, 스테디라면 스테디의 살아있음을 false로 바꿈.
                    GameManager.Instance.SetAliveState(isNella, false);
                    if(!GameManager.Instance.isTopView)
                        CameraManager.Instance.DeadCam(isNella);
                    
                    curHP = maxHP;
                    damageList.Clear();
                    Dead();
                    photonView.RPC(nameof(TurnOffUI), RpcTarget.AllViaServer);

                    // 넬라 & 스테디 둘 다 죽었으면
                    if (!(bool)GameManager.Instance.isAlive[true] && !(bool)GameManager.Instance.isAlive[false])
                    {
                        GameManager.Instance.MediateRevive(false);
                        GameManager.Instance.SetAliveState(isNella, true);
                        GameManager.Instance.SetAliveState(!isNella, true);
                        if (!GameManager.Instance.isTopView && !GameManager.Instance.isSideView)
                        {
                            CameraManager.Instance.ReviveCam(isNella);
                            CameraManager.Instance.ReviveCam(!isNella);
                        }                            
                    }                        
                    else
                        GameManager.Instance.MediateRevive(true);
                }
                else
                {
                    damageList.Add(new HPandTime<int, float>(previousHP - curHP, 0f));
                    if (curHP <= 3)
                    {
                        SetBgImage((int)BgState.DANGEROUS, true);
                        SetBgImage((int)BgState.DAMAGED, false);
                    }
                    else
                        SetBgImage((int)BgState.DAMAGED, true);
                }
                previousHP = curHP;
                SetHpAmount((int)HpState.NORMAL, (float)curHP / (float)maxHP);
            }

            else if (curHP != maxHP && curHP > 0
                    && (hpImages[(int)HpState.DAMAGED].fillAmount == hpImages[(int)HpState.NORMAL].fillAmount))
            {
                SetBgImage((int)BgState.DAMAGED, false);
                healTime += Time.deltaTime;
                if (healTime >= healSecond)
                {
                    healTime = 0;
                    StartCoroutine(nameof(Cure));
                }
            }
            for (int i = damageList.Count - 1 ; i >= 0 ; --i)
            {
                // 각 데미지 스택마다 시간 증가
                damageList[i].curTime += Time.deltaTime;

                if (damageList[i].curTime >= damageSecond)
                {
                    SetHpAmount((int)HpState.DAMAGED, hpImages[(int)HpState.DAMAGED].fillAmount - damageList[i].hp / (float)maxHP);
                    damageList.RemoveAt(i);
                }
            }
        }

        [PunRPC]
        void TurnOffUI()
        {
            charHpUI.SetActive(false);
        }
        void Dead()
        {
            SetHpAmount((int)HpState.DAMAGED, 1f);
            SetHpAmount((int)HpState.NORMAL, 1f);
            SetBgImage((int)BgState.DANGEROUS, false);
            SetBgImage((int)BgState.DAMAGED, false);
        }

        public void SetRevive(bool value)
        {
            photonView.RPC(nameof(SetRevive_RPC), RpcTarget.AllViaServer, value);
        }

        [PunRPC]
        void SetRevive_RPC(bool value)
        {
            if (!value)
            {
                //Debug.Log("HP 만땅 채우기");
                charHpUI.SetActive(true);
                //GameManager.Instance.curPlayerHP = maxHP;
            }
            if(GameManager.Instance.isTopView)
                reviveUI.SetActive(value);
        }

        void SetHpAmount(int curState, float amount)
        {
            hpImages[curState].fillAmount = amount;
        }

        void SetBgImage(int curState, bool isOn)
        {
            bgImages[curState].enabled = isOn;
        }

        IEnumerator Cure()
        {
            int count = 0;

            // 깜빡 2번하기
            while (count < 3)
            {
                yield return new WaitForSeconds(0.1f);
                ++count;
                hpImages[(int)HpState.HEAL].enabled = !hpImages[(int)HpState.HEAL].enabled;
            }


            SetHpAmount((int)HpState.NORMAL, 1f);
            SetHpAmount((int)HpState.DAMAGED, 1f);
            hpImages[(int)HpState.HEAL].enabled = false;
            SetBgImage((int)BgState.DANGEROUS, false);

            GameManager.Instance.curPlayerHP = maxHP;
            curHP = maxHP;
            previousHP = maxHP;

            yield break;

        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // 보내는 사람
            if (stream.IsWriting)
            {
                stream.SendNext(curHP);
                stream.SendNext(previousHP);
                stream.SendNext(hpImages[(int)HpState.NORMAL].fillAmount);
                stream.SendNext(hpImages[(int)HpState.DAMAGED].fillAmount);
                stream.SendNext(hpImages[(int)HpState.HEAL].enabled);

                stream.SendNext(bgImages[(int)BgState.NORMAL].enabled);
                stream.SendNext(bgImages[(int)BgState.DAMAGED].enabled);
                stream.SendNext(bgImages[(int)BgState.DANGEROUS].enabled);

                //stream.SendNext(charHpUI.activeSelf);
                //stream.SendNext(reviveUI.activeSelf);

            }
            // 받는 사람
            else
            {
                curHP                                       = (int)stream.ReceiveNext();
                previousHP                                  = (int)stream.ReceiveNext();
                hpImages[(int)HpState.NORMAL].fillAmount    = (float)stream.ReceiveNext();

                hpImages[(int)HpState.DAMAGED].fillAmount   = (float)stream.ReceiveNext();
                hpImages[(int)HpState.HEAL].enabled         = (bool)stream.ReceiveNext();

                bgImages[(int)BgState.NORMAL].enabled       = (bool)stream.ReceiveNext();
                bgImages[(int)BgState.DAMAGED].enabled      = (bool)stream.ReceiveNext();
                bgImages[(int)BgState.DANGEROUS].enabled    = (bool)stream.ReceiveNext();

                //charHpUI.SetActive((bool)stream.ReceiveNext());
                //reviveUI.SetActive((bool)stream.ReceiveNext());
            }
        }
    }
}

