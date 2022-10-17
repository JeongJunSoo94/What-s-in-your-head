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

        //�����Ÿ� ������Ʈ
        GameObject flickObj;

        Image flickImg;
        private void Awake()
        {
            flickObj = transform.GetChild(2).gameObject;
        }

        void Start()
        {
            flickImg = flickObj.GetComponent<Image>();
            StartCoroutine(nameof(Flick));
        }

        private void Update()
        {
            if (Input.anyKeyDown)
            {
                mainMenu.SetActive(true);
                PhotonManager.Instance.Connect();
                Destroy(this.gameObject);
            }

        }

        IEnumerator Flick()
        {
            while (true)
            {
                yield return new WaitForSeconds(flickSecond);
                flickImg.enabled = !flickImg.enabled;
            }
        }
    }
}

