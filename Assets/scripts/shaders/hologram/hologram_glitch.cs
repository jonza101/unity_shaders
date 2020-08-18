using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class hologram_glitch : MonoBehaviour
{
	public float glitch_chance;
	public float glith_loop_wait;

	public Color tint_color;
	public Color default_tint_color;

	public float min_transparency;
	public float max_transparency;
	public float default_transparency;

	public float min_cutout;
	public float max_cutout;
	public float default_cutout;

	public float min_dist;
	public float max_dist;
	public float default_dist;

	public float min_amplitude;
	public float max_amplitude;
	public float default_amplitude;

	public float min_speed;
	public float max_speed;
	public float default_speed;

	public float min_amount;
	public float max_amount;
	public float default_amount;

	public Vector3 min_scale;
	public Vector3 max_scale;
	public Vector3 default_scale;

	public float min_dur;
	public float max_dur;


	private Renderer renderer;

	private WaitForSeconds glitch_loop_wait_s;
	private WaitForSeconds glitch_dur_s;

	void	Awake()
	{
		this.renderer = GetComponent<Renderer>();

		this.glitch_loop_wait_s = new WaitForSeconds(this.glith_loop_wait);


		this.renderer.material.SetColor("_tint_color", this.default_tint_color);
		this.renderer.material.SetFloat("_transparency", this.default_transparency);
		this.renderer.material.SetFloat("_cutout_thresh", this.default_cutout);

		this.renderer.material.SetFloat("_distance", this.default_dist);
		this.renderer.material.SetFloat("_amplitude", this.default_amplitude);
		this.renderer.material.SetFloat("_speed", this.default_speed);
		this.renderer.material.SetFloat("_amount", this.default_amount);

		transform.localScale = this.default_scale;
	}

	IEnumerator	Start()
	{
		while (true)
		{
			float glitch_test = Random.Range(0.0f, 1.0f);
			if (glitch_test <= this.glitch_chance)
				StartCoroutine(glitch());

			yield return (this.glitch_loop_wait_s);
		}
	}

	IEnumerator glitch()
	{
		this.glitch_dur_s = new WaitForSeconds(Random.Range(this.min_dur, this.max_dur));

		this.renderer.material.SetColor("_tint_color", this.tint_color);
		this.renderer.material.SetFloat("_transparency", Random.Range(this.min_transparency, this.max_transparency));
		this.renderer.material.SetFloat("_cutout_thresh", Random.Range(this.min_cutout, this.max_cutout));

		this.renderer.material.SetFloat("_distance", Random.Range(this.min_dist, this.max_dist));
		this.renderer.material.SetFloat("_amplitude", Random.Range(this.min_amplitude, this.max_amplitude));
		this.renderer.material.SetFloat("_speed", Random.Range(this.min_speed, this.max_speed));
		this.renderer.material.SetFloat("_amount", Random.Range(this.min_amount, this.max_amount));

		transform.localScale = new Vector3(Random.Range( this.min_scale.x, this.max_scale.x),
											Random.Range(this.min_scale.y, this.max_scale.y),
											Random.Range(this.min_scale.z, this.max_scale.z));

		yield return (this.glitch_dur_s);

		this.renderer.material.SetColor("_tint_color", this.default_tint_color);
		this.renderer.material.SetFloat("_transparency", this.default_transparency);
		this.renderer.material.SetFloat("_cutout_thresh", this.default_cutout);

		this.renderer.material.SetFloat("_distance", this.default_dist);
		this.renderer.material.SetFloat("_amplitude", this.default_amplitude);
		this.renderer.material.SetFloat("_speed", this.default_speed);
		this.renderer.material.SetFloat("_amount", this.default_amount);

		transform.localScale = this.default_scale;
	}
}
