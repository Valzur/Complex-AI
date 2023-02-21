using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

	void InitializeBrain(Brain brain)
	{
		this.brain = brain;

		for (int i = 0; i < brain.Modules.Length; i++)
		{
			ModuleGraphViewer.ModuleNodeType type = ModuleGraphViewer.ModuleNodeType.Middle;
			if(i == 0)
			{
				type = ModuleGraphViewer.ModuleNodeType.Start;
			}
			else if(i == brain.Modules.Length)
			{
				type = ModuleGraphViewer.ModuleNodeType.End;
			}

			ModuleGraphViewer moduleGraphViewer = new ModuleGraphViewer(brain.Modules[i], type);
			AddElement(moduleGraphViewer);
		}
	}
	void AddStyles()
	{
		StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Complex AI/Styles/Brain StyleSheet.uss");
		styleSheets.Add(styleSheet);
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
		this.AddManipulator(CreateContextualMenuModules());
	}

	IManipulator CreateContextualMenuModules()
	{
		
		ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
			menuEvent => 
			{
				foreach (var type in typeof(Module).Assembly.GetTypes())
				{
					menuEvent.menu.AppendAction($"Add {type.ToString()}", );
				}
			}
		);

		return contextualMenuManipulator;
		ModuleGraphViewer moduleGraphViewer = new ModuleGraphViewer();
	}

	void CreateModuleFromType(System.Type type)
	{
		
	}
}
