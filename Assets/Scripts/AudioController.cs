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
    public int currentTrack = 0;
    public AudioConfig[] tracks;
    public bool play = true;
    public static float tempo = 1.0f;
    private PlayerController player; 

    // Start is called before the first frame update
    void Start() {
        player = GetComponent<PlayerController>();
        SetCurrentTrack(currentTrack);
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
            if (player != null)
                player.SetTempo(tracks[currentTrack].tempo);
            tempo = tracks[currentTrack].tempo;
        } else {
            target.Stop();
            play = false;
            if (player != null)
                player.SetTempo(1f);
            tempo = 1f;
        }
    }

    private bool trackButtonWasPressed = false;
    private bool kbTrackButtonWasPressed = false;
    
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

        var kb = Keyboard.current;
        if (kb != null) {
            bool nextTrackButton = kb.rightArrowKey.isPressed;
            bool prevTrackButton = kb.leftArrowKey.isPressed;
            bool randomTrackButton = kb.spaceKey.isPressed;
            if (!kbTrackButtonWasPressed && nextTrackButton) {
                PlayNextTrack();
            } else if (!kbTrackButtonWasPressed && prevTrackButton) {
                PlayPrevTrack();
            } else if (!kbTrackButtonWasPressed && randomTrackButton) {
                PlayRandomTrack();
            }
            kbTrackButtonWasPressed = nextTrackButton || prevTrackButton || randomTrackButton;
        }
        if (play && !target.isPlaying) {
            PlayNextTrack();
        }
    }
}
