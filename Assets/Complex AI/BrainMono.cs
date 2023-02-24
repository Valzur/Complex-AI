using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainMono: MonoBehaviour
{
	[SerializeField] float updateRate;
	public Brain brain;
	Coroutine UpdateRoutineReference;

	void Awake() => brain.InitializeModules();
	void OnEnable() => Activate();
	void OnDisable() => DeActivate();

	IEnumerator UpdateRoutine()
	{
		var Awaiter = new WaitForSecondsRealtime(updateRate);

		while(true)
		{
			brain.UpdateTick();

			yield return Awaiter;
		}
	}

	void DeActivate()
	{
		if((UpdateRoutineReference is null))
		{
			return;
		}

		StopCoroutine(UpdateRoutineReference);
	}
	
	void Activate()
	{
		if((UpdateRoutineReference is not null))
		{
			return;
		}

		UpdateRoutineReference = StartCoroutine(UpdateRoutine());
	}
}