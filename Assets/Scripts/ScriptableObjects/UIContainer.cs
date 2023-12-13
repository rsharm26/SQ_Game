/* Filename: UIContainer.cs
 * Project: SQ Term Project PixelAndysAdventure
 * By: Rohin Sharma
 * Date: December 13, 2023
 * Description: This file houses a scriptable object used to store session-wide UI elements.
                Note this is admittedly a strange way to use scriptable object, though highly convenient.
                It also makes sense as UI elements should persist across scenes/within a session.
                Note the actual scriptable component object lives in Resources.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;



/*  
 * Class: UIContainer.
 * Purpose: This class is effectively a DTO for UI elements.
 * It is similar in nature to DyamicGameData.
 * Note that it is a scriptable object, meaning we're basically using a data container with session persistence.
 */
[CreateAssetMenu(fileName = "../Resources/UIContainerObject", menuName = "ScriptableObjects/UIContainer")]
public class UIContainer : ScriptableObject {
    // Attributes.
    [SerializeField]
    private List<PrefabbedUIElement> _uiPrefabs; // Similar to music container list, exposed to editor / convenient for dev to adjust.
    
    // Sadly must use this to map UI types to their instantiated objects.
    // Re-using the list from above results in unpredictable behavior.
    // NOTE can mimic approach from music container, do this in future version (no time).
    private Dictionary<UIType, GameObject> _instantiatedUIElements = new Dictionary<UIType, GameObject>();
    private int _sortingOrder = 0; // Sorting order that elements should use, helps when drawing many elements at once (higher sort order = on top).

    // Properties.
    public UIType? CurrentlyActive { get; private set; } // Track which element is active,

    // Sub-class.
    // This class is effectively a struct that holds the UI type and its prefabbed element.
    [Serializable]
    public class PrefabbedUIElement {
        public UIType uiType;
        public GameObject uiElement;
    }


    /*
     * Method: ToggleUIElement() -- Method with one parameter.
     * Description: This method shall instantiate (if necessary) and display a UI element.
     * Parameters: UIType type: an enum value specifying the UI type to display.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    public void ToggleUIElement(UIType type) {
        PrefabbedUIElement desiredElement = _uiPrefabs.Find(prefab => prefab.uiType == type);

        // Must instantiate a game object before trying to use it (does not exist otherwise).
        // Lazy loading.
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
            uiElement.sortingOrder = ++_sortingOrder;
        }
    }


    /*
     * Method: CloseAllActiveElements() -- Method with no parameters.
     * Description: This convenience method simply closes all instantiated window elements..
     * Parameters: None.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    public void CloseAllActiveElements() {
        foreach (GameObject uiElement in  _instantiatedUIElements.Values) {
            uiElement.GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.None;
            CurrentlyActive = null;
        }
    }
}



// This enum is attached here out of convenience, it is a convenient way to specify desired UI type.
// Enums appear as lists in the unity editor, making it easy for developers to use it and also when using methods...
// ... like ToggleUIElement that take an enum (clearer than using an int or string or something else). 
public enum UIType {
    LevelSelection,
    Leaderboard,
    Settings,
    PauseMenu,
    WinLossMenu,
    InLevelOverlay
}