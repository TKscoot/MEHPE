using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class NPCController : MonoBehaviour
{
	enum NPCSTATE { NONE, NPC_IDLE, NPC_ROAM, NPC_FOLLOW, NPC_ENTERHOUSE }

	GameObject player = null;
	Rigidbody rb = null;
	NPCSTATE currentState = NPCSTATE.NONE;

    NPCNeeds needs;
	Collector collectorScript;

	[SerializeField] Animator anim = null;

    //Movement variables
	float moveSpeed = 5.0f;
	float turnSpeed = 5.0f;

	//Idle Variables
	float idleTime = 99.0f;

	//Roam state variables
	float movementTime = 99.0f;

	//changedirection variables
	float changeDirectionTime = 99.0f;
	bool changeDirectionCoroutineRunning = false;
	bool changeDirectionCoroutineFinished = false;

	//follow state variables
	float closestDistanceToPlayer = 2.0f;

	//house state variables
	GameObject houseGameObject = null;


	const float MIN_MOVESPEED = 11.5f;
	const float MAX_MOVESPEED = 12.5f;
	const float MIN_TURNSPEED = 1.5f;
	const float MAX_TURNSPEED = 7.5f;
	const float MIN_IDLETIME = 1.5f;
	const float MAX_IDLETIME = 7.5f;
	const float MIN_MOVEMENTTIME = 1.5f;
	const float MAX_MOVEMENTTIME = 7.5f;
	const float MIN_CHANGEDRIECTIONTIME = 1.5f;
	const float MAX_CHANGEDRIECTIONTIME = 7.5f;

	/********************************************************************************************************************/
	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		gameObject.tag = "Friendly NPC";
        needs = GetComponent<NPCNeeds>();
    }

	// Use this for initialization
	void Start()
	{
		moveSpeed = Random.Range(MIN_MOVESPEED, MAX_MOVESPEED);
		turnSpeed = Random.Range(MIN_TURNSPEED, MAX_TURNSPEED);
		idleTime = Random.Range(MIN_IDLETIME, MAX_IDLETIME);
		movementTime = Random.Range(MIN_MOVEMENTTIME, MAX_MOVEMENTTIME);
		changeDirectionTime = Random.Range(MIN_CHANGEDRIECTIONTIME, MAX_CHANGEDRIECTIONTIME);
		if(currentState == NPCSTATE.NONE)
		{
			currentState = NPCSTATE.NPC_IDLE;
		}
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		switch(currentState)
		{
			case NPCSTATE.NONE:
				Debug.LogError(this.name + ": NPCSTATE is NPCSTATE.NONE. NPC is not working!");
				break;
			case NPCSTATE.NPC_IDLE:
				HandleIdleState();
				break;
			case NPCSTATE.NPC_ROAM:
				HandleRoamState();
				break;
			case NPCSTATE.NPC_FOLLOW:
				HandleFollowState();
				break;
			case NPCSTATE.NPC_ENTERHOUSE:
				HandleHouseState();
				break;
			default:
				Debug.LogError(this.name + ": switch fell into default case. NPC is not working!");
				break;
		}
	}

	/********************************************************************************************************************/
	void HandleIdleState()
	{
		idleTime -= Time.fixedDeltaTime;

		if(idleTime > 0.0f)
		{

		}
		else
		{
			idleTime = Random.Range(MIN_IDLETIME, MAX_IDLETIME);
			currentState = NPCSTATE.NPC_ROAM;
		}
	}

	/********************************************************************************************************************/
	void HandleRoamState()
	{
		movementTime -= Time.fixedDeltaTime;

		if(movementTime > 0.0f)
		{
			changeDirectionTime -= Time.fixedDeltaTime;
			if(changeDirectionTime <= 0.0f)
			{
				if(!changeDirectionCoroutineRunning)
				{
					StartCoroutine("ChangeDirection");
					changeDirectionCoroutineRunning = true;
				}

				if(changeDirectionCoroutineFinished)
				{
					changeDirectionCoroutineRunning = false;
					changeDirectionCoroutineFinished = false;
					changeDirectionTime = Random.Range(MIN_CHANGEDRIECTIONTIME, MAX_CHANGEDRIECTIONTIME);
				}
			}
			AvoidCollision();
			Vector3 moveDirection = transform.forward * Time.fixedDeltaTime * moveSpeed;
			rb.MovePosition(transform.position + moveDirection);

			anim.SetFloat("Speed_f", moveDirection.magnitude);
		}
		else
		{
			movementTime = Random.Range(MIN_MOVEMENTTIME, MAX_MOVEMENTTIME);
			currentState = NPCSTATE.NPC_IDLE;
			anim.SetFloat("Speed_f", 0.0f);
		}
	}

	IEnumerator ChangeDirection()
	{
		float rotationSign = Mathf.Sign(Random.Range(-1.0f, 1.0f));
		float angle = Random.Range(0f, 360f);

		float leftToRotate = angle;
		float nextRotation = leftToRotate - (leftToRotate - Time.fixedDeltaTime * turnSpeed);

		while(leftToRotate >= 0.0f)
		{
			float signedRotation = nextRotation * rotationSign;
			rb.MoveRotation(Quaternion.Euler(rb.rotation.eulerAngles.x, rb.rotation.eulerAngles.y + signedRotation, rb.rotation.eulerAngles.z));

			leftToRotate -= nextRotation;
			nextRotation = leftToRotate - (leftToRotate - Time.fixedDeltaTime * turnSpeed);

			yield return new WaitForEndOfFrame();
		}

		changeDirectionCoroutineFinished = true;
	}

	void AvoidCollision()
	{
		RaycastHit hit;
		Ray ray = new Ray();
		ray.origin = rb.position;
		ray.direction = transform.forward;

		//Debug.DrawRay(ray.origin, ray.direction * 5, Color.green);


		if(Physics.BoxCast(ray.origin, new Vector3(0.5f, 0.5f, 0.5f), ray.direction, out hit, rb.rotation, ray.direction.magnitude * 5, int.MaxValue, QueryTriggerInteraction.Ignore))
		{
			StopCoroutine("ChangeDirection");
			Vector3 evasionVector = Vector3.Normalize(hit.normal + Vector3.Normalize(ray.direction));
			//Debug.DrawRay(hit.point, evasionVector, Color.magenta);

			float intensity = 1 / (hit.point - ray.origin).magnitude;

			Quaternion newRotation = Quaternion.FromToRotation(ray.direction, evasionVector);
			newRotation.eulerAngles = new Vector3(0, newRotation.eulerAngles.y * Time.fixedDeltaTime * intensity, 0);

			float sign = Mathf.Sign(Vector3.SignedAngle(transform.forward, evasionVector, Vector3.Cross(evasionVector, transform.forward)));

			rb.MoveRotation(Quaternion.Euler(rb.rotation.eulerAngles.x, rb.rotation.eulerAngles.y + newRotation.eulerAngles.y * sign, rb.rotation.eulerAngles.z));
		}
	}

	/********************************************************************************************************************/
	void HandleFollowState()
	{
		if(player == null)
		{
			player = GameObject.FindGameObjectWithTag("Player");
		}

		Vector3 distanceToPlayer = player.GetComponent<Rigidbody>().position - rb.position;
		Vector3 movementDirection = Vector3.zero;
		float intensity = 1 / distanceToPlayer.magnitude;

		if(distanceToPlayer.magnitude > closestDistanceToPlayer && distanceToPlayer.magnitude < closestDistanceToPlayer + 0.1f)
		{
		}
		else if(distanceToPlayer.magnitude < closestDistanceToPlayer)
		{
			movementDirection = -distanceToPlayer * Time.fixedDeltaTime * moveSpeed * intensity;
		}
		else
		{
			movementDirection = distanceToPlayer * Time.fixedDeltaTime * moveSpeed * intensity;
		}

		rb.MovePosition(rb.position + movementDirection);

		anim.SetFloat("Speed_f", movementDirection.magnitude);

		AvoidCollision(movementDirection);

		Quaternion newRotation = Quaternion.FromToRotation(transform.forward, Vector3.Normalize(movementDirection));
		newRotation.eulerAngles = new Vector3(0, newRotation.eulerAngles.y, 0);

		rb.MoveRotation(Quaternion.Euler(rb.rotation.eulerAngles.x, rb.rotation.eulerAngles.y + newRotation.eulerAngles.y, rb.rotation.eulerAngles.z));
	}

	void AvoidCollision(Vector3 _movementDirection)
	{
		RaycastHit hit;
		Ray ray = new Ray();
		ray.origin = rb.position;
		ray.direction = Vector3.Normalize(_movementDirection);

		//Debug.DrawRay(ray.origin, ray.direction * (closestDistanceToPlayer - 0.1f), Color.green);


		if(Physics.BoxCast(ray.origin, new Vector3(0.5f, 0.5f, 0.5f), ray.direction, out hit, rb.rotation, ray.direction.magnitude * (closestDistanceToPlayer - 0.1f), int.MaxValue, QueryTriggerInteraction.Ignore))
		{
			if(hit.transform.tag == "Ground")
			{
				return;
			}

			if(hit.transform.tag == "Friendly NPC" || hit.transform.tag == "Player")
			{
				rb.MovePosition(rb.position - _movementDirection);
				anim.SetFloat("Speed_f", 0.0f);
				return;
			}

			Vector3 evasionVector = Vector3.Normalize(hit.normal + Vector3.Normalize(ray.direction));
			//Debug.DrawRay(hit.point, evasionVector, Color.magenta);

			float intensity = 1 / (hit.point - ray.origin).magnitude;

			Quaternion newRotation = Quaternion.FromToRotation(ray.direction, evasionVector);
			newRotation.eulerAngles = new Vector3(0, newRotation.eulerAngles.y * Time.fixedDeltaTime * intensity, 0);

			float sign = Mathf.Sign(Vector3.SignedAngle(transform.forward, evasionVector, Vector3.Cross(evasionVector, transform.forward)));

			rb.MoveRotation(Quaternion.Euler(rb.rotation.eulerAngles.x, rb.rotation.eulerAngles.y + newRotation.eulerAngles.y * sign, rb.rotation.eulerAngles.z));
		}
	}

	public void SetFollowing()
	{
		currentState = NPCSTATE.NPC_FOLLOW;
        needs.Following();
		anim.SetBool("Following_b", true);
    }

	/********************************************************************************************************************/
	void HandleHouseState()
	{
		if(houseGameObject == null)
		{
			Debug.LogError(gameObject.name + ": No Houseobject assigned NPC is not properly working!");
			return;
		}

		Vector3 distanceToHouse = houseGameObject.transform.position - rb.position;

		Quaternion newRotation = Quaternion.FromToRotation(transform.forward, Vector3.Normalize(distanceToHouse));
		newRotation.eulerAngles = new Vector3(0, newRotation.eulerAngles.y, 0);

		rb.MoveRotation(Quaternion.Euler(rb.rotation.eulerAngles.x, rb.rotation.eulerAngles.y + newRotation.eulerAngles.y, rb.rotation.eulerAngles.z));

		Vector3 moveDirection = distanceToHouse * Time.fixedDeltaTime * moveSpeed;
		rb.MovePosition(rb.position + moveDirection);

		anim.SetFloat("Speed_f", moveDirection.magnitude);
	}

	public void SetHouseGameObject(GameObject _house)
	{
		houseGameObject = _house;
		currentState = NPCSTATE.NPC_ENTERHOUSE;
	}

	public void SetCollectorScript(Collector _script)
	{
		collectorScript = _script;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if(houseGameObject != null && collision.collider.tag == "PlayerHouse")
		{
			collectorScript.IncreaseScore();
			Destroy(gameObject, 0.1f);
		}
	}

	public bool IsFollowing()
	{
		return currentState == NPCSTATE.NPC_FOLLOW; 
	}

	public void Die()
	{
		collectorScript.RemoveFollower(this);
		Destroy(gameObject, 0.1f);
	}
}
