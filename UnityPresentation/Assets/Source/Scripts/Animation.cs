using UnityEngine;

namespace Presentation
{
	public class Animation : MonoBehaviour
	{
		public bool playAutomatically;
		[Tooltip("If playAutomatically is true, this is the time from the last input/animation until this one starts")] public float automaticDelay;
	}
}