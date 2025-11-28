using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] private GameObject playersPrefab;
	[SerializeField] private GameObject players;
	[SerializeField] private TextMeshProUGUI timerText;
	[SerializeField] private TextMeshProUGUI scoreText;
	[SerializeField] private float time;
	private float timer;

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
		players = Instantiate(playersPrefab, transform.parent);

		switch (winnerIndex)
		{
			case 0: Score_P1++; break;
			case 1: Score_P2++; break;
		}

		scoreText.text = Score_P1.ToString() + " : " + Score_P2.ToString();
	}
}
