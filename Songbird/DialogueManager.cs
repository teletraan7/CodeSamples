using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour   
{
    public Text dialogueText;
    public Queue<string> sentences;
    public DialogueTrigger dt;
    public Animator dialoguewindowAnimator;
    public PlayerController playerController;
    private Coroutine typingCoroutine = null;
    
    void Start()
    {
        sentences = new Queue<string>();      
    }

    public void StartDialogue (Dialogue dialogue, DialogueTrigger trigger)
    {
        playerController.disablemovement();
        playerController.enabled = false;
        dialoguewindowAnimator.SetBool("isOpen", true);

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        dt = trigger;
        DisplayNextSentence();
    }

   public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        dt.dialogIndex = dt.dialogIndex + 1;

        string sentence = sentences.Dequeue();
        if (typingCoroutine != null)
        {
            StopAllCoroutines();
            dt.dialougeGate = 1;
            typingCoroutine = null;
        }
        typingCoroutine = StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence (string sentence)
    {
        Debug.Log("Running routine");
        dialogueText.text = "";
        dt.goToNextSentence = false;
        float dialougeSpeed = 0f;

        foreach (char letter in sentence .ToCharArray())
        {
            if (dt.dialougeGate == 1)
            {
                dialougeSpeed = 0.05f;
            }
            else if (dt.dialougeGate == 2)
            {
                dialougeSpeed = 0.000001f;
            }

            dialogueText.text += letter;
            yield return new WaitForSeconds(dialougeSpeed);
               
        }

        dt.dialougeGate = 0;
        dt.goToNextSentence = true;
        typingCoroutine = null;
    }

    public void EndDialogue()
    {
        playerController.enabled = true;
        dialoguewindowAnimator.SetBool("isOpen", false);
        dt.dialogIndex = 0;
        dt.dialougeGate = 0;
        StopAllCoroutines();
        dt.ChangeToNextDialougeElement();
        dt = null;
    }
}