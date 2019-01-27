using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOnAxis : MonoBehaviour
{
    [SerializeField] float rotSpeed;
    [SerializeField] Vector3 axis;

	private void Update ()
    {
        transform.Rotate(axis * rotSpeed * Time.deltaTime);
	}
}
