using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour {

    enum States { NONE, ROAM, FOLLOW};
    [SerializeField] States currentState = States.NONE;

    Rigidbody rb;

    private float timeToChange = 5f;
    [SerializeField] float moveSpeed = 0.1f;
    [SerializeField]
    float followSpeed = 8f;

    Quaternion finalRotation = Quaternion.identity;

    float closestDistanceToPlayer = 4.0f;

    GameObject player = null;

    // Use this for initialization
    void Awake () {
        rb = GetComponent<Rigidbody>();
        if(currentState == States.NONE)
        {
            currentState = States.ROAM;
        }
        player = GameObject.FindGameObjectWithTag("Player");
        rb.freezeRotation = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        switch(currentState)
        {
            case States.ROAM:
                timeToChange -= Time.deltaTime;

                rb.MovePosition(transform.position + transform.forward * moveSpeed);

                if (timeToChange <= 0)
                {
                    float angle = Random.Range(0.0f, 360.0f);

                    finalRotation = Quaternion.Euler(transform.rotation.x, angle, transform.rotation.z);
                    timeToChange = Random.Range(0.5f, 5f);
                }

                AvoidCollision();
                transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, Time.deltaTime * 1f);
                break;
            case States.FOLLOW:
                FollowPlayer();
                break;
        }
    }

    private void FollowPlayer()
    {

        Vector3 distanceToPlayer = player.transform.position - transform.position;
        Vector3 movementDirection = Vector3.zero;
        float intensity = 1 / distanceToPlayer.magnitude;

        if (distanceToPlayer.magnitude > closestDistanceToPlayer && distanceToPlayer.magnitude < closestDistanceToPlayer + 0.1f)
        {
        }
        else if (distanceToPlayer.magnitude < closestDistanceToPlayer)
        {
            movementDirection = -distanceToPlayer * Time.fixedDeltaTime * followSpeed * intensity;
        }
        else
        {
            movementDirection = distanceToPlayer * Time.fixedDeltaTime * followSpeed * intensity;
        }

        rb.MovePosition(rb.position + movementDirection);

        //finalRotation = Quaternion.FromToRotation(transform.forward, Vector3.Normalize(movementDirection));
        //transform.rotation = finalRotation;
        transform.LookAt(player.transform);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            currentState = States.FOLLOW;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player") currentState = States.ROAM;
    }

    void AvoidCollision()
    {
        RaycastHit hit;
        Ray ray = new Ray();
        ray.origin = rb.position + transform.up;
        ray.direction = transform.forward;

        Debug.DrawRay(ray.origin, ray.direction * 5, Color.green);

        //if (Physics.Raycast(ray.origin, ray.direction, out hit, 5f))
        if (Physics.BoxCast(ray.origin, new Vector3(0.5f, 0.5f, 0.5f), ray.direction, out hit, rb.rotation, ray.direction.magnitude * 5))
        {
			if(hit.collider.tag == "Friendly NPC")
			{
				if(hit.collider.gameObject.GetComponent<NPCController>().IsFollowing())
				{
					return;
				}
			}

            Vector3 evasionVector = Vector3.Normalize(hit.normal + Vector3.Normalize(ray.direction));
            Debug.DrawRay(hit.point, evasionVector, Color.magenta);

            float intensity = 1 / (hit.point - ray.origin).magnitude;

            finalRotation = Quaternion.FromToRotation(ray.direction, evasionVector);
            finalRotation.eulerAngles = new Vector3(0, finalRotation.eulerAngles.y * Time.fixedDeltaTime * intensity, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, Time.deltaTime * 1f);
        }
        
    }

	private void OnCollisionEnter(Collision collision)
	{
		if(collision.collider.tag == "Friendly NPC")
		{
			if(collision.collider.gameObject.GetComponent<NPCController>().IsFollowing())
			{
				collision.gameObject.GetComponent<NPCController>().Die();
			}
		}
	}
}