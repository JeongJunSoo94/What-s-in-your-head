using JCW.UI.InGame.Indicator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU
{
    public class RopeSpawner : MonoBehaviour
    {
        [SerializeField] GameObject NellaRopeCenter;
        [SerializeField] GameObject SteadyRopeCenter;
        [SerializeField] GameObject detector;

        //[Header("UI �ݶ��̴�")] [SerializeField] private GameObject UIObj;
        [Header("_______���� ���� ��_______")]
        [Header("Ž�� ���� �Ÿ�")] 
        public float detectingRange = 30f;
        [Header("��ȣ �ۿ� ���� �Ÿ�")] 
        public float interactableRange = 20f;
        [Header("���� ����")] 
        public float ropeLength = 15f;
        [Header("���� ���� ȸ�� �ӷ�")] 
        public float rotationSpeed = 180f;
        [Header("���� ���� ȸ�� ���� ��(ȸ�� �ӷ��� Ŀ�� ������ ���� �� �����ؾ���)")] 
        public float rotationOffset = 2f;
        [Header("���� ���� � �ӷ�")] 
        public float swingSpeed = 30f;
        [Header("���� ���̿� ���� ���� � �ӷ� ���� ����(0.9�� �������� ��ȭ�� ŭ)"), Range(0, 0.9f)] 
        public float SwingDeltaSpeed = 0.4f;
        [Header("���� �ִ� ���� ���޽� ���ߴ� �ð�")] 
        public float waitTime = 0.1f;
        [Header("���� ���� � �ִ� ����")] 
        public float swingAngle = 60f;

        // Start is called before the first frame update
        void Start()
        {
            detector.transform.localScale = new Vector3(1, 1, 1) * (detectingRange * 2f);
            transform.GetChild(3).GetComponent<ConvertIndicator>().SetInteractableRange(detectingRange, interactableRange);
        }

        public void StartRopeAction(GameObject player, float moveToRopeSpeed)
        {
            switch (player.tag)
            {
                case "Nella":
                    {
                        if(NellaRopeCenter.activeSelf)
                        {
                            player.GetComponent<PlayerInteractionState>().isMoveToRope = false;
                            return;
                        }
                        NellaRopeCenter.GetComponent<Rope>().player = player;
                        NellaRopeCenter.GetComponent<Rope>().moveToRopeSpeed = moveToRopeSpeed;
                        NellaRopeCenter.SetActive(true);
                    }
                    break;
                case "Steady":
                    {
                        if (SteadyRopeCenter.activeSelf)
                        {
                            player.GetComponent<PlayerInteractionState>().isMoveToRope = false;
                            return;
                        }
                        SteadyRopeCenter.GetComponent<Rope>().player = player;
                        SteadyRopeCenter.GetComponent<Rope>().moveToRopeSpeed = moveToRopeSpeed;
                        SteadyRopeCenter.SetActive(true);
                    }
                    break;
            }
        }

        public float EndRopeAction(GameObject player)
        {
            switch (player.tag)
            {

                case "Nella":
                    {
                        return NellaRopeCenter.GetComponent<Rope>().DeacvtivateRope(player);
                    }
                case "Steady":
                    {
                        return SteadyRopeCenter.GetComponent<Rope>().DeacvtivateRope(player);
                    }
                default:
                    return 0f;
            }
        }
    }
}
