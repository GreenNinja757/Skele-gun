using UnityEngine;
using TMPro; 

public class NPCPrompt : MonoBehaviour
{
    public GameObject promptText; // Assign the NPCPromptText UI element in the Inspector

    private void Start()
    {
        promptText.SetActive(false); // Hide text at the start
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            promptText.SetActive(true); // Show the message when the player enters
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            promptText.SetActive(false); // Hide the message when the player leaves
        }
    }
}