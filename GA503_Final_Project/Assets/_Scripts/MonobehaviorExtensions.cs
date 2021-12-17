
using System;
using System.Collections;
using UnityEngine;

public static class MonobehaviorExtensions
{
	public static void InvokeAction(this MonoBehaviour invokedOn, Action argAction, float argDelay)
	{
		if (argDelay <= 0f)
		{
			argAction.Invoke();
		}
		else
		{
			invokedOn.StartCoroutine(CoInvokeAction(argAction, argDelay));
		}
	}

	private static IEnumerator CoInvokeAction(Action argAction, float argDelay)
	{
		yield return new WaitForSeconds(argDelay);
		
		argAction.Invoke();
	}
}
