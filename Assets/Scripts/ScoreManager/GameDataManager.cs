using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour 
{
    private static DynamicGameData _instance;

    public static DynamicGameData GetInstance() 
    {
        if (_instance == null) 
        {
            _instance = Resources.Load<DynamicGameData>("DynamicGameDataObject");
        }

        return _instance;
    }
}
