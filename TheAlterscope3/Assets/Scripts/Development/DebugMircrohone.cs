using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class DebugMircrohone : MonoBehaviour
{
    // Start is called before the first frame update
    Keyboard _keyboard;
    bool _recording;
    public AudioSource MyAudio;
    AudioClip _myClip;
    void Start()
    {
        _keyboard = Keyboard.current;
        MyAudio = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        if (_keyboard.spaceKey.isPressed && _recording == false)
        {
            Debug.Log("space key pressed");
            _myClip = Microphone.Start("Built-in Microphone", true, 3, 44100);
            _recording = true;
        }
        if (_recording == true && _keyboard.spaceKey.isPressed == false)
        {
            Debug.Log("space key up");
            Microphone.End("Built-in Microphone");
            MyAudio.clip = _myClip;
            _recording = false;
        }
        if (_keyboard.pKey.isPressed && MyAudio.isPlaying == false)
        {
            MyAudio.Play();
            Debug.Log("P key pressed");
        }


    }
}
