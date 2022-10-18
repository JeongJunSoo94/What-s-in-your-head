using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

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
    public class HealthUI : MonoBehaviour
    {
        [Header("�ִ� ü�� (�⺻�� : 12)")] [SerializeField] int maxHP = 12;
        [Header("N�� �� ȸ��")] [SerializeField] float healSecond;
        [Header("N�� �� ������ ����")] [SerializeField] float damageSecond;


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
            isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];
            if (photonView.IsMine)
            {
                charHpUI = isNella ? transform.GetChild(0).gameObject : transform.GetChild(1).gameObject;

                // ���� ���⼭ ����
                //GameManager.Instance.reviveAllPairs.Add(isNella, this);
                GameManager.Instance.AddReviveAllPair(isNella, this.gameObject);
                //Debug.Log("isNella : " + isNella + " / reviveAllPairs �߰�");
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
        }

        void Update()
        {
            if (!photonView.IsMine)
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
                photonView.RPC(nameof(SetCure), RpcTarget.AllViaServer, false);

                curHP = GameManager.Instance.curPlayerHP;
                // ��� ��
                if (curHP <= 0)
                {
                    GameManager.Instance.curPlayerHP = 0;

                    // ���� ĳ���Ͱ� �ڶ��� �ڶ��� ��������� false��, ���׵��� ���׵��� ��������� false�� �ٲ�.
                    GameManager.Instance.isAlive[isNella] = false;
                    curHP = 0;
                    damageList.Clear();
                    photonView.RPC(nameof(SetHpAmount), RpcTarget.AllViaServer, (int)HpState.DAMAGED, 1f);
                    photonView.RPC(nameof(SetHpAmount), RpcTarget.AllViaServer, (int)HpState.NORMAL, 1f);
                    //photonView.RPC(nameof(SetRevive), RpcTarget.AllViaServer, true);
                    GameManager.Instance.MediateRevive(true);
                }
                else
                {
                    damageList.Add(new HPandTime<int, float>(previousHP - curHP, 0f));
                    if (curHP <= 3)
                    {
                        photonView.RPC(nameof(SetBgImage), RpcTarget.AllViaServer, (int)BgState.DANGEROUS, true);
                        photonView.RPC(nameof(SetBgImage), RpcTarget.AllViaServer, (int)BgState.DAMAGED, false);
                    }
                    else
                        photonView.RPC(nameof(SetBgImage), RpcTarget.AllViaServer, (int)BgState.DAMAGED, true);
                }
                previousHP = curHP;
                photonView.RPC(nameof(SetHpAmount), RpcTarget.AllViaServer, (int)HpState.NORMAL, (float)curHP / (float)maxHP);
            }
            else if (curHP != maxHP && curHP > 0
                    && hpImages[(int)HpState.DAMAGED].fillAmount - hpImages[(int)HpState.NORMAL].fillAmount <= 0.001f)
            {
                healTime += Time.deltaTime;
                if (healTime >= healSecond)
                {
                    healTime = 0;
                    photonView.RPC(nameof(SetCure), RpcTarget.AllViaServer, true);
                }
            }

            for (int i = damageList.Count - 1 ; i >= 0 ; --i)
            {
                // �� ������ ���ø��� �ð� ����
                damageList[i].curTime += Time.deltaTime;

                if (damageList[i].curTime >= damageSecond)
                {
                    float temp = hpImages[(int)HpState.DAMAGED].fillAmount - damageList[i].hp / (float)maxHP;

                    photonView.RPC(nameof(SetHpAmount), RpcTarget.AllViaServer, HpState.DAMAGED, temp);
                    damageList.RemoveAt(i);
                    photonView.RPC(nameof(SetBgImage), RpcTarget.AllViaServer, BgState.DAMAGED, false);
                }
            }
        }

        public void SetRevive(bool value)
        {
            photonView.RPC(nameof(SetRevive_RPC), RpcTarget.AllViaServer, value);
        }

        [PunRPC]
        void SetRevive_RPC(bool value)
        {
            charHpUI.SetActive(!value);
            reviveUI.SetActive(value);
        }

        [PunRPC]
        void SetCure(bool isOn)
        {
            if (isOn)
                StartCoroutine(nameof(Cure));
            else
                StopCoroutine(nameof(Cure));
        }

        [PunRPC]
        void SetHpAmount(int curState, float amount)
        {
            hpImages[curState].fillAmount = amount;
        }
        [PunRPC]
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
            hpImages[(int)HpState.NORMAL].fillAmount = 1f;
            hpImages[(int)HpState.DAMAGED].fillAmount = 1f;
            hpImages[(int)HpState.HEAL].enabled = false;
            bgImages[(int)BgState.DANGEROUS].enabled = false;

            if(photonView.IsMine)
                GameManager.Instance.curPlayerHP = maxHP;
            curHP = maxHP;
            previousHP = maxHP;

            yield break;

        }
    }
}

