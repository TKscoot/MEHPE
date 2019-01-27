using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatScaler : MonoBehaviour
{
    [SerializeField] public float scaleFactor;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private Light pulseLight;

    private Vector3 initScale;
    private Vector3 initPos;
    private float initIntensity;

    private void Awake()
    {
        initScale = transform.localScale;
        initPos = transform.localPosition;

        if (pulseLight != null)
        {
            initIntensity = pulseLight.intensity;
        }
    }

    private void Update()
    {
        transform.localScale = initScale + (Vector3.one * BeatCalculator.Instance.BeatValue * scaleFactor);

        if(particle != null && BeatCalculator.Instance.BeatValue >= 0.9f)
        {
            particle.Emit(1);
        }

        if (pulseLight != null)
        {
            pulseLight.intensity = BeatCalculator.Instance.BeatValue * scaleFactor * initIntensity;
        }
    }
}
