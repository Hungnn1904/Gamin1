using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDIALOG dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    // Kiểm tra điều kiện có thể tương tác
    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    // Xử lý khi có tương tác
    public void Interact()
    {
        if (dialogueData == null)
            return;

        if (!isDialogueActive)
        {
            StartDialogue();
        }
        else
        {
            NextLine();
        }
    }

    // Khởi tạo hội thoại
    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;
        nameText.SetText(dialogueData.name);
        portraitImage.sprite = dialogueData.npcPortrait;
        dialoguePanel.SetActive(true);
        StartCoroutine(TypeLine());
    }

    // Chuyển sang dòng hội thoại tiếp theo
    void NextLine()
    {
        if (isTyping)
        {
            // Nếu đang đánh máy, hiển thị ngay dòng hiện tại
            dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }
        else if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            // Nếu còn dòng hội thoại khác, bắt đầu đánh máy dòng tiếp theo
            StartCoroutine(TypeLine());
        }
        else
        {
            // Kết thúc hội thoại
            EndDialogue();
        }
    }

    // Coroutine đánh máy từng ký tự của dòng hội thoại
    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");
        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingspeed);
        }

        isTyping = false;
        // Tự động chuyển sang dòng tiếp theo nếu được cấu hình tự động
        if (dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }

    // Kết thúc hội thoại và tắt UI
    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);
    }
}
