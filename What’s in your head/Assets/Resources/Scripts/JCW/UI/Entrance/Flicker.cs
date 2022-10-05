using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Flicker : MonoBehaviour
{
    [Tooltip("'버튼을 입력하시오' 문구의 깜빡이는 속도를 조절하는 변수입니다.")]
    [Header("깜빡이는 속도")] [Range(0.0f,2.0f)] [SerializeField] private float flickSecond = 0.5f;
    private Image img = null;
    void Start()
    {
        img = this.gameObject.GetComponent<Image>();
        StartCoroutine(nameof(Flick));
                
    }
    IEnumerator Flick()
    {
        while(true)
        {
            yield return new WaitForSeconds(flickSecond);
            img.enabled = !img.enabled;
        }
        
    }

}
