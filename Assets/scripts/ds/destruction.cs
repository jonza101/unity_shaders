using System.Collections.Generic;
using UnityEngine;

public class destruction : MonoBehaviour
{
	private Renderer		renderer;

	private Mesh			mesh;
	private MeshRenderer	mesh_renderer;
	private MeshFilter		mesh_filter;
	private MeshCollider	mesh_collider;

	List<Vector3>			vertices;
	List<int>				triangles;
	List<Vector3>			normals;
	List<Vector2>			uvs;


	public destruction_main main_obj_controller;
	private bool			can_destruct;


    void	Start()
    {
		this.can_destruct = true;
		this.init_mesh();
	}

	public void init_mesh()
	{
		this.renderer = GetComponent<Renderer>();

		this.mesh_renderer = GetComponent<MeshRenderer>();
		this.mesh_filter = GetComponent<MeshFilter>();
		this.mesh = this.mesh_filter.mesh;
		this.mesh_collider = GetComponent<MeshCollider>();

		this.vertices = new List<Vector3>(this.mesh.vertices);
		this.triangles = new List<int>(this.mesh.triangles);
		this.normals = new List<Vector3>(this.mesh.normals);
		this.uvs = new List<Vector2>(this.mesh.uv);

		//Debug.Log(this.mesh.triangles.Length);
		//Debug.Log(this.mesh.normals.Length);
		//Debug.Log(this.triangles.Count);
		//Debug.Log(this.normals.Count);


		if (this.renderer.bounds.size.x <= this.main_obj_controller.min_poss_volume.x
			&& this.renderer.bounds.size.y <= this.main_obj_controller.min_poss_volume.y
			&& this.renderer.bounds.size.z <= this.main_obj_controller.min_poss_volume.z)
		{
			this.can_destruct = false;
		}
	}

	void	Update()
    {
		if (Input.GetKeyDown(KeyCode.Q))
		{
			if (this.can_destruct && this.main_obj_controller.submesh_count < this.main_obj_controller.max_submesh_count)
			{
				this.main_obj_controller.submesh_count++;
				make_obj();
			}
		}
    }

