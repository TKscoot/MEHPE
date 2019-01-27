using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatCalculator : MonoBehaviour
{
    [SerializeField] private AnimationCurve multiplicator;
    [SerializeField] private float bpm;

    private static BeatCalculator instance = null;
    
    public static BeatCalculator Instance
    {
        get
        {
            return instance;
        }
    }

    private float timer;
    private float diffPerBeat;
    private float beatValue;
    private bool gameStarted;

    private void Awake()
    {
        instance = this;
    }

    private void Start ()
    {
        diffPerBeat = bpm / 60.0f;
        timer = 0.0f;
    }

    private void Update ()
    {
        if (gameStarted)
        {
            timer += Time.deltaTime;

            if (timer >= (1.0f / diffPerBeat))
            {
                timer = 0.0f;
            }

            beatValue = multiplicator.Evaluate(timer / (1.0f / diffPerBeat));
        }
	}

    public void StartGame()
    {
        gameStarted = true;
    }

    public float BeatValue { get { return beatValue; } }
}
