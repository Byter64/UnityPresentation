using UnityEngine;
using UnityEngine.UIElements;

namespace Presentation
{
	public class Slide : MonoBehaviour
	{
		public bool isCameraPerspective;
		private void OnDrawGizmos()
		{
			Camera cam = Camera.main;
			Vector3 position = transform.position + Presentation.Instance.defaultCameraTransform.position;

			if (isCameraPerspective)
			{
				Gizmos.DrawFrustum(position, cam.fieldOfView, cam.farClipPlane, cam.nearClipPlane, cam.aspect);
			}
			else
			{
				Vector3 size;
				size.y = cam.orthographicSize * 2;
				size.x = size.y * cam.aspect;
				size.z = cam.farClipPlane - cam.nearClipPlane;

				position.z += size.z * 0.5f;

				Gizmos.DrawWireCube(position, size);
			}
		}
	}
}