	void	make_obj()
	{
		Vector3 pos = new Vector3(Random.Range(this.mesh.bounds.min.x, this.mesh.bounds.max.x),
									Random.Range(this.mesh.bounds.min.y, this.mesh.bounds.max.y),
									Random.Range(this.mesh.bounds.min.z, this.mesh.bounds.max.z));
		//pos = this.mesh.bounds.center;


		GameObject obj = new GameObject();
		obj.transform.position = this.transform.position;
		obj.transform.rotation = this.transform.rotation;
		obj.transform.localScale = this.transform.localScale;
		obj.layer = this.gameObject.layer;

		Mesh obj_mesh = new Mesh();


		Ray ray1 = new Ray();
		Ray ray2 = new Ray();

		Plane plane = new Plane(Random.onUnitSphere, pos);
		Plane sub_plane = new Plane(-plane.normal, pos);


		List<Vector3> obj_vertices = new List<Vector3>();
		List<int> obj_triangles = new List<int>();
		List<Vector3> obj_normals = new List<Vector3>();
		List<Vector2> obj_uvs = new List<Vector2>();

		List<Vector3> sub_obj_vertices = new List<Vector3>();
		List<int> sub_obj_triangles = new List<int>();
		List<Vector3> sub_obj_normals = new List<Vector3>();
		List<Vector2> sub_obj_uvs = new List<Vector2>();


		bool edge_set = false;
		Vector3 edge_vert = Vector3.zero;
		Plane edge_plane = new Plane();

		bool sub_edge_set = false;
		Vector3 sub_edge_vert = Vector3.zero;
		Plane sub_edge_plane = new Plane();

		for (int i = 0; i < this.triangles.Count; i += 3)
		{
			float d0 = plane.GetDistanceToPoint(this.vertices[this.triangles[i + 0]]);
			float d1 = plane.GetDistanceToPoint(this.vertices[this.triangles[i + 1]]);
			float d2 = plane.GetDistanceToPoint(this.vertices[this.triangles[i + 2]]);

			bool a = plane.GetSide(this.vertices[this.triangles[i + 0]]);
			bool b = plane.GetSide(this.vertices[this.triangles[i + 1]]);
			bool c = plane.GetSide(this.vertices[this.triangles[i + 2]]);
			int add_vert_idx = b == c ? 0 : a == c ? 1 : 2;

			Vector3[] inside_points = new Vector3[3]; int inside_points_count = 0;
			Vector3[] outside_points = new Vector3[3]; int outside_points_count = 0;
			Vector3[] inside_normals = new Vector3[3];
			Vector3[] outside_normals = new Vector3[3];

			if (d0 >= 0.0f)
			{
				inside_normals[inside_points_count] = this.normals[this.triangles[i + 0]];
				inside_points[inside_points_count++] = this.vertices[this.triangles[i + 0]];
			}
			else
			{
				outside_normals[outside_points_count] = this.normals[this.triangles[i + 0]];
				outside_points[outside_points_count++] = this.vertices[this.triangles[i + 0]];
			}
			if (d1 >= 0.0f)
			{
				inside_normals[inside_points_count] = this.normals[this.triangles[i + 1]];
				inside_points[inside_points_count++] = this.vertices[this.triangles[i + 1]];
			}
			else
			{
				outside_normals[outside_points_count] = this.normals[this.triangles[i + 1]];
				outside_points[outside_points_count++] = this.vertices[this.triangles[i + 1]];
			}
			if (d2 >= 0.0f)
			{
				inside_normals[inside_points_count] = this.normals[this.triangles[i + 2]];
				inside_points[inside_points_count++] = this.vertices[this.triangles[i + 2]];
			}
			else
			{
				outside_normals[outside_points_count] = this.normals[this.triangles[i + 2]];
				outside_points[outside_points_count++] = this.vertices[this.triangles[i + 2]];
			}



			float sub_d0 = sub_plane.GetDistanceToPoint(this.vertices[this.triangles[i + 0]]);
			float sub_d1 = sub_plane.GetDistanceToPoint(this.vertices[this.triangles[i + 1]]);
			float sub_d2 = sub_plane.GetDistanceToPoint(this.vertices[this.triangles[i + 2]]);

			bool sub_a = sub_plane.GetSide(this.vertices[this.triangles[i + 0]]);
			bool sub_b = sub_plane.GetSide(this.vertices[this.triangles[i + 1]]);
			bool sub_c = sub_plane.GetSide(this.vertices[this.triangles[i + 2]]);
			int sub_add_vert_idx = sub_b == sub_c ? 0 : sub_a == sub_c ? 1 : 2;

			Vector3[] sub_inside_points = new Vector3[3]; int sub_inside_points_count = 0;
			Vector3[] sub_outside_points = new Vector3[3]; int sub_outside_points_count = 0;
			Vector3[] sub_inside_normals = new Vector3[3];
			Vector3[] sub_outside_normals = new Vector3[3];

			//if (sub_d0 >= 0.0f) sub_inside_points[sub_inside_points_count++] = this.vertices[this.triangles[i + 0]]; else sub_outside_points[sub_outside_points_count++] = this.vertices[this.triangles[i + 0]];
			//if (sub_d1 >= 0.0f) sub_inside_points[sub_inside_points_count++] = this.vertices[this.triangles[i + 1]]; else sub_outside_points[sub_outside_points_count++] = this.vertices[this.triangles[i + 1]];
			//if (sub_d2 >= 0.0f) sub_inside_points[sub_inside_points_count++] = this.vertices[this.triangles[i + 2]]; else sub_outside_points[sub_outside_points_count++] = this.vertices[this.triangles[i + 2]];

			if (sub_d0 >= 0.0f)
			{
				sub_inside_normals[sub_inside_points_count] = this.normals[this.triangles[i + 0]];
				sub_inside_points[sub_inside_points_count++] = this.vertices[this.triangles[i + 0]];
			}
			else
			{
				sub_outside_normals[sub_outside_points_count] = this.normals[this.triangles[i + 0]];
				sub_outside_points[sub_outside_points_count++] = this.vertices[this.triangles[i + 0]];
			}
			if (sub_d1 >= 0.0f)
			{
				sub_inside_normals[sub_inside_points_count] = this.normals[this.triangles[i + 1]];
				sub_inside_points[sub_inside_points_count++] = this.vertices[this.triangles[i + 1]];
			}
			else
			{
				sub_outside_normals[sub_outside_points_count] = this.normals[this.triangles[i + 1]];
				sub_outside_points[sub_outside_points_count++] = this.vertices[this.triangles[i + 1]];
			}
			if (sub_d2 >= 0.0f)
			{
				sub_inside_normals[sub_inside_points_count] = this.normals[this.triangles[i + 2]];
				sub_inside_points[sub_inside_points_count++] = this.vertices[this.triangles[i + 2]];
			}
			else
			{
				sub_outside_normals[sub_outside_points_count] = this.normals[this.triangles[i + 2]];
				sub_outside_points[sub_outside_points_count++] = this.vertices[this.triangles[i + 2]];
			}


			{
				if (inside_points_count == 3)
				{
					obj_triangles.Add(obj_vertices.Count);
					obj_vertices.Add(this.vertices[this.triangles[i + 0]]);
					obj_triangles.Add(obj_vertices.Count);
					obj_vertices.Add(this.vertices[this.triangles[i + 1]]);
					obj_triangles.Add(obj_vertices.Count);
					obj_vertices.Add(this.vertices[this.triangles[i + 2]]);


					obj_normals.Add(this.normals[this.triangles[i + 0]]);
					obj_normals.Add(this.normals[this.triangles[i + 1]]);	
					obj_normals.Add(this.normals[this.triangles[i + 2]]);

					//obj_uvs.Add(this.uvs[this.triangles[i + 0]]);
					//obj_uvs.Add(this.uvs[this.triangles[i + 1]]);
					//obj_uvs.Add(this.uvs[this.triangles[i + 2]]);
				}
				else if (inside_points_count == 1)
				{
					ray1.origin = inside_points[0];
					Vector3 dir1 = this.vertices[this.triangles[i + ((add_vert_idx + 1) % 3)]] - inside_points[0];
					ray1.direction = dir1.normalized;
					plane.Raycast(ray1, out float ray1_enter);
					float lerp1 = (float)ray1_enter / (float)dir1.magnitude;

					ray2.origin = inside_points[0];
					Vector3 dir2 = this.vertices[this.triangles[i + ((add_vert_idx + 2) % 3)]] - inside_points[0];
					ray2.direction = dir2.normalized;
					plane.Raycast(ray2, out float ray2_enter);
					float lerp2 = (float)ray2_enter / (float)dir2.magnitude;


					obj_triangles.Add(obj_vertices.Count);
					obj_vertices.Add(inside_points[0]);
					obj_triangles.Add(obj_vertices.Count);
					obj_vertices.Add(ray1.GetPoint(ray1_enter));
					obj_triangles.Add(obj_vertices.Count);
					obj_vertices.Add(ray2.GetPoint(ray2_enter));


					obj_normals.Add(inside_normals[0]);
					obj_normals.Add(Vector3.Lerp(inside_normals[0], this.normals[this.triangles[i + ((add_vert_idx + 1) % 3)]], lerp1));
					obj_normals.Add(Vector3.Lerp(inside_normals[0], this.normals[this.triangles[i + ((add_vert_idx + 2) % 3)]], lerp2));


					if (!edge_set)
					{
						edge_set = true;
						edge_vert = ray1.GetPoint(ray1_enter);
					}
					else
					{
						edge_plane.Set3Points(edge_vert, ray1.GetPoint(ray1_enter), ray2.GetPoint(ray2_enter));

						obj_triangles.Add(obj_vertices.Count);
						obj_vertices.Add(edge_vert);
						obj_triangles.Add(obj_vertices.Count);
						obj_vertices.Add(edge_plane.GetSide(edge_vert + -plane.normal) ? ray1.GetPoint(ray1_enter) : ray2.GetPoint(ray2_enter));
						obj_triangles.Add(obj_vertices.Count);
						obj_vertices.Add(edge_plane.GetSide(edge_vert + -plane.normal) ? ray2.GetPoint(ray2_enter) : ray1.GetPoint(ray1_enter));

						obj_normals.Add(-plane.normal);
						obj_normals.Add(-plane.normal);
						obj_normals.Add(-plane.normal);
					}
				}
				else if (inside_points_count == 2)
				{
					ray1.origin = this.vertices[this.triangles[i + ((add_vert_idx + 1) % 3)]];
					Vector3 dir1 = outside_points[0] - this.vertices[this.triangles[i + ((add_vert_idx + 1) % 3)]];
					ray1.direction = dir1.normalized;
					plane.Raycast(ray1, out float ray1_enter);
					float lerp1 = (float)ray1_enter / (float)dir1.magnitude;

					ray2.origin = this.vertices[this.triangles[i + ((add_vert_idx + 2) % 3)]];
					Vector3 dir2 = outside_points[0] - this.vertices[this.triangles[i + ((add_vert_idx + 2) % 3)]];
					ray2.direction = dir2.normalized;
					plane.Raycast(ray2, out float ray2_enter);
					float lerp2 = (float)ray2_enter / (float)dir2.magnitude;


					obj_triangles.Add(obj_vertices.Count);
					obj_vertices.Add(ray1.GetPoint(ray1_enter));
					obj_triangles.Add(obj_vertices.Count);
					obj_vertices.Add(this.vertices[this.triangles[i + ((add_vert_idx + 1) % 3)]]);
					obj_triangles.Add(obj_vertices.Count);
					obj_vertices.Add(this.vertices[this.triangles[i + ((add_vert_idx + 2) % 3)]]);


					obj_normals.Add(Vector3.Lerp(this.normals[this.triangles[i + ((add_vert_idx + 1) % 3)]], outside_normals[0], lerp1));
					obj_normals.Add(this.normals[this.triangles[i + ((add_vert_idx + 1) % 3)]]);
					obj_normals.Add(this.normals[this.triangles[i + ((add_vert_idx + 2) % 3)]]);


					obj_triangles.Add(obj_vertices.Count);
					obj_vertices.Add(ray1.GetPoint(ray1_enter));
					obj_triangles.Add(obj_vertices.Count);
					obj_vertices.Add(this.vertices[this.triangles[i + ((add_vert_idx + 2) % 3)]]);
					obj_triangles.Add(obj_vertices.Count);
					obj_vertices.Add(ray2.GetPoint(ray2_enter));


					obj_normals.Add(Vector3.Lerp(this.normals[this.triangles[i + ((add_vert_idx + 1) % 3)]], outside_normals[0], lerp1));
					obj_normals.Add(this.normals[this.triangles[i + ((add_vert_idx + 2) % 3)]]);
					obj_normals.Add(Vector3.Lerp(this.normals[this.triangles[i + ((add_vert_idx + 2) % 3)]], outside_normals[0], lerp2));


					if (!edge_set)
					{
						edge_set = true;
						edge_vert = ray1.GetPoint(ray1_enter);
					}
					else
					{
						edge_plane.Set3Points(edge_vert, ray1.GetPoint(ray1_enter), ray2.GetPoint(ray2_enter));

						obj_triangles.Add(obj_vertices.Count);
						obj_vertices.Add(edge_vert);
						obj_triangles.Add(obj_vertices.Count);
						obj_vertices.Add(edge_plane.GetSide(edge_vert + -plane.normal) ? ray1.GetPoint(ray1_enter) : ray2.GetPoint(ray2_enter));
						obj_triangles.Add(obj_vertices.Count);
						obj_vertices.Add(edge_plane.GetSide(edge_vert + -plane.normal) ? ray2.GetPoint(ray2_enter) : ray1.GetPoint(ray1_enter));


						obj_normals.Add(-plane.normal);
						obj_normals.Add(-plane.normal);
						obj_normals.Add(-plane.normal);
					}
				}
			}

			{
				if (sub_inside_points_count == 3)
				{
					sub_obj_triangles.Add(sub_obj_vertices.Count);
					sub_obj_vertices.Add(this.vertices[this.triangles[i + 0]]);
					sub_obj_triangles.Add(sub_obj_vertices.Count);
					sub_obj_vertices.Add(this.vertices[this.triangles[i + 1]]);
					sub_obj_triangles.Add(sub_obj_vertices.Count);
					sub_obj_vertices.Add(this.vertices[this.triangles[i + 2]]);


					sub_obj_normals.Add(this.normals[this.triangles[i + 0]]);
					sub_obj_normals.Add(this.normals[this.triangles[i + 1]]);
					sub_obj_normals.Add(this.normals[this.triangles[i + 2]]);

					//obj_uvs.Add(this.uvs[this.triangles[i + 0]]);
					//obj_uvs.Add(this.uvs[this.triangles[i + 1]]);
					//obj_uvs.Add(this.uvs[this.triangles[i + 2]]);
				}
				else if (sub_inside_points_count == 1)
				{
					ray1.origin = sub_inside_points[0];
					Vector3 dir1 = this.vertices[this.triangles[i + ((sub_add_vert_idx + 1) % 3)]] - sub_inside_points[0];
					ray1.direction = dir1.normalized;
					sub_plane.Raycast(ray1, out float ray1_enter);
					float lerp1 = (float)ray1_enter / (float)dir1.magnitude;

					ray2.origin = sub_inside_points[0];
					Vector3 dir2 = this.vertices[this.triangles[i + ((sub_add_vert_idx + 2) % 3)]] - sub_inside_points[0];
					ray2.direction = dir2.normalized;
					sub_plane.Raycast(ray2, out float ray2_enter);
					float lerp2 = (float)ray2_enter / (float)dir2.magnitude;


					sub_obj_triangles.Add(sub_obj_vertices.Count);
					sub_obj_vertices.Add(sub_inside_points[0]);
					sub_obj_triangles.Add(sub_obj_vertices.Count);
					sub_obj_vertices.Add(ray1.GetPoint(ray1_enter));
					sub_obj_triangles.Add(sub_obj_vertices.Count);
					sub_obj_vertices.Add(ray2.GetPoint(ray2_enter));


					sub_obj_normals.Add(sub_inside_normals[0]);
					sub_obj_normals.Add(Vector3.Lerp(sub_inside_normals[0], this.normals[this.triangles[i + ((sub_add_vert_idx + 1) % 3)]], lerp1));
					sub_obj_normals.Add(Vector3.Lerp(sub_inside_normals[0], this.normals[this.triangles[i + ((sub_add_vert_idx + 2) % 3)]], lerp2));


					if (!sub_edge_set)
					{
						sub_edge_set = true;
						sub_edge_vert = ray1.GetPoint(ray1_enter);
					}
					else
					{
						sub_edge_plane.Set3Points(sub_edge_vert, ray1.GetPoint(ray1_enter), ray2.GetPoint(ray2_enter));

						sub_obj_triangles.Add(sub_obj_vertices.Count);
						sub_obj_vertices.Add(sub_edge_vert);
						sub_obj_triangles.Add(sub_obj_vertices.Count);
						sub_obj_vertices.Add(sub_edge_plane.GetSide(sub_edge_vert + -sub_plane.normal) ? ray1.GetPoint(ray1_enter) : ray2.GetPoint(ray2_enter));
						sub_obj_triangles.Add(sub_obj_vertices.Count);
						sub_obj_vertices.Add(sub_edge_plane.GetSide(sub_edge_vert + -sub_plane.normal) ? ray2.GetPoint(ray2_enter) : ray1.GetPoint(ray1_enter));


						sub_obj_normals.Add(-sub_plane.normal);
						sub_obj_normals.Add(-sub_plane.normal);
						sub_obj_normals.Add(-sub_plane.normal);
					}
				}
				else if (sub_inside_points_count == 2)
				{
					ray1.origin = this.vertices[this.triangles[i + ((sub_add_vert_idx + 1) % 3)]];
					Vector3 dir1 = sub_outside_points[0] - this.vertices[this.triangles[i + ((sub_add_vert_idx + 1) % 3)]];
					ray1.direction = dir1.normalized;
					sub_plane.Raycast(ray1, out float ray1_enter);
					float lerp1 = (float)ray1_enter / (float)dir1.magnitude;

					ray2.origin = this.vertices[this.triangles[i + ((sub_add_vert_idx + 2) % 3)]];
					Vector3 dir2 = sub_outside_points[0] - this.vertices[this.triangles[i + ((sub_add_vert_idx + 2) % 3)]];
					ray2.direction = dir2.normalized;
					sub_plane.Raycast(ray2, out float ray2_enter);
					float lerp2 = (float)ray2_enter / (float)dir2.magnitude;


					sub_obj_triangles.Add(sub_obj_vertices.Count);
					sub_obj_vertices.Add(ray1.GetPoint(ray1_enter));
					sub_obj_triangles.Add(sub_obj_vertices.Count);
					sub_obj_vertices.Add(this.vertices[this.triangles[i + ((sub_add_vert_idx + 1) % 3)]]);
					sub_obj_triangles.Add(sub_obj_vertices.Count);
					sub_obj_vertices.Add(this.vertices[this.triangles[i + ((sub_add_vert_idx + 2) % 3)]]);


					sub_obj_normals.Add(Vector3.Lerp(this.normals[this.triangles[i + ((sub_add_vert_idx + 1) % 3)]], sub_outside_normals[0], lerp1));
					sub_obj_normals.Add(this.normals[this.triangles[i + ((sub_add_vert_idx + 1) % 3)]]);
					sub_obj_normals.Add(this.normals[this.triangles[i + ((sub_add_vert_idx + 2) % 3)]]);


					sub_obj_triangles.Add(sub_obj_vertices.Count);
					sub_obj_vertices.Add(ray1.GetPoint(ray1_enter));
					sub_obj_triangles.Add(sub_obj_vertices.Count);
					sub_obj_vertices.Add(this.vertices[this.triangles[i + ((sub_add_vert_idx + 2) % 3)]]);
					sub_obj_triangles.Add(sub_obj_vertices.Count);
					sub_obj_vertices.Add(ray2.GetPoint(ray2_enter));


					sub_obj_normals.Add(Vector3.Lerp(this.normals[this.triangles[i + ((sub_add_vert_idx + 1) % 3)]], sub_outside_normals[0], lerp1));
					sub_obj_normals.Add(this.normals[this.triangles[i + ((sub_add_vert_idx + 2) % 3)]]);
					sub_obj_normals.Add(Vector3.Lerp(this.normals[this.triangles[i + ((sub_add_vert_idx + 2) % 3)]], sub_outside_normals[0], lerp2));


					if (!sub_edge_set)
					{
						sub_edge_set = true;
						sub_edge_vert = ray1.GetPoint(ray1_enter);
					}
					else
					{
						sub_edge_plane.Set3Points(sub_edge_vert, ray1.GetPoint(ray1_enter), ray2.GetPoint(ray2_enter));

						sub_obj_triangles.Add(sub_obj_vertices.Count);
						sub_obj_vertices.Add(sub_edge_vert);
						sub_obj_triangles.Add(sub_obj_vertices.Count);
						sub_obj_vertices.Add(sub_edge_plane.GetSide(sub_edge_vert + -sub_plane.normal) ? ray1.GetPoint(ray1_enter) : ray2.GetPoint(ray2_enter));
						sub_obj_triangles.Add(sub_obj_vertices.Count);
						sub_obj_vertices.Add(sub_edge_plane.GetSide(sub_edge_vert + -sub_plane.normal) ? ray2.GetPoint(ray2_enter) : ray1.GetPoint(ray1_enter));


						sub_obj_normals.Add(-sub_plane.normal);
						sub_obj_normals.Add(-sub_plane.normal);
						sub_obj_normals.Add(-sub_plane.normal);
					}
				}
			}
		}

		this.mesh.Clear();
		this.mesh.vertices = obj_vertices.ToArray();
		this.mesh.triangles = obj_triangles.ToArray();
		this.mesh.normals = obj_normals.ToArray();
		//this.mesh.RecalculateBounds();
		this.mesh.Optimize();
		this.mesh_collider.sharedMesh = this.mesh;

		this.vertices.Clear();
		this.vertices = obj_vertices;
		this.triangles.Clear();
		this.triangles = obj_triangles;
		this.normals.Clear();
		this.normals = obj_normals;

		if (this.renderer.bounds.size.x <= this.main_obj_controller.min_poss_volume.x
			&& this.renderer.bounds.size.y <= this.main_obj_controller.min_poss_volume.y
			&& this.renderer.bounds.size.z <= this.main_obj_controller.min_poss_volume.z)
		{
			this.can_destruct = false;
		}


		obj_mesh.vertices = sub_obj_vertices.ToArray();
		obj_mesh.triangles = sub_obj_triangles.ToArray();
		obj_mesh.normals = sub_obj_normals.ToArray();
		//obj_mesh.RecalculateBounds();
		obj_mesh.Optimize();


		MeshRenderer obj_renderer = obj.AddComponent<MeshRenderer>();
		obj_renderer.materials = this.mesh_renderer.materials;
		MeshFilter obj_filter = obj.AddComponent<MeshFilter>();
		obj_filter.mesh = obj_mesh;

		MeshCollider col = obj.AddComponent<MeshCollider>();
		col.sharedMesh = obj_mesh;
		col.convex = true;
		Rigidbody rb = obj.AddComponent<Rigidbody>();

		destruction destruction_script = obj.AddComponent<destruction>();
		destruction_script.main_obj_controller = this.main_obj_controller;
	}
}
