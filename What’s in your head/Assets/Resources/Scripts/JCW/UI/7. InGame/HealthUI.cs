using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using YC.CameraManager_;

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
        [Header("���� ���� �� ����� UI")] [SerializeField] SwapItem swapUI;


        // ĳ���Ϳ� ���� �޸� ����� UI
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
            if (GameManager.Instance.characterOwner.Count == 0)
                isNella = true;
            else
                isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];
            if (photonView.IsMine)
            {
                charHpUI = isNella ? transform.GetChild(0).gameObject : transform.GetChild(1).gameObject;
                GameManager.Instance.AddReviveAllPair(isNella, transform.parent.name);
            }
            else
                charHpUI = isNella ? transform.GetChild(1).gameObject : transform.GetChild(0).gameObject;
        

            charHpUI.SetActive(true);
            reviveUI = transform.GetChild(2).gameObject;

            // ����, HP�� �̸� �Ҵ�
            for (int i = 0 ; i < (int)BgState.END ; ++i)
            {
                bgImages.Add(charHpUI.transform.GetChild(i).GetComponent<Image>());
                hpImages.Add(charHpUI.transform.GetChild((int)HpState.END).GetChild(i).GetComponent<Image>());
            }
            curHP = GameManager.Instance.curPlayerHP;
            previousHP = curHP;

            swapUI.MoveSideUI(true);
        }

        private void OnDisable()
        {
            swapUI.MoveSideUI(false);
        }

        void Update()
        {
            if (!photonView.IsMine || !(bool)GameManager.Instance.isAlive[isNella])
                return;
            // �׽�Ʈ�� >>============================================================
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
                GameManager.Instance.curPlayerHP -= 4;
            // �׽�Ʈ�� <<============================================================

            // ���� HP�� ���� HP ���� �޶����� ��
            if (curHP != GameManager.Instance.curPlayerHP)
            {
                // ���� ���ϼ��� �ִ� ȸ�� �ڷ�ƾ ����.
                healTime = 0f;
                StopCoroutine(nameof(Cure));

                curHP = GameManager.Instance.curPlayerHP;
                // ��� ��
                if (curHP <= 0)
                {
                    GameManager.Instance.curPlayerHP = maxHP;

                    // ���� ĳ���Ͱ� �ڶ��� �ڶ��� ��������� false��, ���׵��� ���׵��� ��������� false�� �ٲ�.
                    GameManager.Instance.CheckAliveState(isNella, false);
                    if (isNella)
                        CameraManager.Instance.NellaDeadCam();
                    else
                        CameraManager.Instance.SteadyDeadCam();
                    curHP = maxHP;
                    damageList.Clear();
                    Dead();
                    photonView.RPC(nameof(TurnOffUI), RpcTarget.AllViaServer, isNella);
                    //photonView.RPC(nameof(SetRevive), RpcTarget.AllViaServer, true);
                    Debug.Log("����ִ� �÷��̾� üũ HashTable ���� ���� : " + GameManager.Instance.isAlive.Count);
                    Debug.Log("Nella ����ֳ� : " + GameManager.Instance.isAlive[true] + " / Steady ����ֳ� : " + GameManager.Instance.isAlive[false]);
                    if (!(bool)GameManager.Instance.isAlive[true] && !(bool)GameManager.Instance.isAlive[false])
                        GameManager.Instance.MediateRevive(false);
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
        void TurnOffUI(bool isNellaValue)
        {
            if (isNellaValue)
                transform.GetChild(0).gameObject.SetActive(false);
            else
                transform.GetChild(1).gameObject.SetActive(false);

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
            if(!value)
            {
                if (photonView.IsMine)
                {
                    if (isNella)
                        transform.GetChild(0).gameObject.SetActive(true);
                    else
                        transform.GetChild(1).gameObject.SetActive(true);
                }
                else
                {
                    if (isNella)
                        transform.GetChild(1).gameObject.SetActive(true);
                    else
                        transform.GetChild(0).gameObject.SetActive(true);
                }
            }            
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

