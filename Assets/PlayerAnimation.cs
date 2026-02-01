using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    [SerializeField] private Animator animator;
    [SerializeField] private float rotateSpeed = 2f;
    [SerializeField] private float walkAngle = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        float forwardValue = Mathf.Lerp(0, 1, playerController.tiltAngle / walkAngle);

        Debug.Log("Setting animation forwards to: " + forwardValue);
        animator.SetFloat("Forwards", forwardValue);

    }
}
