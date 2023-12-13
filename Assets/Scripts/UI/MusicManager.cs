using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager {
    private static MusicContainer _instance;

    public static MusicContainer GetInstance() {
        if (_instance == null) {
            _instance = Resources.Load<MusicContainer>("MusicContainerObject");
        }

        return _instance;
    }
}
