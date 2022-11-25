using System.Collections;
using System.Collections.Generic;
using JCW.AudioCtrl;
using KSU.AutoAim.Object;
using Photon.Pun;
using UnityEngine;


namespace JCW.Object
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(PhotonView))]
    public class CymbalsButton : AutoAimTargetObject
    {
        [Header("��ư ���� �ӵ�")] [SerializeField] float pressedSpeed = 5f;
        [Header("��ư ������ �ӵ�")] [SerializeField] float releaseSpeed = 5f;
        [Header("��ư ������ �����ϴ� �ð�")] [SerializeField] float releaseTime = 1f;
        [Header("�ٲ� ���׸����� �޸� ��ư")] [SerializeField] MeshRenderer meshRenderer;
        [Header("��� �� ��ư ���׸���")] [SerializeField] Material normalMat;
        [Header("�۵� �� ��ư ���׸���")] [SerializeField] Material activeMat;
        [Header("�������� ��ư ����")] [SerializeField] bool isPermanent;
        [Header("��ȣ�ۿ��� �� �ִ� ������Ʈ��")] [SerializeField] List<PlayObject> objectList;

        //bool isPressed;
        float pressed_Ratio = 1f;
        Animator anim;
        //MeshRenderer meshRenderer;
        //bool isMatChanged = false;

        AudioSource audioSource;

        WaitForSeconds ws;

        PhotonView pv;
        

        protected override void Awake()
        {
            base.Awake();
            pv = GetComponent<PhotonView>();
            anim = transform.parent.parent.parent.GetComponent<Animator>();
            //meshRenderer = transform.GetChild(1).GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = normalMat;
            TryGetComponent(out audioSource);
            if(SoundManager.Instance != null)
                SoundManager.Set3DAudio(pv.ViewID, audioSource, 1f, 50f);
            ws = new(releaseTime);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Cymbals"))
                StartCoroutine("ActiveObj");
        }

        IEnumerator ActiveObj()
        {
            // ��� ���� ���׸��� ����
            StopCoroutine("DeactiveObj");
            meshRenderer.sharedMaterial = activeMat;

            for (int i = 0 ; i<objectList.Count ; ++i)
            {
                objectList[i].GetComponent<PlayObject>().enabled = true;
                objectList[i].isActive = true;
            }

            while (pressed_Ratio >= 0.01f)
            {
                pressed_Ratio = Mathf.Lerp(pressed_Ratio, 0, pressedSpeed * Time.deltaTime);
                anim.SetFloat("pressed_ratio", pressed_Ratio);
                anim.Play("Press Blend Tree");
                yield return null;
            }
            pressed_Ratio = 0f;
            anim.SetFloat("pressed_ratio", pressed_Ratio);

            if (isPermanent)
                this.enabled = false;
            else
                StartCoroutine("DeactiveObj");
            yield break;
        }

        IEnumerator DeactiveObj()
        {
            while (pressed_Ratio <= 0.95f)
            {
                pressed_Ratio = Mathf.Lerp(pressed_Ratio, 1, releaseSpeed * Time.deltaTime);
                anim.SetFloat("pressed_ratio", pressed_Ratio);
                anim.Play("Press Blend Tree");
                yield return null;
            }

            pressed_Ratio = 1f;
            anim.SetFloat("pressed_ratio", pressed_Ratio);
            meshRenderer.sharedMaterial = normalMat;
            for (int i = 0 ; i<objectList.Count ; ++i)
            {
                objectList[i].isActive = false;
            }
            yield break;
        }
    }
}

