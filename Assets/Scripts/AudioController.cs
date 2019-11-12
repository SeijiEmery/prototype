using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PlayerController))]
public class AudioController : MonoBehaviour {
    [System.Serializable]
    public struct AudioConfig {
        public AudioClip track;
        
        [Range(0.2f, 2.5f)]
        public float tempo;
    }
    public AudioSource target;
    public AudioConfig[] tracks;
    public int currentTrack = 0;
    public bool play = true;

    private PlayerController player; 

    // Start is called before the first frame update
    void Start() {
        player = GetComponent<PlayerController>();
        PlayRandomTrack();
    }
    public void PlayRandomTrack() {
        SetCurrentTrack(Mathf.FloorToInt(Random.Range(0f, tracks.Length)));
    }
    public void PlayNextTrack() {
        SetCurrentTrack(currentTrack+1);
    }
    public void PlayPrevTrack() {
        SetCurrentTrack(currentTrack-1);
    }
    public void SetCurrentTrack(int track) {
        if (track < 0) track = tracks.Length - 1;
        else if (track >= tracks.Length) track = 0;
        var prevTrack = currentTrack;
        currentTrack = track;
        Debug.Log("Playing track "+track+" => "+currentTrack+", prev track = "+prevTrack);
        if (currentTrack >= 0) {
            target.Stop();
            target.PlayOneShot(tracks[currentTrack].track);
            play = true;
            player.SetTempo(tracks[currentTrack].tempo);
        } else {
            target.Stop();
            play = false;
            player.SetTempo(1f);
        }
    }

    private bool trackButtonWasPressed = false;
    
    // Update is called once per frame
    void Update() {
        var gamepad = Gamepad.current;
        if (gamepad != null) {
            bool nextTrackButton = gamepad.dpad.up.isPressed;
            bool prevTrackButton = gamepad.dpad.down.isPressed;
            bool randomTrackButton = gamepad.dpad.right.isPressed;
            if (!trackButtonWasPressed && nextTrackButton) {
                PlayNextTrack();
            } else if (!trackButtonWasPressed && prevTrackButton) {
                PlayPrevTrack();
            } else if (!trackButtonWasPressed && randomTrackButton) {
                PlayRandomTrack();
            }
            trackButtonWasPressed = nextTrackButton || prevTrackButton || randomTrackButton;
        }
        if (play && !target.isPlaying) {
            PlayNextTrack();
        }
    }
}
