using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	SpawnerController spawner = null;

	[SerializeField, Tooltip("Time of a Round in seconds")] float roundTime = 150.0f;
	[SerializeField] Collector player = null;
	[SerializeField] Text timerText = null;
	[SerializeField] Canvas pauseMenuCanvas = null;
	[SerializeField] Canvas gameOverCanvas = null;

	bool gameIsPaused = false;

	float itemSpawnCooldown = 99.0f;
	const float MIN_SPAWNCOOLDOWN_ITEM = 3.0f;
	const float MAX_SPAWNCOOLDOWN_ITEM = 7.0f;

	float friendlySpawnCooldown = 99.0f;
	const float MIN_SPAWNCOOLDOWN_FRIENDLY = 7.5f;
	const float MAX_SPAWNCOOLDOWN_FRIENDLY = 15.0f;

	List<CarController> enemies = null;
	const int MIN_NUMBEROFENEMIES = 7;

	public int Score { get { return score; } set { score = value; } }
	int score = 0;


	// Use this for initialization
	void Start()
	{
		spawner = GetComponent<SpawnerController>();
		itemSpawnCooldown = Random.Range(MIN_SPAWNCOOLDOWN_ITEM, MAX_SPAWNCOOLDOWN_ITEM);
		friendlySpawnCooldown = Random.Range(MIN_SPAWNCOOLDOWN_FRIENDLY, MAX_SPAWNCOOLDOWN_FRIENDLY);
		//enemySpawnCooldown = Random.Range(MIN_SPAWNCOOLDOWN_ENEMY, MAX_SPAWNCOOLDOWN_ENEMY);

		enemies = new List<CarController>();

		for(int i = 150; i > 0; i--)
		{
			spawner.SpawnItem();

			if(i <= 50)
			{
				spawner.SpawnFriendlyNPC();
			}

			if(i == MIN_NUMBEROFENEMIES)
			{
				enemies.Add(spawner.SpawnEnemyNPC().GetComponent<CarController>());
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			gameIsPaused = !gameIsPaused;
			if(gameIsPaused)
			{
				Time.timeScale = 0.0f;
				AudioController.Instance.Pause();
				pauseMenuCanvas.enabled = true;
				return;
			}
			else
			{
				Time.timeScale = 1.0f;
				AudioController.Instance.Resume();
				pauseMenuCanvas.enabled = false;
			}
		}

		if(roundTime > 0.0f)
		{
			roundTime -= Time.deltaTime;
			timerText.text = string.Format("{0:0}:{1:00}", Mathf.FloorToInt(roundTime) / 60, Mathf.FloorToInt(roundTime) % 60);

			itemSpawnCooldown -= Time.deltaTime;

			if(itemSpawnCooldown <= 0.0f)
			{
				spawner.SpawnItem();
				itemSpawnCooldown = Random.Range(MIN_SPAWNCOOLDOWN_ITEM, MAX_SPAWNCOOLDOWN_ITEM);
			}

			friendlySpawnCooldown -= Time.deltaTime;

			if(friendlySpawnCooldown <= 0.0f)
			{
				spawner.SpawnFriendlyNPC();
				friendlySpawnCooldown = Random.Range(MIN_SPAWNCOOLDOWN_FRIENDLY, MAX_SPAWNCOOLDOWN_FRIENDLY);
			}

			if(player.GetFollowerCount() / 5 > enemies.Count - MIN_NUMBEROFENEMIES)
			{
				enemies.Add(spawner.SpawnEnemyNPC().GetComponent<CarController>());
			}

			if(player.GetFollowerCount() / 5 < enemies.Count - MIN_NUMBEROFENEMIES)
			{
				enemies.RemoveAt(Random.Range(0, enemies.Count));

			}
		}
		else
		{
			EndGame();
		}
	}

	public void Unpause()
	{
		Time.timeScale = 1.0f;
		AudioController.Instance.Resume();
		pauseMenuCanvas.enabled = false;
	}

	void EndGame()
	{
		gameOverCanvas.enabled = true;
		score = player.Score;
		Time.timeScale = 0.0f;
		AudioController.Instance.Pause();
		timerText.text = string.Format("{0:0}:{1:00}", 0, 0);
	}

	public void GoToMainMenu()
	{
		SceneManager.LoadScene(0);
	}
}
