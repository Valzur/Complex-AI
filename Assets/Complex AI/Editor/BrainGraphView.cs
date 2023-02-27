using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public class BrainGraphView : GraphView
{
	Brain brain;
	public BrainGraphView(Brain brain)
	{
		InitializeBrain(brain);
		AddFeatures();
		AddStyles();
	}

	public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
	{
		List<Port> compatiblePorts = new();
		
		foreach (var port in ports)
		{
			if(startPort.node == port.node)
			{
				continue;
			}

			if(startPort.direction == port.direction)
			{
				continue;
			}

			compatiblePorts.Add(port);
		}

		return compatiblePorts;
	}

	void InitializeBrain(Brain brain)
	{
		this.brain = brain;

		for (int i = 0; i < brain.Modules.Count; i++)
		{
			ModuleGraphViewer.ModuleNodeType type = ModuleGraphViewer.ModuleNodeType.Mainstream;

			ModuleGraphViewer moduleGraphViewer = new ModuleGraphViewer(brain.Modules[i], type);
			moduleGraphViewer.SetMainStatus(true);
			AddElement(moduleGraphViewer);
		}

		for (int i = 0; i < brain.UnconnectedModules.Count; i++)
		{
			ModuleGraphViewer.ModuleNodeType type = ModuleGraphViewer.ModuleNodeType.Side;

			ModuleGraphViewer moduleGraphViewer = new ModuleGraphViewer(brain.UnconnectedModules[i], type);
			moduleGraphViewer.SetMainStatus(false);
			AddElement(moduleGraphViewer);
		}
	}
	
	void AddStyles()
	{
		StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Complex AI/Styles/Brain StyleSheet.uss");
		styleSheets.Add(styleSheet);

		StyleSheet nodestyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Complex AI/Styles/NodeStyles.uss");
		styleSheets.Add(nodestyleSheet);
	}

	void AddFeatures()
	{
		GridBackground gridBackground = new GridBackground();
		Insert(0, gridBackground);
		gridBackground.StretchToParentSize();

		SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
		
		this.AddManipulator(new ContentDragger());
		this.AddManipulator(new ContentZoomer());
		this.AddManipulator(new EdgeManipulator());
		this.AddManipulator(new SelectionDragger());
		this.AddManipulator(new ClickSelector());
		this.AddManipulator(new RectangleSelector());
		this.AddManipulator(CreateContextualMenuModules());
	}

	IManipulator CreateContextualMenuModules()
	{
		ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
			menuEvent => 
			{
				foreach (var type in typeof(Module).Assembly.GetTypes())
				{
					if(!type.IsSubclassOf(typeof(Module)))
					{
						continue;
					}

					menuEvent.menu.AppendAction($"Add {type.ToString()}", (action) => CreateModuleFromType(type, action.eventInfo.localMousePosition));
				}
			}
		);

		return contextualMenuManipulator;
	}

	void CreateModuleFromType(Type type, Vector2 position)
	{
		Module module = Activator.CreateInstance(type) as Module;
		ModuleGraphViewer moduleGraphViewer = new ModuleGraphViewer(module);
		moduleGraphViewer.Initialize(position);
		
		if(this.graphElements.Count() == 0)
		{
			brain.Modules.Add(module);
		}
		else
		{
			brain.UnconnectedModules.Add(module);
		}
		EditorUtility.SetDirty(brain);
	
		moduleGraphViewer.SetMainStatus(this.graphElements.Count() == 0);
		
		AddElement(moduleGraphViewer);
		graphViewChanged += moduleGraphViewer.OnGraphViewChanged;
	}
}
