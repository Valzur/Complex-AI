using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public static partial class Utility
{
	public static List<T> ChildrenOfType<T>(this VisualElement visualElement) where T: VisualElement
	{
		List<T> visualElements = new();
		foreach (var element in visualElement.Children())
		{
			if(element is T castedElement)
			{
				visualElements.Add(castedElement);
			}
		}

		return visualElements;
	}

	public static T FirstChildOfType<T>(this VisualElement visualElement) where T: VisualElement => ChildrenOfType<T>(visualElement).FirstOrDefault();

	public static T FirstParentOfType<T>(this VisualElement visualElement) where T: VisualElement
	{
		VisualElement parent = visualElement.parent;
		while(parent is not T)
		{
			parent = parent.parent;
		}

		return parent as T;
	}

	public static bool ContainsElementOfType(this VisualElement visualElement, Type type)
	{
		foreach (var child in visualElement.Children())
		{
			if(child.GetType() == type)
				return true;
		}

		return false;
	}

	public static Node InputOwnerNode(this Edge edge)
	{
		return edge.input.parent.parent.parent.parent.parent as Node;
	}

	public static Node OutputOwnerNode(this Edge edge)
	{
		return edge.output.parent.parent.parent.parent.parent as Node;
	}
}