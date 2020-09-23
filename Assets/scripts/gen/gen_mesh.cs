using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class gen_mesh : MonoBehaviour
{
	private Mesh			mesh;

	private MeshFilter		mesh_filter;
	private MeshRenderer	mesh_renderer;


	[SerializeField]
	private int				x_size;
	[SerializeField]
	private int				z_size;
	[SerializeField]
	private float			scale;

	[SerializeField]
	private float			x_mul;
	[SerializeField]
	private float			z_mul;
	[SerializeField]
	private float			y_mul;

	private Vector3[]		vertices;
	private int[]			indices;


	void	Start()
	{
		this.mesh = new Mesh();

		this.mesh_filter = GetComponent<MeshFilter>();
		this.mesh_filter.mesh = this.mesh;
		this.mesh_renderer = GetComponent<MeshRenderer>();


		gen_shape();
	}

	void	Update()
	{
		gen_shape();
	}

	void	gen_shape()
	{
		this.mesh.Clear();

		int array_size = (this.x_size + 1) * (this.z_size + 1);

		int i = 0;
		this.vertices = new Vector3[array_size];
		this.indices = new int[this.x_size * this.z_size * 6];

		for (int z = 0; z <= this.z_size; z++)
		{
			for (int x = 0; x <= this.x_size; x++)
			{
				float y = Mathf.PerlinNoise(x * this.x_mul, z * this.z_mul) * this.y_mul;
				this.vertices[i] = new Vector3(x * this.scale, y, z * this.scale);
				i++;
			}
		}

		int v = 0;
		int t = 0;
		for (int z = 0; z < this.z_size; z++)
		{
			for (int x = 0; x < this.x_size; x++)
			{
				this.indices[t + 0] = v + 0;
				this.indices[t + 1] = v + this.x_size + 1;
				this.indices[t + 2] = v + 1;
				this.indices[t + 3] = v + 1;
				this.indices[t + 4] = v + this.x_size + 1;
				this.indices[t + 5] = v + this.x_size + 2;

				t += 6;
				v++;
			}
			v++;
		}


		this.mesh.vertices = this.vertices;
		this.mesh.triangles = this.indices;
		this.mesh.RecalculateNormals();
	}

	//void	OnDrawGizmos()
	//{
	//	Gizmos.color = Color.red;
	//	for (int i = 0; i < this.vertices.Length; i++)
	//	{
	//		Gizmos.DrawSphere(this.vertices[i], 0.05f);
	//	}
	//}
}
