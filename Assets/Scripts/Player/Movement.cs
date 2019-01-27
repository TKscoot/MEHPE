using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] Animator anim;
    [SerializeField] Transform character;

    Rigidbody rb;
    Vector2 moveDirection;

    private void Awake()
	{
		gameObject.tag = "Player";
	}
    
	void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        anim.SetFloat("Speed_f", moveDirection.magnitude);

        float rot = Mathf.Rad2Deg * Mathf.Atan2(moveDirection.x, moveDirection.y);

        if (moveDirection.magnitude > 0.0f)
        {
            character.localRotation = Quaternion.Euler(0.0f, rot, 0.0f);
        }
    }
    
    void FixedUpdate()
    {
		rb.MovePosition(transform.position + (transform.right * moveDirection.x * moveSpeed) + (transform.forward * moveDirection.y * moveSpeed));
    }
}
