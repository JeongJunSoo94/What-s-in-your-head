using KSU.AutoAim.Object.Monster;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritOfAlice : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "MonsterAttack":
                {
                    GameManager.Instance.aliceHP -= other.GetComponentInParent<DefenseMonster>().attackDamage;
                    Debug.Log("举府胶 HP : " + GameManager.Instance.aliceHP);
                    if (GameManager.Instance.aliceHP <= 0)
                    {
                        GameManager.Instance.aliceHP = 0;
                        if (PhotonNetwork.IsMasterClient)
                        {
                            PhotonNetwork.LoadLevel(15);
                            return;
                        }
                    }
                    else
                    {
                        GetComponent<Animator>().SetBool("AttackedTrigger", true);
                    }
                }
                break;
            case "MonsterRush":
                {
                    GameManager.Instance.aliceHP -= other.GetComponentInParent<TrippleHeadSnake>().rushDamage;
                    Debug.Log("举府胶 HP : " + GameManager.Instance.aliceHP);
                    if (GameManager.Instance.aliceHP <= 0)
                    {
                        GameManager.Instance.aliceHP = 0;
                        if (PhotonNetwork.IsMasterClient)
                        {
                            PhotonNetwork.LoadLevel(15);
                            return;
                        }
                    }
                }
                break;
        }
    }
}
