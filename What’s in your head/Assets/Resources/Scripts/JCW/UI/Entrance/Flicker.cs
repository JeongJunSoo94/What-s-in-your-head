using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using JCW.Network;

namespace JCW.UI
{
    public class Flicker : MonoBehaviour
    {
        [Tooltip("'��ư�� �Է��Ͻÿ�' ������ �����̴� �ӵ��� �����ϴ� �����Դϴ�.")]
        [Header("�����̴� �ӵ�")] [Range(0.0f,2.0f)] [SerializeField] private float flickSecond = 0.5f;
        [Header("�����Ÿ� ������Ʈ")] [SerializeField] private GameObject flickObj = null;
        [Header("���θ޴� ������Ʈ")] [SerializeField] private GameObject mainMenu = null;
        private Image img = null;
        void Start()
        {
            img = flickObj.GetComponent<Image>();
            StartCoroutine(nameof(Flick));
        }

        private void Update()
        {
            if (Input.anyKeyDown)
            {
                mainMenu.SetActive(true);
                PhotonManager.instance.Connect();
                Destroy(this.gameObject);
            }

        }

        IEnumerator Flick()
        {
            while (true)
            {
                yield return new WaitForSeconds(flickSecond);
                img.enabled = !img.enabled;
            }

        }

    }
}

