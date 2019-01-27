using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Use this script for bodys to be attracted to the planet!
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class FauxGravityBody : MonoBehaviour
{

    private FauxGravityAttractor attractor;
    private Rigidbody rb;

    public bool placeOnSurface = false;

	private void Awake()
	{
        rb = GetComponent<Rigidbody>();
		rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

    }

	void Start()
    {
        attractor = FauxGravityAttractor.instance;
    }

    void FixedUpdate()
    {
        if (placeOnSurface)
            attractor.PlaceOnSurface(rb);
        else
            attractor.Attract(rb);
    }

}
