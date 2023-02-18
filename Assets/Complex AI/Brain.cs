using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
	[SerializeField] float updateRate;
	Module[] Modules;
	Memorizer Memory;
	Coroutine UpdateRoutineReference;
	void Awake()
	{
		foreach (var module in Modules)
		{
			module.Initialize(Memory);
		}
	}
	void OnEnable() => Activate();
	void OnDisable() => DeActivate();

	IEnumerator UpdateRoutine()
	{
		var Awaiter = new WaitForSecondsRealtime(updateRate);

		while(true)
		{
			foreach (var module in Modules)
			{
				module.Process();
			}

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
