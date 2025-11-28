using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] private GameObject playersPrefab;
	[SerializeField] private GameObject players;
	[SerializeField] private CinemachineCamera cinemachineCamera;
	[SerializeField] private TextMeshProUGUI timerText;
	[SerializeField] private TextMeshProUGUI scoreText;
	[SerializeField] private float time;
	private float timer;

	[SerializeField, Tooltip("Below which percentage is accessibility triggered for P1")] private float accessibilityTrigger_P1;
	[SerializeField, Tooltip("Below which percentage is accessibility triggered for P2")] private float accessibilityTrigger_P2;

	[Header("Accessibility P1")]
	[SerializeField] private float cheeseScale;

	[Header("Accessibility P2")]
	[SerializeField] private float ietalcsoa;

	public int Score_P1 {  get; private set; }
    public int Score_P2 {  get; private set; }

	public int Score_Total { get { return Score_P1 + Score_P2; } }
	public float winRate_P1 { get { return Score_P1 / (float)Score_Total; } }
	public float winRate_P2 { get { return Score_P2 / (float)Score_Total; } }

	private void IncrementPlayer1Score()
	{
		Score_P1 += 1;
	}

	private void IncrementPlayer2Score()
	{
		Score_P2 += 1;
	}

	private void Start()
	{
		timer = time;
		ActivateAccessibilityForP1();
		ActivateAccessibilityForP2();
	}

	private void Update()
	{
		timer -= Time.deltaTime;
		timerText.text = timer.ToString(".0") + " s";
		if(timer <= 0)
		{
			timer = time;
			ResetGame(1);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="winnerIndex">0 == hunter, 1 == cheese</param>
	public void ResetGame(int winnerIndex)
	{
		Destroy(players);
		switch (winnerIndex)
		{
			case 0: Score_P1++; break;
			case 1: Score_P2++; break;
		}

		players = Instantiate(playersPrefab, transform.parent);
		cinemachineCamera.Target.TrackingTarget = players.transform.GetChild(0);
		cinemachineCamera.Target.LookAtTarget = players.transform.GetChild(1);

		scoreText.text = Score_P1.ToString() + " : " + Score_P2.ToString();

		if (winRate_P1 <= accessibilityTrigger_P1)
			ActivateAccessibilityForP1();
		if (winRate_P2 <= accessibilityTrigger_P2)
			ActivateAccessibilityForP2();
	}

	private void ActivateAccessibilityForP1()
	{
		GameObject cheese = players.transform.GetChild(1).gameObject;
		GameObject arrow = players.transform.GetChild(2).gameObject;
		GameObject audioHint = players.transform.GetChild(3).gameObject;

		cheese.transform.localScale *= cheeseScale;
		cheese.GetComponent<ParticleSystem>().Play();
		arrow.SetActive(true);
		audioHint.SetActive(true);
	}

	private void ActivateAccessibilityForP2()
	{

	}
}
