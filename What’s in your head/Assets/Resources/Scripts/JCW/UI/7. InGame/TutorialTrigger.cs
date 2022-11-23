using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.UI.InGame
{
    public class TutorialTrigger : MonoBehaviour
    {
        [Header("띄워줄 넬라 스프라이트")] [SerializeField] Sprite nellaSprite;
        [Header("띄워줄 스테디 스프라이트")] [SerializeField] Sprite steadySprite;
        [Header("넬라와 상호작용할 지")] [SerializeField] bool isNellaInteractable;
        [Header("스테디와 상호작용할 지")] [SerializeField] bool isSteadyInteractable;
        
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

