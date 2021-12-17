using System;
using UnityEngine;

public class BoostGate : MonoBehaviour
{
	[SerializeField] private float boostDuration = 2.5f;
	
	private bool used = false;
	
	private void OnTriggerEnter(Collider other)
	{
		if (!used && other.attachedRigidbody && other.attachedRigidbody.CompareTag("Player"))
		{
			PlayerController.Instance.StartBoost(boostDuration);
			used = true;
		}
	}
}