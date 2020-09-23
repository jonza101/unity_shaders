using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grass_interaction : MonoBehaviour
{
	[SerializeField]
	private Material grass_mat;

    
    void Update()
    {
		grass_mat.SetVector("_interact_pos", new Vector4(transform.position.x, transform.position.y, transform.position.z, 1.0f));
    }
}
