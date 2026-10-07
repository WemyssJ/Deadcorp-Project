using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCloseOnPass : MonoBehaviour
{
    public GameObject _player;
    public GameObject _door;


    private void Awake()
    {
        _door.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if(_player.GetComponent<Player>().isDead)
        {
            _door.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            _door.SetActive(true);
        }
    }
}
