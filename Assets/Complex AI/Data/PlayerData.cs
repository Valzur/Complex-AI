using System;

public struct PlayerData : IData
{
	public float LastUpdated { get; set; }
	public Guid GUID { get; set; }
}