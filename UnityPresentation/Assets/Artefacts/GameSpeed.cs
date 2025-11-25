using TMPro;
using UnityEngine;

public class GameSpeed : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI number1;
	[SerializeField] private TextMeshProUGUI number2;
	public void OnValueChange(float value)
	{
		Time.timeScale = value / 10;

		string text = (value / 10).ToString("0.0");
		number1.text = text;
		number2.text = text;
	}


	private void OnDisable()
	{
		Time.timeScale = 1.0f;
	}
}
