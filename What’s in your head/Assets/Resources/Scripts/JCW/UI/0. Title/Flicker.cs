using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using JCW.Network;

namespace JCW.UI
{
    public class Flicker : MonoBehaviour
    {
        [Tooltip("'버튼을 입력하시오' 문구의 깜빡이는 속도를 조절하는 변수입니다.")]
        [Header("깜빡이는 속도")] [Range(0.0f,2.0f)] [SerializeField] float flickSecond = 0.5f;
        [Header("메인메뉴 오브젝트")] [SerializeField] GameObject mainMenu;

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

