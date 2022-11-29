using JCW.AudioCtrl;
using JCW.Dialog;
using KSU.AutoAim.Object.Monster;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritOfAlice : MonoBehaviour
{
    bool isStart20 = false;
    bool isStart10 = false;
    bool isGameOver = false;

    WaitForSeconds wu = new(1.5f);
    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "MonsterAttack":
                {
                    GameManager.Instance.aliceHP -= other.GetComponentInParent<DefenseMonster>().attackDamage;
                    Debug.Log("�ٸ��� HP : " + GameManager.Instance.aliceHP);
                    if (!isStart20 && GameManager.Instance.aliceHP <= 20)
                    {
                        isStart20 = true;
                        StartCoroutine(nameof(Warn20));                        
                    }
                    if (!isStart10 && GameManager.Instance.aliceHP <= 10)
                    {
                        isStart10 = true;
                        StartCoroutine(nameof(Warn10));
                    }
                    if (!isGameOver && GameManager.Instance.aliceHP <= 0)
                    {
                        isGameOver = true;
                        GameManager.Instance.aliceHP = 0;
                        StartCoroutine(nameof(GameOver));

                    }
                    //else
                    //{
                    //    GetComponent<Animator>().SetBool("AttackedTrigger", true);
                    //}
                }
                break;
            case "MonsterRush":
                {
                    GameManager.Instance.aliceHP -= other.GetComponentInParent<TrippleHeadSnake>().rushDamage;
                    Debug.Log("�ٸ��� HP : " + GameManager.Instance.aliceHP);
                    if (!isStart20 && GameManager.Instance.aliceHP <= 20)
                    {
                        isStart20 = true;
                        StartCoroutine(nameof(Warn20));
                    }
                    if (!isStart10 && GameManager.Instance.aliceHP <= 10)
                    {
                        isStart10 = true;
                        StartCoroutine(nameof(Warn10));
                    }
                    if (!isGameOver && GameManager.Instance.aliceHP <= 0)
                    {
                        isGameOver = true;
                        GameManager.Instance.aliceHP = 0;
                        StartCoroutine(nameof(GameOver));

                    }
                }
                break;
        }

        IEnumerator Warn20()
        {
            DialogManager.Instance.steadyText2.text = "�ٸ����� ���ݹް� �־�!";
            DialogManager.Instance.steadyRealText2.text = DialogManager.Instance.steadyText2.text;
            DialogManager.Instance.steadyText2.gameObject.SetActive(true);
            SoundManager.Instance.PlayEffect_RPC("steady_talk_10");
            yield return wu;
            DialogManager.Instance.steadyText2.gameObject.SetActive(false);
            yield break;
        }

        IEnumerator Warn10()
        {
            DialogManager.Instance.nellaText1.text = "���׵�! �ٸ����� ���·ο�!";
            DialogManager.Instance.nellaRealText1.text = DialogManager.Instance.nellaText1.text;
            DialogManager.Instance.nellaText1.gameObject.SetActive(true);
            SoundManager.Instance.PlayEffect_RPC("nella_talk_10");
            yield return wu;
            DialogManager.Instance.nellaText1.gameObject.SetActive(false);
            yield break;
        }

        IEnumerator GameOver()
        {
            DialogManager.Instance.nellaText1.text = "�ƾ�..�ٸ���. �������� ���ؼ� �̾���.";
            DialogManager.Instance.nellaRealText1.text = DialogManager.Instance.nellaText1.text;
            DialogManager.Instance.nellaText1.gameObject.SetActive(true);
            SoundManager.Instance.PlayEffect_RPC("nella_gethit_2");
            yield return wu;
            DialogManager.Instance.nellaText1.gameObject.SetActive(false);
            DialogManager.Instance.steadyText2.text = "���� ����� ����.";
            DialogManager.Instance.steadyRealText2.text = DialogManager.Instance.steadyText2.text;
            DialogManager.Instance.steadyText2.gameObject.SetActive(true);
            SoundManager.Instance.PlayEffect_RPC("steady_dead");
            yield return wu;
            DialogManager.Instance.nellaText1.gameObject.SetActive(false);
            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.LoadLevel(15);
            yield break;
        }
    }
}
