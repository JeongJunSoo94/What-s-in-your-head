using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JJS.Weapon;
using System;
namespace JJS
{
    [CreateAssetMenu(fileName = "PlayerControllerData", menuName = "Scriptable Object/PlayerControllerData", order = int.MaxValue)]
    public class PlayerControllerData : ScriptableObject
    {
        // 수평 Speed
        #region
        [Header("움직임")]
        [Header("걷는 속력")]
        public float walkSpeed = 4f;
        [Header("달리는 속력")]
        public float runSpeed = 7f;
        [Header("대시 속력")]
        public float dashSpeed = 10f;
        [Header("공중 대시 속력")]
        public float airDashSpeed = 8f;
        [Header("관성 속력")]
        public float inertiaSpeed = 0f;
        [Header("공중 이동 속력")]
        public float airMoveSpeed = 1f;
        [Header("공중 이동 최대 속력")]
        public float airMoveMaxSpeed = 10f;
        //[Tooltip("넉백 수평 속력")]
        //public float knockBackHorizonSpeed = 6f;
        #endregion

        // 수직 Speed
        #region
        [Tooltip("점프 속도")]
        public float jumpSpeed = 10f;
        [Tooltip("공중 점프 속도")]
        public float airJumpSpeed = 6f;
        [Range(-100f, 0f), Tooltip("중력")]
        public float gravity = -9.81f;
        public float gravityCofactor = 0.8f;
        [Range(-100f, -1f), Tooltip("종단속도")]
        public float terminalSpeed = -10f;
        [Tooltip("넉백 수직 속력")]
        public float knockBackVerticalSpeed = 8f;
        #endregion

        // 회전 Speed
        #region
        [Tooltip("캐릭터 이동시 회전 속도")]
        public float rotationSpeed = 720f;
        #endregion

        // 속도 관련 벡터
        #region
        [Tooltip("현재 이동 벡터")]
        public Vector3 moveVec = Vector3.zero; // rigidbody.velocity에 대입할 최종 속도 벡터
        [Tooltip("키입력에 따른 수평 벡터")]
        public Vector3 moveDir = Vector3.zero; // 키 입력에 따른 수평 속도 벡터
        [Tooltip("대시 벡터")]
        public Vector3 dashVec = Vector3.zero; // dash또는 airdash중에 사용할 벡터
        [Tooltip("관성 노말 벡터")]
        public Vector3 inertiaNormalVec = Vector3.zero; // 공중 상태에 사용할 관성 벡터
        [Tooltip("넉백 벡터")]
        public Vector3 knockBackVec = Vector3.zero;
        #endregion

        [HideInInspector] public bool isOn_HP_UI = false;


    }

}
