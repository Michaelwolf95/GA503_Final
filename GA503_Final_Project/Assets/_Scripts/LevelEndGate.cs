using System;
using UnityEngine;

public class LevelEndGate : MonoBehaviour
{
	private bool isTriggered = false;
	
	private void OnTriggerEnter(Collider other)
	{
		if (!isTriggered && other.attachedRigidbody && other.attachedRigidbody.CompareTag("Player"))
		{
			GameManager.Instance.CompleteLevel();
		}
	}
}