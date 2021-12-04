using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float lateralMoveAcceleration = 3f;
    [SerializeField] private float lateralBreakDeceleration = 3f;
    [SerializeField] private float lateralMaxMoveSpeed = 3f;
    
    [SerializeField] private float forwardMoveAcceleration = 10f;
    [SerializeField] private float forwardMoveDeceleration = 5f;
    [SerializeField] private float forwardMaxMoveSpeed = 10f;
    
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Animator cameraStateAnimator;

    //private Vector3 velocity = Vector3.zero;

    private Vector2 lateralMoveInput;
    private bool boostPressed = false;
    
    private void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main?.transform;
        }
    }

    private void Update()
    {
        
        lateralMoveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        boostPressed = Input.GetButton("Jump");
        
        cameraStateAnimator.SetBool("IsZoomedOut", boostPressed);
    }

    private void LateUpdate()
    {
        // lateralMoveInput = Vector2.zero;
        // boostPressed = false;
    }

    private void FixedUpdate()
    {
        Vector3 prevLateralVelocity = Vector3.ProjectOnPlane(_rigidbody.velocity, cameraTransform.forward);
        Vector3 lateralMove = Vector3.zero;

        if (lateralMoveInput.sqrMagnitude > 0.01f)
        {
            lateralMove = (cameraTransform.right * lateralMoveInput.x) + (cameraTransform.up * lateralMoveInput.y);
            lateralMove = lateralMove.normalized * lateralMoveAcceleration * Time.fixedDeltaTime;
        }
        else if(prevLateralVelocity.sqrMagnitude > 0.01f)
        {
            lateralMove = -prevLateralVelocity.normalized * lateralBreakDeceleration * Time.fixedDeltaTime;
        }
        
        float forwardAccel = (boostPressed ? forwardMoveAcceleration : -forwardMoveDeceleration);
        Vector3 prevForwardVelocity = Vector3.Project(_rigidbody.velocity, cameraTransform.forward);
        if (!boostPressed && prevForwardVelocity.sqrMagnitude < 0.1f)
        {
            forwardAccel = 0f;
        }
        Vector3 forwardMove = cameraTransform.forward * (forwardAccel) * Time.fixedDeltaTime;
        
        _rigidbody.AddForce(lateralMove + forwardMove, ForceMode.Impulse);

        Vector3 lateralVelocity = Vector3.ProjectOnPlane(_rigidbody.velocity, cameraTransform.forward);
        if (lateralVelocity.sqrMagnitude > Mathf.Pow(lateralMaxMoveSpeed, 2))
        {
            lateralVelocity = lateralVelocity.normalized * lateralMaxMoveSpeed;
        }

        Vector3 forwardVelocity = Vector3.Project(_rigidbody.velocity, cameraTransform.forward);
        if (forwardVelocity.sqrMagnitude > Mathf.Pow(forwardMaxMoveSpeed, 2))
        {
            forwardVelocity = forwardVelocity.normalized * forwardMaxMoveSpeed;
        }
        
        _rigidbody.velocity = lateralVelocity + forwardVelocity;
        //if(boostPressed)

    }
}
