using KSU.AutoAim.Object.Monster;
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
                    if (GameManager.Instance.aliceHP < 0)
                    {
                        GameManager.Instance.aliceHP = 0;
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
                    if (GameManager.Instance.aliceHP < 0)
                    {
                        GameManager.Instance.aliceHP = 0;
                    }
                }
                break;
        }
    }
}
