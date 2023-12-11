using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "../Resources/UIContainerObject", menuName = "ScriptableObjects/UIContainer")]
public class UIContainer : ScriptableObject {
    // This will be exposed to the editor, where dev can add their prefab + UI type.
    [SerializeField]
    private List<PrefabbedUIElement> uiPrefabs;

    // Allow dev to include their prefabbed UI element and specify its UI type (from enum above).
    [Serializable]
    public class PrefabbedUIElement {
        public UIType uiType;
        public GameObject uiElement;
    }

    // Sadly must use this to map UI types to their instantiated objects.
    // Re-using the list from above results in unpredictable behavior.
    private Dictionary<UIType, GameObject> instantiatedUIElements = new Dictionary<UIType, GameObject>();

    // Call this method from other objects to render the desired UI element.
    // It will also instantiate the UI element beforehand. NOTE should prob move to own function.
    // In other words, we can lazy load.
    public void InvokeUIElement(UIType type) {
        PrefabbedUIElement desiredElement = uiPrefabs.Find(prefab => prefab.uiType == type);

        // Must instantiate a game object before trying to use it (does not exist otherwise).
        if (instantiatedUIElements.ContainsKey(desiredElement.uiType) == false) {
            instantiatedUIElements.Add(type, Instantiate(desiredElement.uiElement));
        }

        // Simply modify display USS property to "render" UI element.
        instantiatedUIElements[type].GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.Flex;
    }
}

// Use enum so calling objects have easy time to invoke a UI type of their choice.
public enum UIType {
    LevelSelection,
    Leaderboard,
    Settings,
    PauseMenu,
    WinLossMenu
}