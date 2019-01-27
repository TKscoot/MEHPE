using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private BeatCalculator bpmCalculator;
    [SerializeField] private List<AudioLevel> levelThemes;
    [SerializeField] private int actualPartyMembers;

    private int actualLevel;
	private float pauseVolume = 0.2f;

    private static AudioController instance = null;

    public static AudioController Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartGame();
    }

    public void AddPartyMember(int _amount)
    {
        actualPartyMembers += _amount;

        if (actualLevel < levelThemes.Count - 1)
        {
            if (actualPartyMembers >= levelThemes[actualLevel + 1].partyMembers)
            {
                actualLevel++;
                levelThemes[actualLevel].level.volume = 1.0f;
            }
        }
    }

    public void RemovePartyMember(int _amount)
    {
        actualPartyMembers -= _amount;

        if(actualPartyMembers < 0)
        {
            actualPartyMembers = 0;
        }
        
        if (actualLevel >= 1)
        {
            if (actualPartyMembers < levelThemes[actualLevel].partyMembers)
            {
                levelThemes[actualLevel].level.volume = 0.0f;
                actualLevel--;
            }

        }
    }

    public void StartGame()
    {
        foreach (var themes in levelThemes)
        {
            themes.level.Play();
            themes.level.volume = 0.0f;
        }

        levelThemes[0].level.volume = 1.0f;
        bpmCalculator.StartGame();
    }

	public void Pause()
	{
		foreach(var theme in levelThemes)
		{
			if(theme.level.volume > 0.0f)
			{
				theme.level.volume = pauseVolume;
			}
		}
	}

	public void Resume()
	{
		foreach(var theme in levelThemes)
		{
			if(theme.level.volume > 0.0f)
			{
				theme.level.volume = 1.0f;
			}
		}
	}
}

[System.Serializable]
public struct AudioLevel
{
    public AudioSource level;
    public int partyMembers;
}