using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

// Siilar to UI container.

[CreateAssetMenu(fileName = "../Resources/MusicContainerObject", menuName = "ScriptableObjects/MusicContainer")]
public class MusicContainer : ScriptableObject {
    [SerializeField]
    private List<MappedSong> _music;

    private float _volume = 1.0f;

    [Serializable]
    public class MappedSong {
        public SongType songType;
        public AudioSource audioSource;
        public AudioSource useableAudio;
    }

    public void PlayMusic(SongType songType) {
        MappedSong desiredTune = _music.Find(clip => clip.songType == songType);

        if (desiredTune.audioSource != null) {
            desiredTune.useableAudio = Instantiate(desiredTune.audioSource);
            desiredTune.useableAudio.volume = _volume;
            desiredTune.useableAudio.Play();
        } 
    }

    public void AdjustVolume(float newVolume) {
        _volume = newVolume;

        foreach (MappedSong tune in _music) {
            if (tune.useableAudio != null) {
                tune.useableAudio.volume = newVolume;
            }
        }
    }
}


public enum SongType {
    MainMenuMusic,
    Level1Music,
    Level2Music,
    Level3Music,
    Level4Music,
    Level5Music,
}