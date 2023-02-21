using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Memorizer
{
	Dictionary<Type, IData> AllData = new();

	public void Memorize(IData data) => AllData[typeof(IData)] = data;
	public void Memorize(List<IData> data)
	{
		foreach (var item in data)
		{
			Memorize(data);
		}
	}

	public IData FindDataOfType(IData dataType)
	{
		Type type = dataType.GetType();
		if(!AllData.ContainsKey(type))
		{
			return dataType;
		}

		return AllData[type];
	}
	public List<IData> FindDataOfType(params IData[] dataTypes)
	{
		List<IData> dataList = new();
		
		foreach (IData dataType in dataTypes)
		{
			dataList.Add(FindDataOfType(dataType));
		}

		return dataList;
	}
}
