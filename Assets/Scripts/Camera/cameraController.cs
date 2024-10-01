using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    public float leftMin;
    public float rightMin;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = Mathf.Clamp(transform.position.x, leftMin, rightMin);
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
        //Debug.Log(transform.position);
    }
}
