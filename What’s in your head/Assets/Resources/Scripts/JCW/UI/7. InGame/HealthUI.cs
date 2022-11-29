using System.Collections;
using System.Collections.Generic;
using JCW.Network;
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
        [Header("�ִ� ü�� (�⺻�� : 12)")] [SerializeField] int maxHP = 12;
        [Header("N�� �� ȸ��")] [SerializeField] float healSecond;
        [Header("N�� �� ������ ����")] [SerializeField] float damageSecond;


        // ���� HP UI�� ���� �ϳ� �׾��� �� ���� UI
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

        WaitForSeconds ws_healBlink;
        bool isTopView;
        bool isSideView;
        bool enterDead = false;

        Animator myPlayerAnim;

        PlayerController pc;

        void Awake()
        {
            photonView = GetComponent<PhotonView>();
            ws_healBlink = new(0.1f);
            isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];
            pc = transform.parent.parent.GetComponent<PlayerController>();

            if (photonView.IsMine)
                GameManager.Instance.healthUIPairs.Add(isNella, this);
            else
                GameManager.Instance.healthUIPairs.Add(!isNella, this);
  

            charHpUI = transform.GetChild(0).gameObject;
            charHpUI.SetActive(true);
            reviveUI = transform.GetChild(1).gameObject;

            // ����, HP�� �̸� �Ҵ�
            for (int i = 0 ; i < (int)BgState.END ; ++i)
            {
                bgImages.Add(charHpUI.transform.GetChild(i).GetComponent<Image>());
                hpImages.Add(charHpUI.transform.GetChild((int)HpState.END).GetChild(i).GetComponent<Image>());
            }
            curHP = GameManager.Instance.curPlayerHP;
            previousHP = curHP;
            isTopView = GameManager.Instance.isTopView;
            isSideView = GameManager.Instance.isSideView;
            myPlayerAnim = GameManager.Instance.myPlayerTF.GetComponent<Animator>();
        }

        private void OnEnable()
        {
            pc.isOn_HP_UI = true;
        }

        private void OnDisable()
        {
            pc.isOn_HP_UI = false;
        }

        void Update()
        {
            if (!photonView.IsMine)
                return;

            if (Input.GetKeyDown(KeyCode.KeypadMinus))
                GameManager.Instance.curPlayerHP -= 4;

            // ���� HP�� ���� HP ���� �޶����� ��
            if (curHP != GameManager.Instance.curPlayerHP)
            {
                // ���� ���ϼ��� �ִ� ȸ�� �ڷ�ƾ ����.
                healTime = 0f;
                StopCoroutine("Cure");

                curHP = GameManager.Instance.curPlayerHP;
                // ��� ��
                if (curHP <= 0)
                {
                    // 3-1 �������� �϶�
                    if (GameManager.Instance.curStageType == 1)
                    {
                        myPlayerAnim.SetBool("isDead", false);
                        Dead();
                        return;
                    }
                    // ���� ĳ���Ͱ� �ڶ��� �ڶ��� ��������� false��, ���׵��� ���׵��� ��������� false�� �ٲ�.
                    if (!isSideView && (bool)GameManager.Instance.isAlive[isNella])
                    {
                        CheckDeadEnd(isNella);
                        photonView.RPC(nameof(CheckDeadEnd), RpcTarget.Others, isNella);
                        photonView.RPC(nameof(TurnOffUI), RpcTarget.AllViaServer);
                    }

                    Dead();

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
                    && ((int)hpImages[(int)HpState.DAMAGED].fillAmount == (int)hpImages[(int)HpState.NORMAL].fillAmount))
            {                
                SetBgImage((int)BgState.DAMAGED, false);
                healTime += Time.deltaTime;
                if (healTime >= healSecond)
                {
                    healTime = 0;
                    StartCoroutine("Cure");
                }
            }
            for (int i = damageList.Count - 1 ; i >= 0 ; --i)
            {
                // �� ������ ���ø��� �ð� ����
                damageList[i].curTime += Time.deltaTime;

                if (damageList[i].curTime >= damageSecond)
                {
                    SetHpAmount((int)HpState.DAMAGED, hpImages[(int)HpState.DAMAGED].fillAmount - damageList[i].hp / (float)maxHP);
                    damageList.RemoveAt(i);
                }
            }
        }

        [PunRPC]
        void CheckDeadEnd(bool isNella)
        {
            GameManager.Instance.SetAlive(isNella, false);
            if (!(bool)GameManager.Instance.isAlive[true] && !(bool)GameManager.Instance.isAlive[false])
                ReloadCurrentScene();
            else
                GameManager.Instance.MediateRevive_RPC(true);
        }

        [PunRPC]
        void ReloadCurrentScene()
        {
            GameManager.Instance.ResetDefault();
            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.LoadLevel(15);
        }

        [PunRPC]
        void TurnOffUI()
        {
            charHpUI.SetActive(false);
        }
        void Dead()
        {
            curHP = maxHP;
            damageList.Clear();
            SetHpAmount((int)HpState.DAMAGED, 1f);
            SetHpAmount((int)HpState.NORMAL, 1f);
            SetBgImage((int)BgState.DANGEROUS, false);
            SetBgImage((int)BgState.DAMAGED, false);
        }

        public void SetRevive_RPC(bool value)
        {
            photonView.RPC(nameof(SetRevive), RpcTarget.AllViaServer, value);
        }

        [PunRPC]
        public void SetRevive(bool value)
        {
            // ���⼭ HP UI�� �ٽ� ����.
            if (!value)
                charHpUI.SetActive(true);
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

            // ���� 2���ϱ�
            while (count < 3)
            {
                yield return ws_healBlink;
                ++count;
                hpImages[(int)HpState.HEAL].enabled = !hpImages[(int)HpState.HEAL].enabled;
            }


            SetHpAmount((int)HpState.NORMAL, 1f);
            SetHpAmount((int)HpState.DAMAGED, 1f);
            hpImages[(int)HpState.HEAL].enabled = false;
            SetBgImage((int)BgState.DANGEROUS, false);
            SetBgImage((int)BgState.DAMAGED, false);


            GameManager.Instance.curPlayerHP = maxHP;
            curHP = maxHP;
            previousHP = maxHP;

            yield break;

        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (!GameManager.Instance.GetCharOnScene() || hpImages.Count <= 2)
                return;
            // ������ ���
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

            }
            // �޴� ���
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
            }
        }
    }
}

