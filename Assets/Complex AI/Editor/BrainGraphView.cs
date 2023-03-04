using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public class BrainGraphView : GraphView
{
	Brain brain;
	public UnityEvent<Edge> onEdgeCreatedCallback = new();
	Dictionary<Type, UnityEvent<GraphElement>> onGraphElementRemovedCallbacks = new();
	Dictionary<Type, UnityEvent<GraphElement>> onGraphElementMovedCallbacks = new();

	public BrainGraphView(Brain brain)
	{
		InitializeBrain(brain);
		AddFeatures();
		AddStyles();

		RegisterOnElementRemovedCallback<ModuleNode>(DisableModule);
		RegisterOnElementRemovedCallback<ModuleNode>(SyncRemovedModulesWithBrain);
		graphViewChanged += OnGraphViewChanged;
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

			if(startPort.portType != port.portType && !startPort.portType.IsSubclassOf(port.portType))
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

		List<ModuleNode> mainGraphModules = new();
		for (int i = 0; i < brain.Modules.Count; i++)
		{
			ModuleNode.ModuleNodeType type = ModuleNode.ModuleNodeType.Mainstream;

			ModuleNode moduleNode = new ModuleNode(brain.Modules[i], type);
			moduleNode.SetBrain(brain);
			mainGraphModules.Add(moduleNode);
			moduleNode.SetMainStatus(true);
			AddElement(moduleNode);
			EnableModule(moduleNode);

			List<SubModuleNode> subModuleNodes = new();
			foreach (var subModule in moduleNode.Module.SubModules)
			{
				SubModuleNode subModuleNode = new(subModule);
				subModuleNodes.Add(subModuleNode);

				subModuleNode.SetBrain(brain);
				AddElement(subModuleNode);
				moduleNode.EnableSubModule(subModuleNode);
			}

			foreach(var subModuleNode in subModuleNodes)
			{
				Edge edge = new Edge();
				Port outputPort = subModuleNode.outputContainer.FirstChildOfType<Port>();
				Port inputPort = moduleNode.AddInputPort();
				edge.output = outputPort;
				edge.input = inputPort;

				outputPort.Connect(edge);
				inputPort.Connect(edge);
				AddElement(edge);
			}
		}

		if(brain.Modules.Count >= 2)
		{
			for (int i = 0; i < brain.Modules.Count - 1; i++)
			{
				Edge edge = new Edge();
				Port outputPort = mainGraphModules[i].outputContainer.FirstChildOfType<Port>();
				Port inputPort = mainGraphModules[i+1].inputContainer.FirstChildOfType<Port>();
				edge.output = outputPort;
				edge.input = inputPort;

				outputPort.Connect(edge);
				inputPort.Connect(edge);
				AddElement(edge);
			}
		}

		for (int i = 0; i < brain.UnconnectedModules.Count; i++)
		{
			ModuleNode.ModuleNodeType type = ModuleNode.ModuleNodeType.Side;

			ModuleNode moduleGraphViewer = new ModuleNode(brain.UnconnectedModules[i], type);
			moduleGraphViewer.SetBrain(brain);
			moduleGraphViewer.SetMainStatus(false);
			AddElement(moduleGraphViewer);
			EnableModule(moduleGraphViewer);
		}
	}

	void AddStyles()
	{
		StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Complex AI/Editor/Styles/Brain StyleSheet.uss");
		styleSheets.Add(styleSheet);

		StyleSheet nodestyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Complex AI/Editor/Styles/NodeStyles.uss");
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

					menuEvent.menu.AppendAction($"Modules/{type.ToString()}", (action) => CreateModuleFromType(type, action.eventInfo.localMousePosition));
				}
			}
		);

		return contextualMenuManipulator;
	}

	void CreateModuleFromType(Type type, Vector2 position)
	{
		Module module = ScriptableObject.CreateInstance(type) as Module;
		AssetDatabase.AddObjectToAsset(module, brain);
		ModuleNode moduleGraphViewer = new ModuleNode(module);
		moduleGraphViewer.SetBrain(brain);
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
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	
		moduleGraphViewer.SetMainStatus(this.graphElements.Count() == 0);
		
		AddElement(moduleGraphViewer);
		EnableModule(moduleGraphViewer);
	}

	void EnableModule(ModuleNode moduleGraphViewer)
	{
		moduleGraphViewer.OnEnable();
		moduleGraphViewer.RegisterGraphCallbacks(this);
		graphViewChanged += moduleGraphViewer.OnGraphViewChanged;
	}
	void DisableModule(GraphElement graphElement) => DisableModule(graphElement as ModuleNode);
	void DisableModule(ModuleNode moduleGraphViewer)
	{
		graphViewChanged -= moduleGraphViewer.OnGraphViewChanged;
		moduleGraphViewer.UnRegisterGraphCallbacks(this);
		moduleGraphViewer.OnDisable();
	}
	
	public GraphViewChange OnGraphViewChanged(GraphViewChange changes)
	{
		if(changes.edgesToCreate is not null)
		{
			foreach (var edgeToCreate in changes.edgesToCreate)
			{
				onEdgeCreatedCallback?.Invoke(edgeToCreate);
			}
		}

		if(changes.elementsToRemove is not null)
		{
			foreach (var elementToRemove in changes.elementsToRemove)
			{
				foreach (var Callback in onGraphElementRemovedCallbacks)
				{
					if(elementToRemove.GetType() == Callback.Key)
					{
						Callback.Value?.Invoke(elementToRemove);
					}
				}
			}
		}

		if(changes.movedElements is not null)
		{
			foreach (var movedElement in changes.movedElements)
			{
				foreach (var Callback in onGraphElementMovedCallbacks)
				{
					if(movedElement.GetType() == Callback.Key)
					{
						Callback.Value?.Invoke(movedElement);
					}
				}
			}
		}

		return changes;
	}

	void SyncRemovedModulesWithBrain(GraphElement elementToRemove)
	{
		if(elementToRemove is not ModuleNode moduleNode)
		{
			return;
		}

		brain.Remove(moduleNode.Module);
	}

	#region Registration
	public void RegisterOnElementRemovedCallback<T>(UnityAction<GraphElement> action) where T: GraphElement
	{
		if(!onGraphElementRemovedCallbacks.ContainsKey(typeof(T)))
		{
			onGraphElementRemovedCallbacks[typeof(T)] = new();
		}

		onGraphElementRemovedCallbacks[typeof(T)].AddListener(action);
	}

	public void UnRegisterOnElementRemovedCallback<T>(UnityAction<GraphElement> action) where T: GraphElement
	{
		if(!onGraphElementRemovedCallbacks.ContainsKey(typeof(T)))
		{
			return;
		}

		onGraphElementRemovedCallbacks[typeof(T)].RemoveListener(action);
	}

	public void RegisterOnElementMovedCallback<T>(UnityAction<GraphElement> action) where T: GraphElement
	{
		if(!onGraphElementMovedCallbacks.ContainsKey(typeof(T)))
		{
			onGraphElementMovedCallbacks[typeof(T)] = new();
		}

		onGraphElementMovedCallbacks[typeof(T)].AddListener(action);
	}

	public void UnRegisterOnElementMovedCallback<T>(UnityAction<GraphElement> action) where T: GraphElement
	{
		if(!onGraphElementMovedCallbacks.ContainsKey(typeof(T)))
		{
			return;
		}

		onGraphElementMovedCallbacks[typeof(T)].RemoveListener(action);
	}
	#endregion
}
