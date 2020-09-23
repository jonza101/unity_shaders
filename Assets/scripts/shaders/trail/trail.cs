using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class trail : MonoBehaviour
{
	[SerializeField] float pos_lerp_factor;
	[SerializeField] float normal_lerp_factor;

	private float elapsed_time;
	private Vector3 old_pos;
	private Vector3 old_normal;
	[SerializeField] private float speed = 0.0f;

	private Renderer ren;
    
    void	Start()
    {
		this.elapsed_time = Time.time;
		this.old_pos = this.transform.position;
		this.old_normal = new Vector3(0.0f, 0.0f, 0.0f);

		this.ren = GetComponent<Renderer>();
	}

	void	Update()
	{
		Vector3 plane_normal = Vector3.Lerp(this.old_normal, (this.transform.position - this.old_pos).normalized, this.normal_lerp_factor);
		Plane plane = new Plane(plane_normal, this.transform.position);

		this.ren.material.SetVector("_old_pos", new Vector4(this.old_pos.x, this.old_pos.y, this.old_pos.z, 1.0f));
		this.ren.material.SetVector("_curr_pos", new Vector4(this.transform.position.x, this.transform.position.y, this.transform.position.z, 1.0f));
		this.ren.material.SetVector("_plane", new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance));


		float curr_time = Time.time;
		this.speed = Vector3.Distance(this.old_pos, this.transform.position) / (curr_time - this.elapsed_time);
		this.elapsed_time = curr_time;
		this.old_pos = Vector3.Slerp(this.old_pos, this.transform.position, this.pos_lerp_factor);
	}
}
