using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "../Resources/UIContainerObject", menuName = "ScriptableObjects/UIContainer")]
public class UIContainer : ScriptableObject {
    // This will be exposed to the editor, where dev can add their prefab + UI type.
    [SerializeField]
    private List<PrefabbedUIElement> _uiPrefabs;

    // Also specify sort order.
    [SerializeField]
    private int _sortingOrder;

    // Property that calling classes can use to check currently active before invoking the toggle method.
    // SEE IF YOU CAN REVISE.
    public UIType? CurrentlyActive { get; private set; }

    // Allow dev to include their prefabbed UI element and specify its UI type (from enum above).
    [Serializable]
    public class PrefabbedUIElement {
        public UIType uiType;
        public GameObject uiElement;
    }

    // Sadly must use this to map UI types to their instantiated objects.
    // Re-using the list from above results in unpredictable behavior.
    private Dictionary<UIType, GameObject> _instantiatedUIElements = new Dictionary<UIType, GameObject>();

    // Call this method from other objects to render the desired UI element.
    // It will also instantiate the UI element beforehand. NOTE should prob move to own function.
    // In other words, we can lazy load.
    public void ToggleUIElement(UIType type) {
        PrefabbedUIElement desiredElement = _uiPrefabs.Find(prefab => prefab.uiType == type);

        // Must instantiate a game object before trying to use it (does not exist otherwise).
        if (_instantiatedUIElements.ContainsKey(desiredElement.uiType) == false) {
            _instantiatedUIElements.Add(type, Instantiate(desiredElement.uiElement));

            DontDestroyOnLoad(_instantiatedUIElements[type]);
        }

        // Simply modify display USS property to show/hide menu.
        // Adjust currentlyActive menu so we can track which menu is open.
        UIDocument uiElement = _instantiatedUIElements[type].GetComponent<UIDocument>();
        StyleEnum<DisplayStyle> currentDisplay = uiElement.rootVisualElement.style.display;

        if (currentDisplay == DisplayStyle.Flex) {
            uiElement.rootVisualElement.style.display = DisplayStyle.None;
            CurrentlyActive = null;
        } else {
            uiElement.rootVisualElement.style.display = DisplayStyle.Flex;
            CurrentlyActive = type;
        }

        uiElement.sortingOrder = _sortingOrder; // Arbitrary, revise.
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