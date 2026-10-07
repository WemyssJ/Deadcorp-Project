using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{

    private float startPos, length;
    public GameObject cam;
    
    // The speed at which the background should move relative to the camera 
    public float parallaxEffect; 


    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // calculate distance background should move based on camera movement
        float distance = cam.transform.position.x * parallaxEffect; // 0 = move with camera || 1 = wont move || 0.5 half speed 

        float movement = cam.transform.position.x * (1 - parallaxEffect);

        transform.position = new Vector3 (startPos + distance, transform.position.y, transform.position.z);

        //infinite scrolling 

        if (movement > startPos + length)
        {
            startPos += length;
        }
        else if (movement < startPos - length) 
        {
            startPos -= length;
        }

    }
}
