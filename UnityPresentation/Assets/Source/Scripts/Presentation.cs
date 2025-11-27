using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Presentation
{
	[InitializeOnLoad]
	public class Presentation : MonoBehaviour
	{
		public static Presentation Instance 
		{ get
			{
				if (instance == null)
					InitialiseSingleton();
				return instance;
			}
			private set
			{
				instance = value;
			}
		}

		private static Presentation instance;

		public Transform defaultCameraTransform;

		private int activeSlideIndex;
		private GameObject activeSlide;
		private Camera MainCam 
		{
			get
			{
				if(mainCam == null && Camera.main != null)
					mainCam = Camera.main;
				return mainCam;
			}
		}
		private Camera mainCam;

		[SerializeField, Tooltip("Editor Only Variable"), Min(0)] private int previewSlideIndex;
		[SerializeField, Tooltip("Editor Only Variable")] bool startFromBeginning;

		private void OnValidate()
		{
			if (previewSlideIndex != activeSlideIndex && previewSlideIndex < transform.childCount)
			{
				activeSlideIndex = previewSlideIndex;
				UpdatePresentationEditor();
			}
		}

		public static void InitialiseSingleton()
		{
			if (instance != null) return;
			Presentation[] presentations = FindObjectsByType<Presentation>(FindObjectsSortMode.None);
			if(presentations.Length == 0)
			{
				throw new System.Exception("There is no presentation in this scene");
			}
			if(presentations.Length > 1)
			{
				throw new System.Exception("There is more than one presentation in this scene");
			}
			
			instance = presentations[0];
		}

		private void Awake()
		{
			Instance = this;

			foreach (Transform child in transform)
			{
				child.gameObject.SetActive(false);
			}
		}

		private void Start()
		{
			MainCam.transform.position = defaultCameraTransform.position;
			MainCam.transform.rotation = defaultCameraTransform.rotation;
			MainCam.transform.localScale = defaultCameraTransform.localScale;

#if UNITY_EDITOR
			if (startFromBeginning)
				activeSlideIndex = 0;
			else
				activeSlideIndex = Mathf.Min(previewSlideIndex, transform.childCount - 1);
#else
		activeSlideIndex = 0;
#endif
			activeSlide = transform.GetChild(activeSlideIndex).gameObject;
			UpdatePresentation();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				GoToPreviousSlide();
			}

			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				GoToNextSlide();
			}
		}

		public void GoToNextSlide(InputAction.CallbackContext context)
		{
			if (!context.performed) return;

			GoToNextSlide();
		}

		public void GoToNextSlide()
		{
			if (activeSlideIndex + 1 >= transform.childCount) return;
			activeSlideIndex++;
			UpdatePresentation();
		}

		public void GoToPreviousSlide(InputAction.CallbackContext context)
		{
			if (!context.performed) return;

			GoToPreviousSlide();
		}

		public void GoToPreviousSlide()
		{
			if (activeSlideIndex <= 0) return;
			activeSlideIndex--;
			UpdatePresentation();
		}

		private void UpdatePresentation()
		{
			Survivor[] survivors = activeSlide.GetComponentsInChildren<Survivor>();
			GameObject newActiveSlide = transform.GetChild(activeSlideIndex).gameObject;
			foreach (Survivor survivor in survivors)
			{
				if(survivor.startSlide <= activeSlideIndex && survivor.lastSlide >= activeSlideIndex)
					survivor.transform.SetParent(newActiveSlide.transform, false);
			}

			activeSlide.SetActive(false);
			UpdatePresentationEditor();
			activeSlide.SetActive(true);
		}

		public void UpdatePresentationEditor()
		{
			transform.position = -transform.GetChild(activeSlideIndex).localPosition;
			activeSlide = transform.GetChild(activeSlideIndex).gameObject;
			MainCam.orthographic = !activeSlide.GetComponent<Slide>().isCameraPerspective;
			if(activeSlide.GetComponentInChildren<Camera>() != null)
				MainCam.gameObject.SetActive(false);
			else
				MainCam.gameObject.SetActive(true);
		}

		public void UpdatePresentationEditor(int slideIndex)
		{
			previewSlideIndex = slideIndex;
			activeSlideIndex = slideIndex;
			UpdatePresentationEditor();
		}
	}
}