using System.Collections;
using System.Collections.Generic;
using JCW.AudioCtrl;
using UnityEngine;


namespace JCW.Object
{
    [RequireComponent(typeof(AudioSource))]
    public class CymbalsButton : MonoBehaviour
    {
        [Header("��ư ���� �ӵ�")] [SerializeField] float pressedSpeed = 5f;
        [Header("��ư ������ �ӵ�")] [SerializeField] float releaseSpeed = 5f;
        [Header("��ư ������ �����ϴ� �ð�")] [SerializeField] float releaseTime = 7f;
        [Header("��� �� ��ư ���׸���")] [SerializeField] Material normalMat;
        [Header("�۵� �� ��ư ���׸���")] [SerializeField] Material activeMat;
        [Header("�������� ��ư ����")] [SerializeField] bool isPermanent;
        [Header("��ȣ�ۿ��� �� �ִ� ������Ʈ��")] [SerializeField] List<PlayObject> objectList;

        //bool isPressed;
        float pressed_Ratio = 1f;
        Animator anim;
        MeshRenderer meshRenderer;
        //bool isMatChanged = false;

        AudioSource audioSource;

        WaitForSeconds ws;

        private void Awake()
        {
            anim = transform.parent.parent.GetComponent<Animator>();
            meshRenderer = transform.GetChild(1).GetComponent<MeshRenderer>();
            meshRenderer.material = normalMat;
            audioSource = GetComponent<AudioSource>();
            AudioCtrl.AudioSettings.SetAudio(audioSource, 1f, 50f);
            ws = new(releaseTime);
        }
        /*
        void Update()
        {
            if (isPressed)
            {
                if (pressed_Ratio != 0f)
                {
                    // ��� ���� ���׸��� ����
                    if (!isMatChanged)
                    {
                        //SoundManager.Instance.Play3D_RPC("ButtonTurnOn", audioSource);
                        meshRenderer.material = activeMat;
                        isMatChanged = true;
                        Debug.Log("���׸��� ��ü");
                    }

                    // �ִϸ��̼� ���
                    pressed_Ratio = Mathf.Lerp(pressed_Ratio, 0, pressedSpeed * Time.deltaTime);
                    if (pressed_Ratio < 0.01f)
                    {
                        Debug.Log("1->0�� �Ǿ����Ƿ� �ٽ� ��ư �۵� ����");
                        pressed_Ratio = 0f;
                        for (int i = 0 ; i<objectList.Count ; ++i)
                        {
                            objectList[i].GetComponent<PlayObject>().enabled = true;
                            objectList[i].isActive = true;
                        }
                        if (isPermanent)
                            Destroy(this);
                    }
                    anim.SetFloat("pressed_ratio", pressed_Ratio);
                    anim.Play("Press Blend Tree");
                }
            }
            else if (!isPermanent && pressed_Ratio != 1f)
            {

                if (pressed_Ratio > 0.95f)
                {
                    isPressed = false;
                    Debug.Log("0->1�� �Ǿ����Ƿ� ��ư �۵�");
                    pressed_Ratio = 1f;
                    meshRenderer.material = normalMat;
                    isMatChanged = false;
                    for (int i = 0 ; i<objectList.Count ; ++i)
                    {
                        objectList[i].isActive = false;
                    }
                }
                pressed_Ratio = Mathf.Lerp(pressed_Ratio, 1, releaseSpeed * Time.deltaTime);
                anim.SetFloat("pressed_ratio", pressed_Ratio);
                anim.Play("Press Blend Tree");
            }
        }
        */
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Cymbals"))
                StartCoroutine("ActiveObj");
        }

        IEnumerator ActiveObj()
        {
            // ��� ���� ���׸��� ����
            StopCoroutine("DeactiveObj");
            meshRenderer.material = activeMat;

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
                Destroy(this);
            else
                StartCoroutine("DeactiveObj");
            yield break;
        }

        IEnumerator DeactiveObj()
        {
            yield return ws;
            while (pressed_Ratio <= 0.95f)
            {
                pressed_Ratio = Mathf.Lerp(pressed_Ratio, 1, releaseSpeed * Time.deltaTime);
                anim.SetFloat("pressed_ratio", pressed_Ratio);
                anim.Play("Press Blend Tree");
                yield return null;
            }

            pressed_Ratio = 1f;
            anim.SetFloat("pressed_ratio", pressed_Ratio);
            meshRenderer.material = normalMat;
            for (int i = 0 ; i<objectList.Count ; ++i)
            {
                objectList[i].isActive = false;
            }
            yield break;
        }
    }
}

