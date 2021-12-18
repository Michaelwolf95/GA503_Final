using System;
using UnityEngine;

public class PulseDamageTrigger : MonoBehaviour
{
	[SerializeField] private float explosionForce = 8f;
	
	private void OnTriggerEnter(Collider other)
	{
		if (!other.isTrigger && other.attachedRigidbody && other.attachedRigidbody.CompareTag("Enemy"))
		{
			EnemyController enemyController = other.attachedRigidbody.GetComponent<EnemyController>();
			enemyController?.Kill();
			other.attachedRigidbody.AddExplosionForce(explosionForce, this.transform.position, this.transform.localScale.x * 2f);
		}
	}
}