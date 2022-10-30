using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// 
/// Current Issue       : 
/// 
/// Object Name         : ����
/// 
/// Object Description  : �̵��� ���� �ܼ� �ı� ������Ʈ, �÷��̾��� ��θ� ���θ��� ��ֹ��μ� ������ Ÿ�Ⱑ �Ұ��� �ϴ�("Ŀ�ٶ� ������ �浹ü�� �����")
///                         ���� ������Ʈ�� �Ʒ� ��ų�� �ǰ� ���ҽ�, �ı������ ����ϰ� �������. (�ٽ� ���� X)
/// 
/// Script Description  : ù��° �ڽ� ������Ʈ�� �̵��� �������� �ܼ� ���� �ݶ��̴�, �ι�° �ڽ� ������Ʈ�� ���� �÷��̾� ��ų�� ��ȣ�ۿ��ϴ� ���� ������Ʈ
///                        �ڽĿ�����Ʈ�� �浹�� üũ�� ��, �ش� ������Ʈ���� �浹 �޽����� ������. (ž �ݶ��̴��� �־)
///                         
/// Player Intraction   : ���׵�(������ ��ų), �ڶ�(��Ÿ)
/// 
/// </summary>

namespace YC_OBJ
{
    public class Bush : MonoBehaviour
    {
        public void Destroy_Cor()
        {
            Debug.Log("Destroy");
            StartCoroutine("DestroyAndEffect");
        }

        IEnumerator DestroyAndEffect()
        {
            yield return new WaitForSeconds(3f);
            Destroy(this.gameObject);
        }
    }
}
