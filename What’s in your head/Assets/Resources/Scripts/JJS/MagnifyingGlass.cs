using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YC.Camera_;
using YC.Camera_Single;
using JCW.AudioCtrl;
using Photon.Pun;

namespace JJS.Weapon
{
    public class MagnifyingGlass : MonoBehaviour
    {

        Ray ray;

        Vector3 dir;

        [Header("Beam Info")]
        public float beamWaitTime=2f;

        public float maxDistance;
        public float curDistance;
        public float curveHeight = 1f;
        public float curveWidth;
        public LayerMask layerMask;

        [Header("Don't Touch")]
        public GameObject particleGather;
        public GameObject particleBeam;
        public BoxCollider paticleBoxCollider;
        public Camera mainCamera;
        public GameObject targetIK;
        public GameObject direction;
        public GameObject hitPos;
        public GameObject mousePoint;
        public GameObject weapon;
        public GameObject startPos;
        public GameObject hitBox;
        public bool beaming;
        public int lineCountCheck=0;

        CameraController cameraController; // << : 찬 수정 

        AudioSource audioSource;
        PhotonView pv;

        private void Awake()
        {
            audioSource = this.gameObject.GetComponentInParent<AudioSource>();
            pv = this.gameObject.GetComponentInParent<PhotonView>();
        }

        void Start()
        {
            mainCamera = this.gameObject.transform.parent.GetComponent<CameraController>().FindCamera(); // 멀티용
            cameraController = this.gameObject.transform.parent.GetComponent<CameraController>(); // << : 찬 수정 

            SoundManager.Set3DAudio(pv.ViewID, audioSource, 1.0f, 10f, false);
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
            if(enable)
                SoundManager.Instance.PlayIndirect3D("S3_SteadyMagnifying", pv.ViewID);          
            else
                SoundManager.Instance.Stop3D(pv.ViewID);
            
            particleBeam.SetActive(enable);

            if(this.gameObject.transform.parent.GetComponent<PhotonView>().IsMine)
                cameraController.SendMessage(nameof(CameraController.ShakeCamera), enable); 
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
            lineCountCheck = 0;
            StartCoroutine("EffectCoroutine");
        }

        IEnumerator EffectCoroutine()
        {
            beaming = true;
            EffectEnable(true);
            yield return new WaitForSeconds(beamWaitTime);
            BeamEnable(true);
        }

        public void HitLine(int type)
        {
            RaycastHit hit;
            Vector3 hitPoint;
            if (type == 1)
            {
                if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, maxDistance, layerMask, QueryTriggerInteraction.Ignore))
                {
                    if (hit.distance > Vector3.Distance(startPos.transform.position, mainCamera.transform.position))
                    {
                        //dir = (hit.point - startPos.transform.position).normalized;
                        hitPoint = hit.point;
                    }
                    else
                    {
                        hitPoint = MaxPhysicsLine(mainCamera.transform.position, mainCamera.transform.forward);
                    }
                }
                else
                {
                    hitPoint = MaxPhysicsLine(mainCamera.transform.position, mainCamera.transform.forward);
                }
            }
            else
            {
                dir = (mousePoint.transform.position - startPos.transform.position).normalized;

                if (Physics.Raycast(startPos.transform.position, dir, out hit, maxDistance, layerMask, QueryTriggerInteraction.Ignore))
                {
                    hitPoint = hit.point;
                }
                else
                {
                    hitPoint = MaxPhysicsLine(startPos.transform.position, dir);
                }
            }
            if (type != 3)
            {
                hitPos.transform.position = hitPoint;
            }
            else
            {
                if (lineCountCheck == 0)
                {
                    lineCountCheck++;
                    hitPos.transform.position = mousePoint.transform.localPosition;
                }
            }

            // hitPos.transform.position = hitPoint;
            //if (Physics.Raycast(startPos.transform.position, dir, out hit, maxDistance, -1, QueryTriggerInteraction.Ignore))
            //{
            //    hitPoint = hit.point;
            //    curDistance = Vector3.Distance(startPos.transform.position, hitPoint);
            //}
            //else
            //{
            //    hitPoint = startPos.transform.position + dir * maxDistance;

            //    curDistance = Vector3.Distance(startPos.transform.position, hitPoint);

            //}
            curDistance = Vector3.Distance(startPos.transform.position, hitPos.transform.position);
            particleGather.transform.localScale = new Vector3(1,1, curDistance);
            particleBeam.transform.localScale = new Vector3(1,1, curDistance);

            particleGather.transform.LookAt(hitPos.transform.position);
            particleBeam.transform.LookAt(hitPos.transform.position);
            targetIK.transform.LookAt(hitPos.transform.position);
        }

        Vector3 MaxPhysicsLine(Vector3 startPosition, Vector3 rayDirection)
        {
            return startPosition + rayDirection * maxDistance;

        }
    }

}
