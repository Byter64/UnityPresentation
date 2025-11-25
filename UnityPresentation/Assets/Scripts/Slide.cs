using UnityEngine;

public class Slide : MonoBehaviour
{
	private void OnDrawGizmos()
	{
		Camera cam = Camera.main;
		Gizmos.DrawFrustum(transform.position + Presentation.Instance.defaultCameraTransform.position, cam.fieldOfView, cam.farClipPlane, cam.nearClipPlane, cam.aspect);
	}
}
