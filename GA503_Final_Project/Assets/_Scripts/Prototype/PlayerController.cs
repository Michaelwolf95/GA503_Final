using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance = null;
    
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Animator cameraStateAnimator;
    [SerializeField] private CinemachineDollyCart dollyCart;
    
    [Space]
    [SerializeField] private float lateralMoveAcceleration = 3f;
    [SerializeField] private float lateralBreakDeceleration = 3f;
    [SerializeField] private float lateralMaxMoveSpeed = 3f;
    
    [SerializeField] private float forwardDefaultMoveSpeed = 5f;
    [SerializeField] private float forwardBoostMoveSpeed = 8f;
    
    [SerializeField] private float forwardMoveAcceleration = 10f;
    [SerializeField] private float forwardMoveDeceleration = 5f;
    
    [Space]
    [SerializeField] private Animator puleEffectAnimator;
    [SerializeField] private float pulseCooldownDuration = 0.4f;

    [Space] 
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip boostSound;
    [SerializeField] private AudioClip pulseSound;
    [SerializeField] private AudioClip deathSound;

    //private Vector3 velocity = Vector3.zero;

    private Transform dollyTransform => dollyCart.transform;

    private Vector2 lateralMoveInput;
    private bool isBoosting = false;

    private float currentDollyTrackSpeed = 0f;
    private float currentDollyPositionSpeed = 0f;
    private Vector3 lastDollyFixedUpdatePos = Vector3.zero;
    

    public bool isPlayerDead { get; private set; }
    private bool isPlayerMoving = false;
    private bool pulseAvailable = false;

    private void Awake()
    {
        Instance = this;
    }
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
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
            PulseAttack();
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

        Vector3 lateralVelocity = Vector3.ProjectOnPlane(_rigidbody.velocity, dollyTransform.forward);
        if (lateralVelocity.sqrMagnitude > Mathf.Pow(lateralMaxMoveSpeed, 2))
        {
            lateralVelocity = lateralVelocity.normalized * lateralMaxMoveSpeed;
        }

        currentDollyPositionSpeed = (currDollyFixedUpdatePos - lastDollyFixedUpdatePos).magnitude / Time.fixedDeltaTime;
        //Vector3 forwardVelocity = currentDollyPositionSpeed * dollyTransform.forward;
        //Vector3 forwardVelocity = dollyCart.m_Speed * dollyTransform.forward;

        float currSpeed = Mathf.Min(currentDollyPositionSpeed, dollyCart.m_Speed);
        
        Vector3 distFromCam = Vector3.Project(transform.position - mainCamera.transform.position, mainCamera.transform.forward);
        Vector3 dollyDistFromCam = Vector3.Project(dollyTransform.position - mainCamera.transform.position, mainCamera.transform.forward);
        if (dollyDistFromCam.magnitude + 0.5f < distFromCam.magnitude)
        {
            currSpeed -= 1f;
        }
        
        Vector3 forwardVelocity = currSpeed * dollyTransform.forward;
        
        _rigidbody.velocity = lateralVelocity + forwardVelocity;
        
        lastDollyFixedUpdatePos = currDollyFixedUpdatePos;
    }

    public void OnLevelStart()
    {
        isPlayerMoving = true;
        pulseAvailable = true;
    }
    
    public void OnLevelCompleted()
    {
        isPlayerMoving = false;
        pulseAvailable = false;
    }

    public void StartBoost(float argDuration)
    {
        if (!isPlayerDead)
        {
            isBoosting = true;
            this.InvokeAction((() =>
            {
                isBoosting = false;
            }), argDuration);
            
            sfxSource.PlayOneShot(boostSound);
        }
    }

    public void KillPlayer()
    {
        if (isPlayerDead)
        {
            return;
        }
        
        isPlayerDead = true;
        isPlayerMoving = false;
        pulseAvailable = false;
        
        playerAnimator.Play("Death");
        sfxSource.PlayOneShot(deathSound);

        dollyCart.m_Speed = 0f;

        GameManager.Instance.ResetLevel();
    }

    private void PulseAttack()
    {
        if (pulseAvailable && !isPlayerDead)
        {
            puleEffectAnimator.Play("Pulse");
            pulseAvailable = false;
            this.InvokeAction((() =>
            {
                pulseAvailable = true;
            }), pulseCooldownDuration);
            sfxSource.PlayOneShot(pulseSound);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Enemy"))
        {
            KillPlayer();
        }
    }

}
