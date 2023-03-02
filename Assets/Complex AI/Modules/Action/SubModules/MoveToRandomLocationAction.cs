using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToRandomLocationAction : ActionSubModule
{
	public override bool CanPerform(params IData[] data) => true;

	public override List<IData> Process(params IData[] data) => default;

	public override float WouldPerform(params IData[] data) => 1;
}
