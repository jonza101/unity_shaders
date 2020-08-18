using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class reflection : MonoBehaviour
{
	public Vector2Int resolution;
	public float refl_cam_fov;
	public RenderTextureFormat texture_format;

	public float clip_plane_offset;

	public LayerMask layer_mask;

	public Material water_mat;
	private RenderTexture rt;

	private bool inside_rendering = false;
	private GameObject	refl_cam_go;
	private Camera		refl_cam;


	void	Start()
    {
		this.rt = new RenderTexture(this.resolution.x, this.resolution.y, 16, this.texture_format);
		this.rt.Create();

		this.refl_cam_go = new GameObject("refl cam", typeof(Camera), typeof(Skybox));
		this.refl_cam = this.refl_cam_go.GetComponent<Camera>();
		this.refl_cam.enabled = false;
		this.refl_cam.fieldOfView = this.refl_cam_fov;
		this.refl_cam.transform.position = this.transform.position;
		this.refl_cam.transform.rotation = this.transform.rotation;
		this.refl_cam.gameObject.AddComponent<FlareLayer>();
		this.refl_cam_go.hideFlags = HideFlags.HideAndDontSave;
	}

	void	OnWillRenderObject()
    {
		if (this.inside_rendering)
			return;
		this.inside_rendering = true;

		Camera cam = Camera.current;


		float d = -Vector3.Dot(this.transform.up, this.transform.position) - this.clip_plane_offset;
		Vector4 refl_plane = new Vector4(this.transform.up.x, this.transform.up.y, this.transform.up.z, d);

		Matrix4x4 refl_m = Matrix4x4.zero;
		CalculateReflectionMatrix(ref refl_m, refl_plane);
		Vector3 old_pos = cam.transform.position;
		Vector3 new_pos = refl_m.MultiplyPoint(old_pos);
		this.refl_cam.worldToCameraMatrix = cam.worldToCameraMatrix * refl_m;

		Vector4 clip_plane = CameraSpacePlane(this.refl_cam, this.transform.position, this.transform.up, 1.0f);
		Matrix4x4 proj = cam.CalculateObliqueMatrix(clip_plane);
		this.refl_cam.projectionMatrix = proj;

		this.refl_cam.cullingMask = ~(1 << 4) & this.layer_mask.value;
		this.refl_cam.targetTexture = this.rt;
		GL.invertCulling = true;
		this.refl_cam.transform.position = new_pos;
		Vector3 euler = cam.transform.eulerAngles;
		this.refl_cam.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);
		this.refl_cam.Render();
		this.refl_cam.transform.position = old_pos;
		GL.invertCulling = false;
		this.water_mat.SetTexture("_refl_map", this.rt);

		this.inside_rendering = false;
	}

	// Given position/normal of the plane, calculates plane in camera space.
	private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 offsetPos = pos + normal * this.clip_plane_offset;
		Matrix4x4 m = cam.worldToCameraMatrix;
		Vector3 cpos = m.MultiplyPoint(offsetPos);
		Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
	}

	// Calculates reflection matrix around the given plane
	private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
	{
		reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
		reflectionMat.m01 = (-2F * plane[0] * plane[1]);
		reflectionMat.m02 = (-2F * plane[0] * plane[2]);
		reflectionMat.m03 = (-2F * plane[3] * plane[0]);

		reflectionMat.m10 = (-2F * plane[1] * plane[0]);
		reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
		reflectionMat.m12 = (-2F * plane[1] * plane[2]);
		reflectionMat.m13 = (-2F * plane[3] * plane[1]);

		reflectionMat.m20 = (-2F * plane[2] * plane[0]);
		reflectionMat.m21 = (-2F * plane[2] * plane[1]);
		reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
		reflectionMat.m23 = (-2F * plane[3] * plane[2]);

		reflectionMat.m30 = 0F;
		reflectionMat.m31 = 0F;
		reflectionMat.m32 = 0F;
		reflectionMat.m33 = 1F;
	}
}
