using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
	[SerializeField] IData CurrentData = new PlayerData();
	[SerializeField] List<IData> dataSet = new();

	[ContextMenu("Add Current")]
	void AddFirst()
	{
		dataSet.Add(CurrentData);
	}

	[ContextMenu("Testoo")]
	void TestThis()
	{
		Debug.Log(dataSet.Contains(CurrentData));
	}
}