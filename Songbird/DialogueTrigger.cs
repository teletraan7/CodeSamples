using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTrigger : MonoBehaviour {

    private bool triggering;

    public int dialougeGate = 0;
    public int dialogIndex = 0;
    public bool goToNextSentence = false;
    public List<Dialogue> npcText = new List<Dialogue>();
    public DialogueManager dm = null;
    public Animator interactButtonFade = null;

    [SerializeField]
    protected musicUI_Manager musicManager = null;

    protected int currentDialougeElement = 0;

    //find the dialog manager trigger function
    public void TriggerDialogue()
    {
        dm.StartDialogue(npcText[currentDialougeElement], this);
    }


    public void ChangeToNextDialougeElement ()
    {
        if ((currentDialougeElement + 1) <= npcText.Count && npcText[currentDialougeElement].goodToContinue)
        {
            currentDialougeElement++;
        }
        if (currentDialougeElement == 1)
        {
            musicManager.MusicLocked = false;
        }
    }

    
    public void QuestActionCompelete ()
    {
        npcText[currentDialougeElement].goodToContinue = true;
        ChangeToNextDialougeElement();
    }


    //find the dialog manager advance text function
    public void AdvanceToNextSentence()
    {
        dm.DisplayNextSentence();
    }


    //determine if in range
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            triggering = true;
            interactButtonFade.SetBool("isOpen", true);
        }
    }


    protected void OnTriggerExit2D(Collider2D collision )
    {
        if (collision.tag == "Player")
        {
            triggering = false;
            interactButtonFade.SetBool("isOpen", false);
        }
    }


    //if in range, run the trigger dialog function
    void Update()
    {
        if (triggering && currentDialougeElement < npcText.Count)
        {
            if (Input.GetButtonDown("test"))
            {

                dialougeGate++;
                if (dialogIndex == 0)
                {
                    TriggerDialogue();
                }

                else if (dialougeGate == 3 || goToNextSentence == true)
                {
                    AdvanceToNextSentence();
                }
            }
        }
    }
}