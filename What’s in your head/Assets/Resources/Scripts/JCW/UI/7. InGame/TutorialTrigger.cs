using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.UI.InGame
{
    public class TutorialTrigger : MonoBehaviour
    {
        [Header("����� �ڶ� ��������Ʈ")] [SerializeField] Sprite nellaSprite;
        [Header("����� ���׵� ��������Ʈ")] [SerializeField] Sprite steadySprite;
        [Header("�ڶ�� ��ȣ�ۿ��� ��")] [SerializeField] bool isNellaInteractable;
        [Header("���׵�� ��ȣ�ۿ��� ��")] [SerializeField] bool isSteadyInteractable;
        
        private void OnTriggerEnter(Collider other)
        {
            if(isNellaInteractable && other.CompareTag("Nella"))
            {
                if (other.GetComponent<PlayerState>().isMine)
                {
                    TutorialUI.Instance.ChangeSprite(true, nellaSprite);
                    TutorialUI.Instance.SetImage(true, true);
                }
            }
            if (isSteadyInteractable && other.CompareTag("Steady"))
            {
                if (other.GetComponent<PlayerState>().isMine)
                {
                    TutorialUI.Instance.ChangeSprite(false, steadySprite);
                    TutorialUI.Instance.SetImage(false, true);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (isNellaInteractable && other.CompareTag("Nella"))
            {
                if (other.GetComponent<PlayerState>().isMine)
                    TutorialUI.Instance.SetImage(true, false);
            }
            if (isSteadyInteractable && other.CompareTag("Steady"))
            {
                if (other.GetComponent<PlayerState>().isMine)
                    TutorialUI.Instance.SetImage(false, false);
            }
        }
    }
}

