using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenOnBossDeath : MonoBehaviour
{
    public GameObject _enemy;
    public GameObject EntryDoor1;
    public GameObject EntryDoor2;
    public GameObject Billboard1;

    public int iTrackPicker;
    public bool iToggleChange;
    public GameObject _player;

    private void Awake()
    {
        if (Billboard1 != null)
        {
            Billboard1.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_enemy.GetComponent<Enemy>().isDead)
        {
            
            _player.GetComponent<MusicManager>().TrackPicker = iTrackPicker;
            _player.GetComponent<MusicManager>().ToggleChange = iToggleChange;
            if (Billboard1 != null)
            {
                Billboard1.SetActive(true);
            }
            Destroy(EntryDoor1);
            Destroy(EntryDoor2);
            StartCoroutine(Wait(0.1f));
            
            
        }
    }
    IEnumerator Wait(float duration)
    {
        //Debug.Log($"Started at {Time.time}, waiting for {duration} seconds");
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);

    }
}
