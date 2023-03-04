using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BrainMono: MonoBehaviour
{
	[Tooltip("Use a rate of Infinity for every frame execution")]
	[SerializeField] float updateRate;
	public Brain brain;
	Coroutine UpdateRoutineReference;
	UnityEvent updateEveryFrame = new();

	void Awake()
	{
		brain.InitializeModules(transform);
	}
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

	void Update() => updateEveryFrame?.Invoke();

	void DeActivate()
	{
		if((UpdateRoutineReference is null))
		{
			return;
		}

		if(updateRate == Mathf.Infinity)
		{
			updateEveryFrame.RemoveAllListeners();
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

		if(updateRate == Mathf.Infinity)
		{
			updateEveryFrame.AddListener(() => brain.UpdateTick());
			return;
		}

		UpdateRoutineReference = StartCoroutine(UpdateRoutine());
	}
}