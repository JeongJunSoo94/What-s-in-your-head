using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using JCW.Network;

namespace JCW.UI
{
    public class Flicker : MonoBehaviour
    {
        [Tooltip("'��ư�� �Է��Ͻÿ�' ������ �����̴� �ӵ��� �����ϴ� �����Դϴ�.")]
        [Header("�����̴� �ӵ�")] [Range(0.0f,2.0f)] [SerializeField] float flickSecond = 0.5f;
        [Header("���θ޴� ������Ʈ")] [SerializeField] GameObject mainMenu;

        Image flickImg;

        private void Awake()
        {
            flickImg = transform.GetChild(2).GetComponent<Image>();
            Cursor.lockState = CursorLockMode.None;
        }

        void Start()
        {
            StartCoroutine(nameof(Flick));
        }
        IEnumerator Flick()
        {
            float curTime = 0f;
            while (!Input.anyKeyDown)
            {
                curTime += Time.deltaTime;
                if (curTime >= flickSecond)
                {
                    curTime = 0f;
                    flickImg.enabled = !flickImg.enabled;
                }
                yield return null;
            }
            mainMenu.SetActive(true);
            PhotonManager.Instance.Connect();
            Destroy(this.gameObject);
        }
    }
}

