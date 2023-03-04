using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Data : IEqualityComparer
{
	[SerializeField] public float LastUpdated { get; protected set; }
	protected Guid GUID { get; set; } = Guid.NewGuid();


	bool IEqualityComparer.Equals(object thisObject, object otherObject)
	{
		if(otherObject is Data dataObject)
		{
			return dataObject.GetHashCode() == GetHashCode();
		}
		
		throw new Exception();
	}

	int IEqualityComparer.GetHashCode(object obj) => HashCode.Combine(LastUpdated, GUID);

	public void Update() => LastUpdated = Time.time;
}