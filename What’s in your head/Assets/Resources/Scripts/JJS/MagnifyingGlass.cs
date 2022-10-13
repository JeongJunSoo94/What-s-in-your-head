using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YC.Camera_;
using YC.Camera_Single;
using Photon.Pun;

namespace JJS.Weapon
{
    public class MagnifyingGlass : MonoBehaviour
    {
        public Camera mainCamera;

        Ray ray;

        Vector3 dir;
        public float maxDistance;
        public float curDistance;

        public GameObject IK;
        public GameObject Weapon;
        public GameObject startPos;
        private void Awake()
        {
        }

        void Start()
        {
            if (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Joined)
                mainCamera = this.gameObject.transform.parent.GetComponent<CameraController>().mainCam; // 멀티용
            else
                mainCamera = this.gameObject.transform.parent.GetComponent<CameraController_Single>().mainCam; // 싱글용

            //mainCamera = this.gameObject.transform.parent.GetComponent<CameraController_Single>().mainCam; // 싱글용
            //mainCamera = this.gameObject.transform.parent.GetComponent<CameraController>().mainCam; // 멀티용

        }

        private void FixedUpdate()
        {
            HitLine();
        }

        public void Shoot()
        {
        }

        void OnDrawGizmos()
        {
            OnDrawGizmosRay();
        }

        void OnDrawGizmosRay()
        {
            ray.origin = startPos.transform.position;
            ray.direction = dir;

            Debug.DrawRay(ray.origin, ray.direction * curDistance, Color.red);
        }

        void HitLine()
        {
            RaycastHit hit;
            Vector3 hitPoint;
            dir = mainCamera.transform.forward;

            if (Physics.Raycast(startPos.transform.position, dir, out hit, maxDistance, -1, QueryTriggerInteraction.Ignore))
            {
                hitPoint = hit.point;
                curDistance = Vector3.Distance(startPos.transform.position, hitPoint);
            }
            else
            {
                hitPoint = startPos.transform.position + dir * maxDistance;

                curDistance = Vector3.Distance(startPos.transform.position, hitPoint);

            }
            IK.transform.position = hitPoint;
            Weapon.transform.LookAt(hitPoint);
        }
    }

}
