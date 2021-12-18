using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	[SerializeField] private float initialSpeed = 4f;
	[SerializeField] private float acceleration = 0.5f;
	[SerializeField] private float maxSpeed = 10f;
	[SerializeField] private Rigidbody _rigidbody;
	[SerializeField] private Animator animator;
	
	[Space] 
	[SerializeField] private AudioSource sfxSource;
	[SerializeField] private AudioClip aggroSound;
	[SerializeField] private AudioClip deathSound;
	
	private bool isAggrod = false;
	private bool isDead = false;
	private float currentSpeed = 0f;

	private void Awake()
	{
		isDead = false;
		isAggrod = false;
		_rigidbody.useGravity = false;
		_rigidbody.isKinematic = true;
	}

	private void Update()
	{
		if (isAggrod && !isDead && PlayerController.Instance && !PlayerController.Instance.isPlayerDead)
		{
			transform.position = Vector3.MoveTowards(transform.position, PlayerController.Instance.transform.position, currentSpeed * Time.deltaTime);

			currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		// Aggro Trigger
		if (!isAggrod &&other.attachedRigidbody && other.attachedRigidbody.CompareTag("Player"))
		{
			Aggro();
		}
	}

	private void Aggro()
	{
		animator.Play("Aggro");
		sfxSource.PlayOneShot(aggroSound);
		this.InvokeAction((() =>
		{
			isAggrod = true;
			currentSpeed = initialSpeed;
		}), 0.15f);
	}

	public void Kill()
	{
		isDead = true;
		animator.Play("Death");
		_rigidbody.isKinematic = false;
		
		sfxSource.PlayOneShot(deathSound);
		
		// this.InvokeAction((() =>
		// {
		// 	this.gameObject.SetActive(false);
		// }), 35f/60f);
	}
	
}