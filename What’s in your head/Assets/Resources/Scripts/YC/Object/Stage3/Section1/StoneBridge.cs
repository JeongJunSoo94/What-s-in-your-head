using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Current Issue       : �޽� �ݶ��̴� �̽�
/// 
/// Object Name         : ���ٸ�
/// 
/// Object Description  : �������� �ʰ� ���߿� �� �ִ� ���� ������Ʈ, �Ʒ����� ������
///                         �ʹݺο��� �Ĺݺη� ������ ������ ũ��� �۾�����. �߰��� ���̺� ����Ʈ ����
/// 
/// Player Intraction   :
/// 
/// </summary>

namespace YC_OBJ
{
    public class StoneBridge : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("Collision!");
        }
    }
}
