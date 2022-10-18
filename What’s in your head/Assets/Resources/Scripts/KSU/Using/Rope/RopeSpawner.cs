using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU
{
    public class RopeSpawner : MonoBehaviour
    {
        //public List<GameObject> playerList;

        public GameObject NellaRopeAction;
        public GameObject SteadyRopeAction;

        [Header("Ž�� ���� �Ÿ�")] public float detectingRange = 30f;
        [Header("��ȣ �ۿ� ���� �Ÿ�")] public float interactableRange = 20f;

        [Header("���� ����")] public float ropeLength = 15f;

        [Header("���� ���� ȸ�� �ӷ�")] public float rotationSpeed = 180f;

        [Header("���� ���� � �ӷ�")] public float swingSpeed = 30f;
        [Header("���� ���̿� ���� ���� � �ӷ� ���� ����(0.9�� �������� ��ȭ�� ŭ)"), Range(0, 0.9f)] public float SwingDeltaSpeed = 0.4f;
        [Header("���� �ִ� ���� ���޽� ���ߴ� �ð�")] public float waitTime = 0.1f;

        [Header("���� ���� � �ִ� ����")] public float swingAngle = 60f;

        // Start is called before the first frame update
        void Start()
        {
            InitCollider();
        }

        void InitCollider()
        {
            transform.localScale = new Vector3(1, 1, 1) * (detectingRange * 2f);
        }

        public void StartRopeAction(GameObject player, float moveToRopeSpeed)
        {
            switch (player.tag)
            {
                case "Nella":
                    {
                        NellaRopeAction.GetComponent<Rope>().player = player;
                        NellaRopeAction.GetComponent<Rope>().moveToRopeSpeed = moveToRopeSpeed;
                        NellaRopeAction.SetActive(true);
                    }
                    break;
                case "Steady":
                    {
                        SteadyRopeAction.GetComponent<Rope>().player = player;
                        SteadyRopeAction.GetComponent<Rope>().moveToRopeSpeed = moveToRopeSpeed;
                        SteadyRopeAction.SetActive(true);
                    }
                    break;
            }
            //GameObject obj = Instantiate<GameObject>(ropeAction, transform);
            //obj.GetComponent<RopeAction>().spawner = this;
            //obj.GetComponent<RopeAction>().player = player;
        }

        public float EndRopeAction(GameObject player)
        {
            switch (player.tag)
            {

                case "Nella":
                    {
                        return NellaRopeAction.GetComponent<Rope>().DeacvtivateRope(player);
                    }
                case "Steady":
                    {
                        return SteadyRopeAction.GetComponent<Rope>().DeacvtivateRope(player);
                    }
                default:
                    return 0f;
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, transform.forward * 5f);
        }
    }
}
