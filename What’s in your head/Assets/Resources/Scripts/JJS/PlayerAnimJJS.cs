using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimJJS : MonoBehaviour
{
    Animator animator;
    private string currentState;

    const string PLAYER_RUN = "RUN00_F";
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        ChangeAnimationState(PLAYER_RUN);
    }

    void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        animator.Play(newState);

        currentState = newState;
    }
}
