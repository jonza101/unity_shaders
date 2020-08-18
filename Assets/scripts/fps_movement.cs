using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class fps_movement : MonoBehaviour
{
	public GameObject           camera;
    public CharacterController	controller;

	public float		speed;
	public float		jump_height;
	public float		jump_gravity;
	public float		gravity;
	public float		gravity_ground_force;

	public Transform	ground_check;
	public float		ground_dist;
	public LayerMask	ground_mask;

	private Vector3		movement;
	private Vector3		velocity;
	private bool		is_grounded;


    void	Start()
	{
		this.movement = new Vector3(0.0f, 0.0f, 0.0f);
		this.velocity = new Vector3(0.0f, 0.0f, 0.0f);
	}

    void	Update()
	{
		this.is_grounded = Physics.CheckSphere(this.ground_check.position, this.ground_dist, this.ground_mask);

		this.movement = (Input.GetAxisRaw("Horizontal") * transform.right + Input.GetAxisRaw("Vertical") * transform.forward) * this.speed;
		this.movement = Vector3.ClampMagnitude(this.movement, this.speed);
		controller.Move(this.movement * Time.deltaTime);

		if (Input.GetKeyDown(KeyCode.Space) && this.is_grounded)
			this.velocity.y += Mathf.Sqrt(this.jump_height * -2.0f * this.jump_gravity);

		this.velocity.y += this.gravity * Time.deltaTime * Time.deltaTime;
		if (this.is_grounded && this.velocity.y < 0.0f)
			this.velocity.y = this.gravity_ground_force;

		controller.Move(this.velocity);
	}
}
