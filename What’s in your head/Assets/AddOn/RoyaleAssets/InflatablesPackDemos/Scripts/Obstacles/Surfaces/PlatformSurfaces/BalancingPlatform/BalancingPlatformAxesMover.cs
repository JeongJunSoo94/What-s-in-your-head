using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Obstacles
{
    public class BalancingPlatformAxesMover : MonoBehaviour
    {
        [SerializeField] float currentAngle;

        [SerializeField] float velocity = 1.0f;
        [SerializeField] float minAngle = -85f;
        [SerializeField] float maxAngle = 85f;

        private Quaternion originRotation;

        private void Start()
        {
            originRotation = transform.rotation;
        }

        public void Rotate(float deltaAngle, Vector3 rotationAxes)
        {
            deltaAngle = deltaAngle * velocity * Time.deltaTime;
            currentAngle = Mathf.Clamp(currentAngle + deltaAngle, minAngle, maxAngle);

            var targetRotation = Quaternion.AngleAxis(currentAngle, rotationAxes);
            transform.rotation = targetRotation * originRotation;
        }
    }
}