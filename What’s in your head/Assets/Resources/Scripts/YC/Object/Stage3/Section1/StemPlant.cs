using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// 
/// Current Issue       : ��ȹ ���� ���� 
/// 
/// Object Name         : �ٱ� �Ĺ�
/// 
/// Object Description  : �ڶ��� ������ �°� �ȴٸ� '���� Ȥ�� ����' ������Ʈ�� ������ �����´�.
/// 
/// Script Description  : �� ������Ʈ�� �ڽ� ������Ʈ�� ���� Ȥ�� ������ ���� �ִ�(Section_1������ ����, Section_2������ ������ ���� �ִ�.)
///                         OnTriggerEnter() ȣ��� �ڽ� ������Ʈ�� SetActive(true)���� ���̴�.
///                         
/// Player Intraction   : �ڶ�(���� ��ų)
/// 
/// </summary>

namespace YC_OBJ
{
    public class StemPlant : MonoBehaviour
    {
        string interactionObjTag = "NellaWater";

        GameObject Obj;

        void Awake()
        {
            Obj = transform.GetChild(1).gameObject;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(interactionObjTag))
            {
                Obj.SetActive(true);
            }
        }
    }
}
