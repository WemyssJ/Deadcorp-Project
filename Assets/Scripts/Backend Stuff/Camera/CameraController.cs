using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Room Camera
    [SerializeField] private float speed;
    private float currentPosX;
    private Vector3 velocity = Vector3.zero;

    //Follow Player
    public Transform Player;
    public float aheadDistanceX;
    public float aheadDistanceY;
    public float CameraSpeed;
    public float lookAheadX;
    public float lookAheadY;

    //Pick
    public bool FollowPlayer;

    // Start is called before ;the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (FollowPlayer == true)
        {
            //Follow player
            transform.position = new Vector3(Player.position.x + lookAheadX, Player.position.y + lookAheadY, transform.position.z);
            lookAheadX = Mathf.Lerp(lookAheadX, (aheadDistanceX * Player.localScale.x), Time.deltaTime * CameraSpeed);
            lookAheadY = Mathf.Lerp(lookAheadY, (aheadDistanceY * Player.localScale.y), Time.deltaTime * CameraSpeed);
        }
        else
        {
            //Room Camera
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(currentPosX, transform.position.y, transform.position.z), ref velocity, speed);
        }
    }

    public void MoveToNewRoom(Transform _NewRoom)
    {
        currentPosX = _NewRoom.position.x;
    }
}
