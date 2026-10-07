using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendCameraSettings : MonoBehaviour
{
    public GameObject _Camera;
    public float aheadDistanceX;
    public float aheadDistanceY;
    public float CameraSpeed;

    public bool FollowPlayer;

    //public float lookAheadX;
    //public float lookAheadY;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            _Camera.GetComponent<CameraController>().aheadDistanceY = aheadDistanceY;
            _Camera.GetComponent<CameraController>().aheadDistanceX = aheadDistanceX;
            _Camera.GetComponent<CameraController>().CameraSpeed = CameraSpeed;
            //_Camera.GetComponent<CameraController>().lookAheadY = lookAheadY;
            //_Camera.GetComponent<CameraController>().lookAheadX = lookAheadX;
            StartCoroutine(Wait(0.1f));

        }
    }
    IEnumerator Wait(float duration)
    {
        //Debug.Log($"Started at {Time.time}, waiting for {duration} seconds");
        yield return new WaitForSeconds(duration);
        _Camera.GetComponent<CameraController>().FollowPlayer = FollowPlayer;

    }

}
