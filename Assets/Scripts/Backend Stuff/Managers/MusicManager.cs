using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] MusicLibary;
    public AudioClip MusicPlaying;
    public AudioSource BackgroundMusic;

    public int TrackPicker;
    public bool ToggleChange;
    bool PlayMusic;
    // Start is called before the first frame update
    void Start()
    {
        
        TrackPicker = 0;
        MusicPlaying = MusicLibary[TrackPicker];
        BackgroundMusic.clip = MusicPlaying;
        

        PlayMusic = true;
        ToggleChange = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayMusic && ToggleChange)
        {
            ToggleChange = false;
            MusicPlaying = MusicLibary[TrackPicker];
            BackgroundMusic.clip = MusicPlaying;
            BackgroundMusic.Play();
            
        }

        if (!PlayMusic && ToggleChange)
        {
            ToggleChange = false;
            BackgroundMusic.Stop();

        }

    }


}
