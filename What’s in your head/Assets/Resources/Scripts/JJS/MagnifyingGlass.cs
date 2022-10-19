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
        public GameObject particleGather;
        public GameObject particleBeam;
        public BoxCollider paticleBoxCollider;
        public Camera mainCamera;

        Ray ray;

        Vector3 dir;

        public float beamWaitTime=2f;

        public float maxDistance;
        public float curDistance;
        public float curveHeight = 1f;
        public float curveWidth;

        public GameObject targetIK;
        public GameObject direction;
        public GameObject hitPos;
        public GameObject mousePoint;

        public GameObject weapon;
        public GameObject startPos;

        public GameObject hitBox;

        public LayerMask layer;

        public bool beaming;

        private void Awake()
        {
        }

        void Start()
        {
            if (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Joined)
                mainCamera = this.gameObject.transform.parent.GetComponent<CameraController>().FindCamera(); // 멀티용
            else
                mainCamera = this.gameObject.transform.parent.GetComponent<CameraController_Single>().FindCamera(); // 싱글용
        }


        void OnDrawGizmos()
        {
            OnDrawGizmosRay();
        }

        void OnDrawGizmosRay()
        {
            ray.origin = startPos.transform.position;
            ray.direction = dir.normalized;

            Debug.DrawRay(ray.origin, ray.direction * curDistance, Color.red);
            ray.origin = targetIK.transform.position;
            ray.direction = (hitPos.transform.position- targetIK.transform.position).normalized;
            Debug.DrawRay(ray.origin, ray.direction * curDistance, Color.green);
        }

        public void BeamEnable(bool enable)
        {
            particleBeam.SetActive(enable);
        }

        public void EffectEnable(bool enable)
        {
            particleGather.SetActive(enable);
        }
        public void StopBeam()
        {
            if (beaming)
            {
                StopCoroutine("EffectCoroutine");
                beaming = false;
            }
            else
            {
                beaming = false;
            }
            EffectEnable(false);
            BeamEnable(false);
        }
        public void Shoot()
        {
            StartCoroutine("EffectCoroutine");
        }

        IEnumerator EffectCoroutine()
        {
            beaming = true;
            EffectEnable(true);
            yield return new WaitForSeconds(beamWaitTime);
            BeamEnable(true);
        }

        public void HitLine()
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
            particleGather.transform.localScale = new Vector3(1,1, curDistance);
            particleBeam.transform.localScale = new Vector3(1,1, curDistance);

            hitPos.transform.position = hitPoint;
            particleGather.transform.LookAt(hitPoint);
            particleBeam.transform.LookAt(hitPoint);
            targetIK.transform.LookAt(hitPoint);
        }
    }

}
