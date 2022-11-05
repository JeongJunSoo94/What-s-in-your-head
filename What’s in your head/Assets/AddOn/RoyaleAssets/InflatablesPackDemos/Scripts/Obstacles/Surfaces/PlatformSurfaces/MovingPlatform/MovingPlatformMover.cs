using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Obstacles
{
    public class MovingPlatformMover : MonoBehaviour
    {
        public float speed = 2f;
        public float angularSpeed = 0f;

        [SerializeField] Transform start = null;
        [SerializeField] Transform end = null;

        private Vector3 deltaPosition;
        private Quaternion deltaRotation;

        private bool isMovingForward = true;
        private bool isDisplacementUpdated = false;


        private Vector3 CurrentDestination => isMovingForward ? end.position : start.position;
        private Vector3 UpDirection => transform.parent != null ? transform.parent.up : transform.up;


        private void Start()
        {
            if(start == null)
            {
                start = transform.Find("start");
            }

            if(end == null)
            {
                end = transform.Find("end");
            }

            start.SetParent(transform.parent, true);
            end.SetParent(transform.parent, true);
        }

        private void Update()
        {
            if (!isDisplacementUpdated)
            {
                UpdateDisplacement(Time.deltaTime);
            }

            transform.SetPositionAndRotation(transform.position + deltaPosition, deltaRotation * transform.rotation);

            if ((CurrentDestination - transform.position).sqrMagnitude < 1E-04f)
            {
                isMovingForward = !isMovingForward;
            }

            isDisplacementUpdated = false;
        }

        private void UpdateDisplacement(float deltaTime)
        {
            deltaPosition = Vector3.MoveTowards(Vector3.zero, CurrentDestination - transform.position, speed * deltaTime);
            deltaRotation = Quaternion.AngleAxis(angularSpeed * deltaTime, UpDirection);
            isDisplacementUpdated = true;
        }

        public void GetDisplacement(out Vector3 deltaPosition, out Quaternion deltaRotation)
        {
            if (!isDisplacementUpdated)
            {
                UpdateDisplacement(Time.deltaTime);
            }

            deltaPosition = this.deltaPosition;
            deltaRotation = this.deltaRotation;
        }
    }
}