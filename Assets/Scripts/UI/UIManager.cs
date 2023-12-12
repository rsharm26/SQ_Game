using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager {
    private static UIContainer _instance;

    public static UIContainer GetInstance() {
        if (_instance == null) {
            _instance = Resources.Load<UIContainer>("UIContainerObject");
        }

        return _instance;
    }
}
