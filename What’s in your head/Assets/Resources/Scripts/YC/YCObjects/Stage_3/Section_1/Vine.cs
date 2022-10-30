using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// 
/// Current Issue       : ����
/// 
/// Object Name         : ����
/// 
/// Object Description  : ������ ���� ������Ʈ�� ���� ��ȭ�ҽ�, ������ �Ǵ� ������Ʈ�̴�. (���Թ��̶�� ����)
/// 
/// Script Description  : ��ŸƮ���� ������ �������� ã�� �� ����, ������Ʈ���� ������ �������� ��ȭ���θ� üũ�Ѵ�. 
///                         ��� ������ ��ȭ�ȴٸ� ���� ������ �ڷ�ƾ �Լ��� ȣ���Ѵ�.
///                         
/// Player Intraction   : ����
/// 
/// </summary>

namespace YC_OBJ
{
    public class Vine : MonoBehaviour
    {
        [Header("<��ȹ ���� ����>")]
        [Space]

        [Tooltip("ȸ�� �ӵ�")]
        [SerializeField] float rotSpeed;

        [Tooltip("���� ����������� ������ �ð�")]
        [SerializeField] float openDelay;

        Transform leftAxis;     // ���� ����
        Transform rightAxis;    // ������ ����

        float MaxAxis = 90;

        float leftAsixMax;
        float rightAsixMax;


        [HideInInspector] public bool isOpen { get; private set; }
        [HideInInspector] public List<ContaminatedTree> contaminatedTrees { get; private set; }

        string findObjTag = "ContTree";

        void Awake()
        {
            leftAxis = transform.GetChild(0).gameObject.GetComponent<Transform>();
            rightAxis = transform.GetChild(1).gameObject.GetComponent<Transform>();

            isOpen = false;

            contaminatedTrees = new List<ContaminatedTree>();

            leftAsixMax = 360 - MaxAxis;
            rightAsixMax = MaxAxis;
        }

        void Start()
        {
            FindTrees();
        }

        void Update()
        {
            CheckIsPure();

            if (isOpen)
            {
                StartCoroutine("OpenPassage");
                isOpen = false;
            }
        }

        void FindTrees()
        {
            GameObject[] temp = GameObject.FindGameObjectsWithTag("ContTree");

            foreach (GameObject obj in temp)
            {
                contaminatedTrees.Add(obj.GetComponent<ContaminatedTree>());
            }
        }

        void CheckIsPure()
        {
            if (isOpen) return; // �ߺ� ���� ����

            foreach (ContaminatedTree tree in contaminatedTrees)
            {
                if (!tree.isPure)
                    return;
            }

            isOpen = true;
        }


        IEnumerator OpenPassage()
        {
            yield return new WaitForSeconds(openDelay);

            while (rightAxis.rotation.eulerAngles.z < rightAsixMax)
            {
                leftAxis.transform.Rotate(new Vector3(0, 0, -rotSpeed * Time.deltaTime));
                rightAxis.transform.Rotate(new Vector3(0, 0, rotSpeed * Time.deltaTime));

                if (rightAxis.rotation.eulerAngles.z > rightAsixMax) 
                    rightAxis.rotation = Quaternion.Euler(0, 0, rightAsixMax);
              
                yield return null;
            }
        }
    }
}
