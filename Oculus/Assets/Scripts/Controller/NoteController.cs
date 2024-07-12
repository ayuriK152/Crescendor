using System;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    void Update()
    {
        if (transform.position.z > -5.0f && transform.position.z < 5.0f)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
