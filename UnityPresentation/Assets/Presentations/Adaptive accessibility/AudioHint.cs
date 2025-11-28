using UnityEngine;

public class AudioHint : MonoBehaviour
{
	[SerializeField] private Transform t1;    
	[SerializeField] private Transform t2;    
	[SerializeField] private Vector2 mapRange;
	[SerializeField] private float minVolume;
	[SerializeField] private float maxVolume;

	private AudioSource source;

	private void OnEnable()
	{
		source = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()
    {
		float dist = (t2.position - t1.position).magnitude;
		float factor = 1 - Mathf.InverseLerp(mapRange.x, mapRange.y, dist);
		source.volume = Mathf.Lerp(minVolume, maxVolume, factor);
		source.pitch = 0.5f + (factor * 0.5f);
    }
}
