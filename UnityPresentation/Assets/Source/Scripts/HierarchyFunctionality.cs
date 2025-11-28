using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Presentation
{

	[InitializeOnLoad]
	public class HierarchyFunctionality
	{
		public static float slideDistance = 20;

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

		private static Click lastClick;

		static HierarchyFunctionality()
		{
			lastClick = new Click(double.NaN, -1);
			EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
			ObjectChangeEvents.changesPublished += OnPublish;
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
				int slideIndex = clicked.transform.GetSiblingIndex();
				Presentation.Instance.UpdatePresentationEditor(slideIndex);
				PlayerPrefs.SetInt("slideIndex", slideIndex);
			}

			lastClick = new Click(time, instanceID);
		}

		static void OnPublish(ref ObjectChangeEventStream stream)
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode) return;

			for (int i = 0; i < stream.length; i++)
			{
				ObjectChangeKind type = stream.GetEventType(i);
				GameObject gameObject;
				switch(type)
				{
					case ObjectChangeKind.CreateGameObjectHierarchy:
						stream.GetCreateGameObjectHierarchyEvent(i, out CreateGameObjectHierarchyEventArgs createdEvent);
						gameObject = EditorUtility.InstanceIDToObject(createdEvent.instanceId) as GameObject;

						if (gameObject == null) continue;
						if (gameObject.GetComponent<Slide>() == null) continue;
						UpdateSlidePositions();
						break;
					case ObjectChangeKind.DestroyGameObjectHierarchy:
						//It seems the game object is destroyed already, so I can't check if it was a slide or not
						UpdateSlidePositions();
						break;
				}
			}
		}

		static void PlaceSlide(GameObject slide)
		{
			if (slide.transform.parent != Presentation.Instance.transform)
				slide.transform.SetParent(Presentation.Instance.transform);
			SetSlidePosition(slide);

			Debug.Log($"Created \"{slide}\" with slide index {slide.transform.GetSiblingIndex()}");
		}

		static void UpdateSlidePositions()
		{
			foreach(Transform slide in Presentation.Instance.transform)
			{
				SetSlidePosition(slide.gameObject);
			}
		}

		static void SetSlidePosition(GameObject slide)
		{
			slide.transform.localPosition = new Vector3(slide.transform.GetSiblingIndex() * slideDistance, 0, 0);
		}
	}
}