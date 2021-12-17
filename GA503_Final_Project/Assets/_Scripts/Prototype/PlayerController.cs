using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance = null;
    
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float lateralMoveAcceleration = 3f;
    [SerializeField] private float lateralBreakDeceleration = 3f;
    [SerializeField] private float lateralMaxMoveSpeed = 3f;
    
    [SerializeField] private float forwardDefaultMoveSpeed = 5f;
    [SerializeField] private float forwardBoostMoveSpeed = 8f;
    
    [SerializeField] private float forwardMoveAcceleration = 10f;
    [SerializeField] private float forwardMoveDeceleration = 5f;
    [SerializeField] private float forwardMaxMoveSpeed = 10f;
    
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Animator cameraStateAnimator;
    [SerializeField] private CinemachineDollyCart dollyCart;

    [Space]
    [SerializeField] private Animator puleEffectAnimator;

    //private Vector3 velocity = Vector3.zero;

    private Transform dollyTransform => dollyCart.transform;

    private Vector2 lateralMoveInput;
    private bool isBoosting = false;

    private float currentDollyTrackSpeed = 0f;
    private float currentDollyPositionSpeed = 0f;
    private Vector3 lastDollyFixedUpdatePos = Vector3.zero;

    private bool isPlayerMoving = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        dollyCart.m_Speed = forwardDefaultMoveSpeed;
        currentDollyTrackSpeed = dollyCart.m_Speed;
        lastDollyFixedUpdatePos = dollyTransform.position;
        
        this.InvokeAction((() =>
        {
            isPlayerMoving = true;
        }), 0.8f);
    }

    private void Update()
    {
        if (isPlayerMoving == false)
        {
            dollyCart.m_Speed = 0f;
            return;
        }
        
        // Play pulse
        if (Input.GetButtonDown("Fire1"))
        {
            
        }
        
        lateralMoveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        //isBoosting = Input.GetButton("Fire3");

        cameraStateAnimator.SetBool("IsZoomedOut", isBoosting);
        
        float forwardAccel = (isBoosting ? forwardMoveAcceleration : -forwardMoveDeceleration);
        Vector3 prevForwardVelocity = Vector3.Project(_rigidbody.velocity, dollyTransform.forward);
        if (!isBoosting && prevForwardVelocity.sqrMagnitude < 0.1f)
        {
            forwardAccel = 0f;
        }
        currentDollyTrackSpeed = Mathf.Clamp(currentDollyTrackSpeed + forwardAccel * Time.deltaTime, forwardDefaultMoveSpeed, forwardBoostMoveSpeed);
        dollyCart.m_Speed = currentDollyTrackSpeed;
        
        
    }

    private void LateUpdate()
    {
        // lateralMoveInput = Vector2.zero;
        // boostPressed = false;

        //Vector3 currPos = transform.position;
        //transform.position = currPos;
    }

    private void FixedUpdate()
    {
        Vector3 currDollyFixedUpdatePos = dollyTransform.position;
        if (isPlayerMoving == false || currDollyFixedUpdatePos == lastDollyFixedUpdatePos)
        {
            _rigidbody.velocity = Vector3.zero;
            return;
        }
        
        Vector3 prevLateralVelocity = Vector3.ProjectOnPlane(_rigidbody.velocity, dollyTransform.forward);
        Vector3 lateralMove = Vector3.zero;

        if (lateralMoveInput.sqrMagnitude > 0.01f)
        {
            lateralMove = (dollyTransform.right * lateralMoveInput.x) + (dollyTransform.up * lateralMoveInput.y);
            lateralMove = lateralMove.normalized * lateralMoveAcceleration * Time.fixedDeltaTime;
        }
        else if(prevLateralVelocity.sqrMagnitude > 0.01f)
        {
            lateralMove = -prevLateralVelocity.normalized * lateralBreakDeceleration * Time.fixedDeltaTime;
        }
        _rigidbody.AddForce(lateralMove, ForceMode.Impulse);
        /*
        float forwardAccel = (boostPressed ? forwardMoveAcceleration : -forwardMoveDeceleration);
        Vector3 prevForwardVelocity = Vector3.Project(_rigidbody.velocity, dollyTransform.forward);
        if (!boostPressed && prevForwardVelocity.sqrMagnitude < 0.1f)
        {
            forwardAccel = 0f;
        }
        Vector3 forwardMove = dollyTransform.forward * (forwardAccel) * Time.fixedDeltaTime;
        
        //_rigidbody.AddForce(lateralMove + forwardMove, ForceMode.Impulse);
        */

        Vector3 lateralVelocity = Vector3.ProjectOnPlane(_rigidbody.velocity, dollyTransform.forward);
        if (lateralVelocity.sqrMagnitude > Mathf.Pow(lateralMaxMoveSpeed, 2))
        {
            lateralVelocity = lateralVelocity.normalized * lateralMaxMoveSpeed;
        }

        /*
        Vector3 forwardVelocity = Vector3.Project(_rigidbody.velocity, dollyTransform.forward);
        if (forwardVelocity.sqrMagnitude > Mathf.Pow(forwardMaxMoveSpeed, 2))
        {
            forwardVelocity = forwardVelocity.normalized * forwardMaxMoveSpeed;
        }
        */
        
        
        currentDollyPositionSpeed = (currDollyFixedUpdatePos - lastDollyFixedUpdatePos).magnitude / Time.fixedDeltaTime;
        Vector3 forwardVelocity = currentDollyPositionSpeed * dollyTransform.forward;
        
        //Vector3 forwardVelocity = dollyCart.m_Speed * dollyTransform.forward;
        
        _rigidbody.velocity = lateralVelocity + forwardVelocity;
        
        lastDollyFixedUpdatePos = currDollyFixedUpdatePos;
    }

    public void StartBoost(float argDuration)
    {
        isBoosting = true;
        this.InvokeAction((() =>
        {
            isBoosting = false;
        }), argDuration);
    }

    public void KillPlayer()
    {
        isPlayerMoving = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Enemy"))
        {
            KillPlayer();
        }
    }

}
