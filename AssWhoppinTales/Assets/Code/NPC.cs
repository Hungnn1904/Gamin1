using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDIALOG dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;
    public bool CanInteract()
    {
        return !isDialogueActive;
    }
    public void Interact(){
        if(dialogueData == null || (!isDialogueActive)) return;
        if(isDialogueActive){
            
        }
        else{
            StartDialogue();
        }
    }
    void StartDialogue(){
        isDialogueActive = true;
        dialogueIndex = 0;
        nameText.SetText(dialogueData.name);
        portraitImage.sprite = dialogueData.npcPortrait;
        dialoguePanel.SetActive(true);
        StartCoroutine(TypeLine());
    }
    void NextLine(){
        if(isTyping){
            dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }
        else if(++dialogueIndex < dialogueData.dialogueLines.Length){
            //if another line , type next line
            StartCoroutine(TypeLine());
        }
        else{
            EndDialogue();
        }
    }
    IEnumerator TypeLine(){
        isTyping = true;
        dialogueText.SetText("");
        foreach(char letter in dialogueData.dialogueLines[dialogueIndex]){
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingspeed);
        }    
        isTyping = false;
        if(dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex]){
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }
    public void EndDialogue(){
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);
    }
}

