using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor;

public static partial class Utility
{
	public static List<VisualElement> ChildrenOfType<T>(this VisualElement visualElement) where T: VisualElement
	{
		List<VisualElement> visualElements = new();
		foreach (var element in visualElement.Children())
		{
			if(element is T)
			{
				visualElements.Add(element);
			}
		}

		return visualElements;
	}

	public static VisualElement FirstChildOfType<T>(this VisualElement visualElement) where T: VisualElement => ChildrenOfType<T>(visualElement).FirstOrDefault();
}