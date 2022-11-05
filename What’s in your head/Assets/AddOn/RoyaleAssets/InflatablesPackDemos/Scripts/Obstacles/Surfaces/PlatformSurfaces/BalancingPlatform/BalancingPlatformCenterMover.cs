using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Obstacles
{
    public class BalancingPlatformCenterMover : MonoBehaviour
    {
        [SerializeField] float currentAngle;
        [SerializeField] Vector3 currentUp;

        [SerializeField] float velocity = 1.0f;
        [SerializeField] float velocityUp = 1f;

        [SerializeField] float minAngle = -40f;
        [SerializeField] float maxAngle = 40f;

        public void Rotate(float deltaAngle, Vector3 up)
        {
            deltaAngle = deltaAngle * velocity * Time.deltaTime;
            currentAngle = Mathf.Clamp(currentAngle + deltaAngle, minAngle, maxAngle);
            currentUp = Vector3.Slerp(currentUp, up, Time.deltaTime * velocityUp);

            var deltaRotation = Quaternion.AngleAxis(currentAngle, currentUp);
            transform.rotation = deltaRotation;
        }
    }
}
