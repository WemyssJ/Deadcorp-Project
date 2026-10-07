using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    //public GameObject _player;
    public int iTrackPicker;
    public bool iToggleChange;
    public GameObject _player;


    void Update()
    {
        

        if (_player.GetComponent<Player>().isDead)
        {
            iToggleChange = true;
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "Player")
        {
            collision.GetComponent<MusicManager>().TrackPicker = iTrackPicker;
            collision.GetComponent<MusicManager>().ToggleChange = iToggleChange;
            iToggleChange = false;
        }
    }
}
