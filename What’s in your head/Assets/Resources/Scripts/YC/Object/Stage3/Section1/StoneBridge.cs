using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Current Issue       : 메시 콜라이더 이슈
/// 
/// Object Name         : 돌다리
/// 
/// Object Description  : 움직이지 않고 공중에 떠 있는 지형 오브젝트, 아래에는 데드존
///                         초반부에서 후반부로 갈수록 연잎의 크기는 작아진다. 중간쯤 세이브 포인트 있음
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
