using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Flicker : MonoBehaviour
{
    [Tooltip("'��ư�� �Է��Ͻÿ�' ������ �����̴� �ӵ��� �����ϴ� �����Դϴ�.")]
    [Header("�����̴� �ӵ�")] [Range(0.0f,2.0f)] [SerializeField] private float flickSecond = 0.5f;
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
