using UnityEngine;

namespace Presentation
{
	public class Slide : MonoBehaviour
	{
		private void OnDrawGizmos()
		{
			Camera cam = Camera.main;
			Vector3 size, position;
			size.y = cam.orthographicSize * 2;
			size.x = size.y * cam.aspect;
			size.z = cam.farClipPlane - cam.nearClipPlane;

			position = transform.position + Presentation.Instance.defaultCameraTransform.position;
			position.z += size.z * 0.5f;

			Gizmos.DrawWireCube(position, size);
		}
	}
}