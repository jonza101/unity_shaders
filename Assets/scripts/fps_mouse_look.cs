using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fps_mouse_look : MonoBehaviour
{
	public GameObject   player;

    public float		sensitivity_x;
    public float		sensitivity_y;

    public float		min_x_rotation;
    public float		max_x_rotation;
    private float		x_rot = 0.0f;
    private float		y_rot = 0.0f;


    void    Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void    Update()
    {
        float mouse_x = Input.GetAxis("Mouse X") * this.sensitivity_x;// * Time.deltaTime;
        float mouse_y = Input.GetAxis("Mouse Y") * this.sensitivity_y;// * Time.deltaTime;
        this.x_rot = Mathf.Clamp(this.x_rot - mouse_y, this.min_x_rotation, this.max_x_rotation);
        this.y_rot += mouse_x;

        transform.localRotation = Quaternion.Euler(this.x_rot, 0.0f, 0.0f);
		player.transform.Rotate(Vector3.up * mouse_x);
    }
}
