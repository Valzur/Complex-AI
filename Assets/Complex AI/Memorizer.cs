using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Memorizer
{
	[SerializeReference] List<Data> AllData = new();

	public void Memorize(Data newData)
	{
		if(AllData.Exists((data) => data.GetType() == newData.GetType()))
		{
			Debug.LogError($"You should not be aading more than once instance of data, attempted to add: {newData.GetType()}");
			return;
		}

		newData.Update();
		AllData.Add(newData);
	}
	public void Memorize(List<Data> allData)
	{
		foreach (var data in allData)
		{
			Memorize(data);
		}
	}

	public Data FindDataOfType(Type dataType)
	{
		Data foundData = AllData.FirstOrDefault((data) => data.GetType() == dataType);
		if(foundData == default)
		{
			Memorize(System.Activator.CreateInstance(dataType) as Data);
		}

		return foundData;
	}

	public List<Data> FindDataOfType(params Type[] dataTypes)
	{
		List<Data> dataList = new();
		
		foreach (Data data in AllData)
		{
			bool isDataNeeded = false;
			foreach (Type dataType in dataTypes)
			{
				if(data.GetType() == dataType)
				{
					isDataNeeded = true;
					break;
				}
			}

			if(isDataNeeded)
			{
				dataList.Add(data);
			}
		}

		if(dataList.Count != dataTypes.Length)
		{
			List<Type> missingTypes = new();

			foreach (var type in dataTypes)
			{
				if(!dataList.Exists((data) => data.GetType() == type))
				{
					missingTypes.Add(type);
				}
			}

			foreach (var missingType in missingTypes)
			{
				Data newData = System.Activator.CreateInstance(missingType) as Data;
				dataList.Add(newData);
				AllData.Add(newData);
			}
		}

		List<Data> orderedDataList = new();
		foreach (var type in dataTypes)
		{
			orderedDataList.Add(dataList.First((data) => data.GetType() == type));
		}

		dataList = orderedDataList;

		return dataList;
	}
}
