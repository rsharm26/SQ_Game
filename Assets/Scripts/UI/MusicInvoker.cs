using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicInvoker : MonoBehaviour {
    [SerializeField]
    private SongType _songType; 

    void Start() {
        MusicManager.GetInstance().PlayMusic(_songType);
    }
}
