using UnityEngine;

public class HelperArrow : MonoBehaviour
{
	[SerializeField] private float orbitRadius;
	[SerializeField] private Transform target;
	[SerializeField] private Transform orbitCenter;

    // Update is called once per frame
    void FixedUpdate()
    {
		Vector3 dist = target.position - orbitCenter.position;
        transform.position = orbitCenter.position + dist.normalized * orbitRadius;
		transform.LookAt(target);
    }
}
