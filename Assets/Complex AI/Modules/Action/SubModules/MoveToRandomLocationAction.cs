using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToRandomLocationAction : ActionSubModule
{
	public override Type[] RequiredDataTypes => Array.Empty<Type>();

	public override bool CanPerform(params Data[] data) => true;

	public override void Process(params Data[] data){}

	public override float WouldPerform(params Data[] data) => 1;
}
