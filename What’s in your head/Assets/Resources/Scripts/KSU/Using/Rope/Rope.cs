using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU
{
    public class Rope : MonoBehaviour
    {
        //public enum Direction { F, FR, R, BR, B, BL, L, FL, Default }

        public RopeSpawner spawner;
        [SerializeField] GameObject ropeAnchor;
        [SerializeField] GameObject rope;
        public GameObject player;

        public float startXAngle = 0f;
        //public Direction targetDirection = Direction.Default;

        public float rotationX;
        public float startYRotation = 0f;
        public float targetAddYRotationDefault;
        public float currentAddYRotationDefault;
        public float targetAddYRotation;
        public float currentAddYRotation;
        [Header("로프 수평 회전 허용 오차값")] public float rotationTolerance = 5f;

        public bool isReadyToRide = false;
        public bool isRopeExisting = false;

        public bool isSwingForward = true;
        bool isntSwing = false;

        public bool isRotating = true;
        public bool isRotatingToDefault = true;

        public float moveToRopeSpeed = 4f;



        void Start()
        {
            spawner = GetComponentInParent<RopeSpawner>();
        }

        //void Update()
        //{
        //    if(isRopeExisting)
        //    {
        //        InputKey();
        //    }
        //}

        private void FixedUpdate()
        {
            //if (isRopeExisting)
            //{
            //    SetRotation();
            //    Swing();
            //    CalculateDistance();
            //}
            //else
            //{
            //    if (isReadyToRide)
            //    {
            //        MovePlayerToRope();
            //    }
            //}

            if (isRopeExisting)
            {
                if (isRotatingToDefault)
                {
                    SetRotationDefault();
                }
                //else
                //{
                //    SetRotation();
                //}
                if(!isntSwing)
                {
                    Swing();
                }
                //CalculateDistance();
            }
            else
            {
                if (isReadyToRide)
                {
                    MovePlayerToRope();
                }
            }
        }

        void FindStartPoints()
        {
            //gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);

            //Vector3 playerFowardVec = player.transform.forward;
            //playerFowardVec.y = 0;

            //Vector3 playerPosVec = player.transform.position - transform.position;
            //startXAngle = (Vector3.Angle(playerFowardVec, playerPosVec) - 90f);

            //float angle = Vector3.Angle(transform.forward, playerFowardVec);
            //if (Mathf.Abs(angle) > 90f)
            //{
            //    startYRotation = 180f;
            //    startXAngle = -startXAngle;
            //}
            //else
            //{
            //    startYRotation = 0f;
            //}

            gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);

            Vector3 playerPosVec;
            Vector3 playerFowardVec = player.transform.forward;
            playerFowardVec.y = 0;
            float angle = Vector3.Angle(playerFowardVec, transform.forward);
            if (angle > 90f)
            {
                startYRotation = 180f;
            }
            else
            {
                startYRotation = 0f;
            }

            playerPosVec = player.transform.position - transform.position;
            startXAngle = FitInRange(Vector3.Angle(playerPosVec, -transform.up), 0, spawner.swingAngle);
            playerPosVec.y = 0;
            if (Vector3.Angle(playerPosVec, transform.forward) > 90f)
            {
                // 로프 앞 쪽에 플레이어가 있을 때
                if (startYRotation > 90f)
                {
                    // 로프가 보는 방향과 플레이어가 보는 방향이 같을 때
                    transform.LookAt(transform.position + playerPosVec);
                    startXAngle = -startXAngle;
                }
                else
                {
                    // 로프가 보는 방향과 플레이어가 보는 방향이 반대일 때
                    transform.LookAt(transform.position - playerPosVec);
                }
            }
            else
            {
                // 로프 뒤 쪽에 플레이어가 있을 때
                if (startYRotation > 90f)
                {
                    // 로프가 보는 방향과 플레이어가 보는 방향이 같을 때
                    transform.LookAt(transform.position - playerPosVec);

                }
                else
                {
                    // 로프가 보는 방향과 플레이어가 보는 방향이 반대일 때
                    transform.LookAt(transform.position + playerPosVec);
                    startXAngle = -startXAngle;
                }
            }

            currentAddYRotationDefault = transform.localRotation.eulerAngles.y;
            targetAddYRotationDefault = startYRotation;
        }

        void MakeRope()
        {
            FindStartPoints();

            ropeAnchor.transform.localScale = new Vector3(1, 1, 1) * (1f / spawner.transform.localScale.x);
            Vector3 localPos = Vector3.zero;
            localPos.y = -spawner.ropeLength;

            rope.transform.localPosition = localPos;
            player.GetComponent<PlayerController>().enabled = false;
            Vector3 localRot = Vector3.zero;
            localRot.x = startXAngle;
            ropeAnchor.transform.localRotation = Quaternion.Euler(localRot);

            isReadyToRide = true;
        }

        void MovePlayerToRope()
        {
            player.GetComponent<Rigidbody>().velocity = (rope.transform.position - player.transform.position).normalized * moveToRopeSpeed;

            if (Vector3.Distance(player.transform.position, rope.transform.position) < 1f)
            {
                rope.transform.localRotation = Quaternion.Euler(Vector3.zero);
                player.transform.parent = rope.transform;
                player.transform.localPosition = Vector3.zero;
                player.transform.localRotation = Quaternion.Euler(Vector3.zero);

                isRopeExisting = true;
                isRotatingToDefault = true;
                player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                player.GetComponent<PlayerInteractionState>().isMoveToRope = false;
            }
        }

        void AcceptPlayer()
        {
            rope.transform.localRotation = Quaternion.Euler(Vector3.zero);
            player.transform.parent = rope.transform;
            player.transform.localPosition = Vector3.zero;
            player.transform.localRotation = Quaternion.Euler(Vector3.zero);

            isRopeExisting = true;
            player.GetComponent<PlayerInteractionState>().isMoveToRope = false;

        }

        public float DeacvtivateRope(GameObject player)
        {
            isRopeExisting = false;
            player.transform.parent = null;
            this.gameObject.SetActive(false);
            if (!isSwingForward)
            {
                if (rotationX > 0)
                {
                    return rotationX / spawner.swingAngle;
                }
            }
            return -rotationX / spawner.swingAngle;
        }

        float FitInHalfDegree(float Angle)
        {
            return (Angle > 180) ? (Angle - 360f) : Angle;
        }

        void Swing()
        {
            //player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            player.transform.localPosition = Vector3.zero;

            // -180 < rot X <= 180 사이로 고정 
            rotationX = FitInHalfDegree(ropeAnchor.transform.localRotation.eulerAngles.x);

            if (isSwingForward) // 앞으로 갈지 뒤로갈지 결정
            {
                rotationX -= spawner.swingSpeed * Time.fixedDeltaTime * ((Mathf.Abs(spawner.swingAngle)- Mathf.Abs(rotationX)) * spawner.SwingDeltaSpeed / spawner.swingAngle + ( 1 - spawner.SwingDeltaSpeed));
                if (rotationX < -spawner.swingAngle)
                {
                    isSwingForward = false;
                    StartCoroutine("StopSwingInMoment");
                }
            }
            else
            {
                rotationX += spawner.swingSpeed * Time.fixedDeltaTime * ((Mathf.Abs(spawner.swingAngle) - Mathf.Abs(rotationX)) * spawner.SwingDeltaSpeed / spawner.swingAngle + (1 - spawner.SwingDeltaSpeed));
                if (rotationX > spawner.swingAngle)
                {
                    isSwingForward = true;
                    StartCoroutine("StopSwingInMoment");
                }
            }

            // rotation에 대입
            ropeAnchor.transform.localRotation = Quaternion.Euler(Vector3.right * rotationX);
        }

        IEnumerator StopSwingInMoment()
        {
            isntSwing = true;
            yield return new WaitForSeconds(spawner.waitTime);
            isntSwing = false;
        }

        //void InputKey()
        //{
        //    if (isRotating)
        //    {
        //        return;
        //    }

        //    bool F = KeyManager.Instance.GetKey(PlayerAction.MoveForward);
        //    bool B = KeyManager.Instance.GetKey(PlayerAction.MoveBackward);
        //    bool L = KeyManager.Instance.GetKey(PlayerAction.MoveLeft);
        //    bool R = KeyManager.Instance.GetKey(PlayerAction.MoveRight);

        //    Direction input = targetDirection;

        //    if(F)
        //    {
        //        if(R)
        //        {
        //            input = Direction.FR;
        //        }
        //        else if(L)
        //        {
        //            input = Direction.FL;
        //        }
        //        else
        //        {
        //            input = Direction.F;
        //        }
        //    }
        //    else if(B)
        //    {
        //        if (R)
        //        {
        //            input = Direction.BR;
        //        }
        //        else if (L)
        //        {
        //            input = Direction.BL;
        //        }
        //        else
        //        {
        //            input = Direction.B;
        //        }
        //    }
        //    else if(R)
        //    {
        //        input = Direction.R;
        //    }
        //    else if(L)
        //    {
        //        input = Direction.L;
        //    }

        //    if (input != targetDirection)
        //    {
        //        targetDirection = input;
        //        targetAddYRotation = (int)targetDirection * 45;
        //        isRotating = true;
        //    }
        //}

        void SetRotationDefault()
        {
            //player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            // 목표 방향으로 도달하면 회전 멈춤
            if ((Mathf.Abs(currentAddYRotationDefault - targetAddYRotationDefault) < rotationTolerance) || (Mathf.Abs(currentAddYRotationDefault - targetAddYRotationDefault) > (360f - rotationTolerance)))
            {
                isRotatingToDefault = false;
                return;
            }

            //  목표 방향으로 시계, 반시계 중 가까운 방향으로 회전
            if (currentAddYRotationDefault > targetAddYRotationDefault)
            {
                if ((currentAddYRotationDefault - targetAddYRotationDefault) > (targetAddYRotationDefault + 360f - currentAddYRotationDefault))
                {
                    currentAddYRotationDefault += spawner.rotationSpeed * Time.fixedDeltaTime;
                }
                else
                {
                    currentAddYRotationDefault -= spawner.rotationSpeed * Time.fixedDeltaTime;
                }
            }
            else
            {
                if ((currentAddYRotationDefault + 360f - targetAddYRotationDefault) > (targetAddYRotationDefault - currentAddYRotationDefault))
                {
                    currentAddYRotationDefault += spawner.rotationSpeed * Time.fixedDeltaTime;
                }
                else
                {
                    currentAddYRotationDefault -= spawner.rotationSpeed * Time.fixedDeltaTime;
                }
            }

            currentAddYRotationDefault = FitInRange(currentAddYRotationDefault, 0f, 360f);

            // rotation 변경
            transform.localRotation = Quaternion.Euler(Vector3.up * currentAddYRotationDefault);
        }

        //void SetRotation()
        //{
        //    if (isRotating)
        //    {
        //        //player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //        // 목표 방향으로 도달하면 회전 멈춤
        //        if (Mathf.Abs(currentAddYRotation - targetAddYRotation) < rotationTolerance)
        //        {
        //            isRotating = false;
        //            return;
        //        }

        //        //  목표 방향으로 시계, 반시계 중 가까운 방향으로 회전
        //        if (currentAddYRotation > targetAddYRotation)
        //        {
        //            if ((currentAddYRotation - targetAddYRotation) > (targetAddYRotation + 360f - currentAddYRotation))
        //            {
        //                currentAddYRotation += spawner.rotationSpeed * Time.fixedDeltaTime;
        //            }
        //            else
        //            {
        //                currentAddYRotation -= spawner.rotationSpeed * Time.fixedDeltaTime;
        //            }
        //        }
        //        else
        //        {
        //            if ((currentAddYRotation + 360f - targetAddYRotation) > (targetAddYRotation - currentAddYRotation))
        //            {
        //                currentAddYRotation += spawner.rotationSpeed * Time.fixedDeltaTime;
        //            }
        //            else
        //            {
        //                currentAddYRotation -= spawner.rotationSpeed * Time.fixedDeltaTime;
        //            }
        //        }

        //        currentAddYRotation = FitInRange(currentAddYRotation, 0f, 360f);

        //        float totalYRotaion = startYRotation + currentAddYRotation;

        //        totalYRotaion = FitInRange(totalYRotaion, 0f, 360f);

        //        // rotation 변경
        //        transform.localRotation = Quaternion.Euler(Vector3.up * totalYRotaion);
        //    }
        //}

        float FitInRange(float num, float min, float max)
        {
            if (num < min)
            {
                num += max;
            }
            else if (num > max)
            {
                num -= max;
            }
            return num;
        }

        //void CalculateDistance()
        //{
        //    Debug.Log(Vector3.Distance(player.transform.position, this.transform.position));
        //}

        private void OnEnable()
        {
            MakeRope();
        }
    }
}
