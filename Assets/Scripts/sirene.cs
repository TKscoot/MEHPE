using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sirene : MonoBehaviour
{
    [SerializeField] int speed = 20;

	void Update ()
    {
        gameObject.transform.Rotate(Time.deltaTime * Vector3.up * speed);
	}
}
