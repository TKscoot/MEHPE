using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HighScoreManager : MonoBehaviour {

    [SerializeField] GameManager gm = null;
    [SerializeField] Text[] names = new Text[10];
    [SerializeField] Text[] scores = new Text[10];
    [SerializeField] InputField enterName = null;
    [SerializeField] Text userRank = null;
	[SerializeField] GameObject panel10 = null;
	[SerializeField] Button submitButton = null;
	[SerializeField] GameObject mainMenuButton = null;


    Dictionary<string, float> dict = null;

	// Use this for initialization
	void Start () {
		if(gm == null)
		{
			panel10.SetActive(false);
		}

        dict = SqlHandler.instance.GetTop10UserScores();

        for(int i = 0; i < dict.Count; i++)
        {
            var item = dict.ElementAt(i);
            names[i].text = item.Key;
            scores[i].text = item.Value.ToString();
        }
	}

    public void SetUserScore()
    {
        SqlHandler.instance.SetUserScore(enterName.text, gm.Score);
        userRank.text = SqlHandler.instance.GetUserRank(enterName.text).ToString();

		dict = SqlHandler.instance.GetTop10UserScores();

		for(int i = 0; i < dict.Count; i++)
		{
			var item = dict.ElementAt(i);
			names[i].text = item.Key;
			scores[i].text = item.Value.ToString();
		}

		enterName.interactable = false;
		submitButton.interactable = false;

		mainMenuButton.SetActive(true);
	}

	public void LoadMainMenu()
	{
		SceneManager.LoadScene(0);
	}

}
