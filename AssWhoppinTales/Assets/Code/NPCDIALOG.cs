using UnityEngine;

[CreateAssetMenu(fileName = "NPCDIALOG", menuName = "NPCDIALOG")]
public class NPCDIALOG : ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;
    public string[] dialogueLines;
    public bool[] autoProgressLines;
    public float autoProgressDelay = 2f;
    public float typingspeed = 0.05f;
    public AudioClip voiceSound;
    public float voicePitch = 1.5f;
    public float autoProgressTime = 2f;
}
