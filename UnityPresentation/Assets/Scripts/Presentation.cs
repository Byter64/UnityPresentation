using UnityEngine;
using UnityEngine.InputSystem;

namespace Presentation
{
	public class Presentation : MonoBehaviour
	{
		public static Presentation Instance { get; private set; }

		public Transform defaultCameraTransform;

		private int activeSlideIndex;
		private GameObject activeSlide;

		[SerializeField, Tooltip("Editor Only Variable"), Min(0)] private int viewSlideWithIndex;
		[SerializeField, Tooltip("Editor Only Variable")] bool startFromBeginning;

		private void OnValidate()
		{
			Instance = this;

			if (viewSlideWithIndex != activeSlideIndex && viewSlideWithIndex < transform.childCount)
			{
				activeSlideIndex = viewSlideWithIndex;
				UpdatePresentationEditor();
			}
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
			Camera.main.transform.position = defaultCameraTransform.position;
			Camera.main.transform.rotation = defaultCameraTransform.rotation;
			Camera.main.transform.localScale = defaultCameraTransform.localScale;

#if UNITY_EDITOR
			if (startFromBeginning)
				activeSlideIndex = 0;
			else
				activeSlideIndex = Mathf.Min(viewSlideWithIndex, transform.childCount - 1);
#else
		activeSlideIndex = 0;
#endif
			activeSlide = transform.GetChild(activeSlideIndex).gameObject;
			UpdatePresentation();
		}

		public void GoToNextSlide(InputAction.CallbackContext context)
		{
			if (!context.performed) return;
			if (activeSlideIndex + 1 >= transform.childCount) return;

			activeSlideIndex++;
			UpdatePresentation();
		}

		public void GoToPreviousSlide(InputAction.CallbackContext context)
		{
			if (!context.performed) return;
			if (activeSlideIndex <= 0) return;

			activeSlideIndex--;
			UpdatePresentation();
		}

		private void UpdatePresentation()
		{
			activeSlide.SetActive(false);
			transform.position = -transform.GetChild(activeSlideIndex).localPosition;
			activeSlide = transform.GetChild(activeSlideIndex).gameObject;
			activeSlide.SetActive(true);
		}

		private void UpdatePresentationEditor()
		{
			transform.position = -transform.GetChild(activeSlideIndex).localPosition;
			activeSlide = transform.GetChild(activeSlideIndex).gameObject;
		}
	}
}