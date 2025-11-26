using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Presentation
{
	[InitializeOnLoad]
	public class HierarchyFunctionality
	{
		private struct Click
		{
			public static double doubleClickThreshold = 0.5f;
			public double time;
			public int instanceID;

			public Click(double time, int instanceID)
			{
				this.time = time;
				this.instanceID = instanceID;
			}
		}

		private static Presentation Presentation
		{ get
			{
				if (Presentation.Instance == null)
					Presentation.InitialiseSingleton();
				return Presentation.Instance;
			} 
		}

		private static Click lastClick;

		static HierarchyFunctionality()
		{
			lastClick = new Click(double.NaN, -1);
			EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
		}

		static void OnGUI(int instanceID, Rect selectionRect)
		{
			Event e = Event.current;
			if (!(e.type == EventType.MouseDown && selectionRect.Contains(e.mousePosition))) return;

			GameObject clicked = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
			if (clicked == null) return;
			if (clicked.GetComponent<Slide>() == null) return;

			double time = EditorApplication.timeSinceStartup;
			if(time - lastClick.time < Click.doubleClickThreshold && instanceID == lastClick.instanceID)
			{
				Presentation.UpdatePresentationEditor(clicked.transform.GetSiblingIndex());
			}

			lastClick = new Click(time, instanceID);
		}
	}
}