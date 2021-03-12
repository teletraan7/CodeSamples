using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Dialogue {

    public bool goodToContinue = false;
    [Tooltip("This is the gameobject that the player will need to interact with in some way to progress the dialogue.")]
    public GameObject objectToInteractWith = null;
    [TextArea(3, 10)]
    public string[] sentences;
}
