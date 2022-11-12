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
        // ���� Speed
        #region
        [Header("������")]
        [Header("�ȴ� �ӷ�")]
        public float walkSpeed = 4f;
        [Header("�޸��� �ӷ�")]
        public float runSpeed = 7f;
        [Header("��� �ӷ�")]
        public float dashSpeed = 10f;
        [Header("���� ��� �ӷ�")]
        public float airDashSpeed = 8f;
        [Header("���� �ӷ�")]
        public float inertiaSpeed = 0f;
        [Header("���� �̵� �ӷ�")]
        public float airMoveSpeed = 1f;
        [Header("���� �̵� �ִ� �ӷ�")]
        public float airMoveMaxSpeed = 10f;
        //[Tooltip("�˹� ���� �ӷ�")]
        //public float knockBackHorizonSpeed = 6f;
        #endregion

        // ���� Speed
        #region
        [Tooltip("���� �ӵ�")]
        public float jumpSpeed = 10f;
        [Tooltip("���� ���� �ӵ�")]
        public float airJumpSpeed = 6f;
        [Range(-100f, 0f), Tooltip("�߷�")]
        public float gravity = -9.81f;
        public float gravityCofactor = 0.8f;
        [Range(-100f, -1f), Tooltip("���ܼӵ�")]
        public float terminalSpeed = -10f;
        [Tooltip("�˹� ���� �ӷ�")]
        public float knockBackVerticalSpeed = 8f;
        #endregion

        // ȸ�� Speed
        #region
        [Tooltip("ĳ���� �̵��� ȸ�� �ӵ�")]
        public float rotationSpeed = 720f;
        #endregion

        // �ӵ� ���� ����
        #region
        [Tooltip("���� �̵� ����")]
        public Vector3 moveVec = Vector3.zero; // rigidbody.velocity�� ������ ���� �ӵ� ����
        [Tooltip("Ű�Է¿� ���� ���� ����")]
        public Vector3 moveDir = Vector3.zero; // Ű �Է¿� ���� ���� �ӵ� ����
        [Tooltip("��� ����")]
        public Vector3 dashVec = Vector3.zero; // dash�Ǵ� airdash�߿� ����� ����
        [Tooltip("���� �븻 ����")]
        public Vector3 inertiaNormalVec = Vector3.zero; // ���� ���¿� ����� ���� ����
        [Tooltip("�˹� ����")]
        public Vector3 knockBackVec = Vector3.zero;
        #endregion

        [HideInInspector] public bool isOn_HP_UI = false;


    }

}
