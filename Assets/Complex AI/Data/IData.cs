using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IData : IEqualityComparer
{
	float LastUpdated { get; set; }
	Guid GUID { get; set; }

	bool IEqualityComparer.Equals(object thisObject, object otherObject)
	{
		if(otherObject is IData dataObject)
		{
			return dataObject.GetHashCode() == GetHashCode();
		}
		
		throw new Exception();
	}

	int IEqualityComparer.GetHashCode(object obj) => HashCode.Combine(LastUpdated, GUID);

	void Update() => LastUpdated = Time.time;
}