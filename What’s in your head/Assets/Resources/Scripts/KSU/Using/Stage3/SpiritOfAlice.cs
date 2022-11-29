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
                    Debug.Log("앨리스 HP : " + GameManager.Instance.aliceHP);
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
                    Debug.Log("앨리스 HP : " + GameManager.Instance.aliceHP);
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
            DialogManager.Instance.steadyText2.text = "앨리스가 공격받고 있어!";
            DialogManager.Instance.steadyRealText2.text = DialogManager.Instance.steadyText2.text;
            DialogManager.Instance.steadyText2.gameObject.SetActive(true);
            SoundManager.Instance.PlayEffect_RPC("steady_talk_10");
            yield return wu;
            DialogManager.Instance.steadyText2.gameObject.SetActive(false);
            yield break;
        }

        IEnumerator Warn10()
        {
            DialogManager.Instance.nellaText1.text = "스테디! 앨리스가 위태로워!";
            DialogManager.Instance.nellaRealText1.text = DialogManager.Instance.nellaText1.text;
            DialogManager.Instance.nellaText1.gameObject.SetActive(true);
            SoundManager.Instance.PlayEffect_RPC("nella_talk_10");
            yield return wu;
            DialogManager.Instance.nellaText1.gameObject.SetActive(false);
            yield break;
        }

        IEnumerator GameOver()
        {
            DialogManager.Instance.nellaText1.text = "아아..앨리스. 지켜주지 못해서 미안해.";
            DialogManager.Instance.nellaRealText1.text = DialogManager.Instance.nellaText1.text;
            DialogManager.Instance.nellaText1.gameObject.SetActive(true);
            SoundManager.Instance.PlayEffect_RPC("nella_gethit_2");
            yield return wu;
            DialogManager.Instance.nellaText1.gameObject.SetActive(false);
            DialogManager.Instance.steadyText2.text = "정말 면목이 없군.";
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
