/* Filename: MusicContainer.cs
 * Project: SQ Term Project PixelAndysAdventure
 * By: Rohin Sharma
 * Date: December 13, 2023
 * Description: This file houses a scriptable object used to store session-wide data relating to music and its options.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;



/*  
 * Class: MusicContainer.
 * Purpose: This class is effectively a DTO for sound files.
 * It is similar in nature to DyamicGameData, albeit much simpler as there are no events (methods used instead).
 * Note that it is a scriptable object, meaning we're basically using a data container with session persistence.
 */
[CreateAssetMenu(fileName = "../Resources/MusicContainerObject", menuName = "ScriptableObjects/MusicContainer")]
public class MusicContainer : ScriptableObject {
    // Attributes.
    [SerializeField]
    private List<MappedSong> _music; // Exposed to the developer in unity editor, can add songs easily to this list.

    // Sub-class.
    // This class is effectively a struct that holds the song type and audio source, alongside an instantiated audio source.
    // Important to note that the audioSource field is technically a prefab and ALL prefabs must be instantiated before usage. 
    [Serializable]
    public class MappedSong {
        public SongType songType; // Music type.
        public AudioSource audioSource; // Prefabbed audio source.

        [HideInInspector]
        public AudioSource useableAudio; // Instantiated audio source (hidden from unity editor).
    }



    /*
     * Method: PlayMusic() -- Method with one parameter.
     * Description: This method shall instantiate (if necessary) and play an audio file.
     * Parameters: SongType songType: an enum value indicating the desired song.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    public void PlayMusic(SongType songType) {
        MappedSong desiredTune = _music.Find(clip => clip.songType == songType);

        // Lazy loading.
        if (desiredTune.audioSource != null) {
            desiredTune.useableAudio = Instantiate(desiredTune.audioSource);
            desiredTune.useableAudio.volume = PlayerPrefs.GetFloat("volume", 50.0f) / 100;
            desiredTune.useableAudio.Play();
        } 
    }

    /*
     * Method: AdjustVolume() -- Method with one parameter.
     * Description: This method shall adjust the volume of each sound file to the desired volume level.
                    This is needed when attempting to change to volume level from settings.
     * Parameters: float newVolume: an float value indicating the desired volume level.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    public void AdjustVolume(float newVolume) {
        foreach (MappedSong tune in _music) {
            if (tune.useableAudio != null) {
                tune.useableAudio.volume = newVolume;
            }
        }
    }
}


// This enum is attached here out of convenience, it is a convenient way to specify desired song type.
// Enums appear as lists in the unity editor, making it easy for developers to use it and also when using methods...
// ... like PlayMusic that take an enum (clearer than using an int or string or something else). 
public enum SongType {
    MainMenuMusic,
    Level1Music,
    Level2Music,
    Level3Music,
    Level4Music,
    Level5Music,
}