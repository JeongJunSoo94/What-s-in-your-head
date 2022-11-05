using KSU.AutoAim.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAimObject : MonoBehaviour
{
    public SteadyAutoAimAction player;
    public GameObject spawner;
    protected Rigidbody objectRigidbody;
    protected AudioSource audioSource;

    protected float moveSpeed = 15f;
    protected Vector3 endPosistion;
    protected float departingOffset = 0.2f;
    public bool isEndPosition = false;
    public bool isSucceeded = false;

    // Start is called before the first frame update
    //void Awake()
    //{
    //    objectRigidbody = GetComponent<Rigidbody>();
    //    audioSource = GetComponent<AudioSource>();
    //    JCW.AudioCtrl.AudioSettings.SetAudio(audioSource, 1f, 50f);
    //}

    public virtual void InitObject(Vector3 startPos, Vector3 endPos, float objectSpeed, float offset)
    {
        objectRigidbody.velocity = Vector3.zero;
        transform.position = startPos;
        endPosistion = endPos;
        isEndPosition = false;
        isSucceeded = false;
        moveSpeed = objectSpeed;
        departingOffset = offset;
        this.gameObject.SetActive(true);
    }

    protected void MoveToEndPosition()
    {
        //1.리지드바디 추가해서 벨로시티로움직이기
        if (Vector3.Distance(endPosistion, transform.position) < departingOffset)
        {
            isEndPosition = true;
            if (this.gameObject.activeSelf)
            {
                player.RecieveAutoAimObjectInfo(false, null, AutoAimTargetType.Null);
            }
            this.gameObject.SetActive(false);
        }
        else
        {
            isEndPosition = false;
        }
        transform.LookAt(endPosistion);
        Vector3 dir = (endPosistion - transform.position).normalized;
        objectRigidbody.velocity = dir * moveSpeed;
    }
}
