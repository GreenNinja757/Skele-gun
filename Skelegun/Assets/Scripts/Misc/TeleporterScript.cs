using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class TeleporterScript : MonoBehaviour
{
    private Vector3 destination;
    public bool isFinalExit = false;
    public string nextLevelScene; // Set by the spawner
    private static bool isTeleporting = false;  // Shared Teleporter Cooldown 
    private static float teleportCooldown = 2f;  // Delay to prevent looping
    private static Image fader;

    private void Start()
    {
        if (fader == null)
        {
            fader = GameObject.Find("fader")?.GetComponent<Image>();
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTeleporting)
        {
            StartCoroutine(TeleportPlayer(other.transform));
        }
    }
    private IEnumerator TeleportPlayer(Transform player)
    {
        isTeleporting = true;
        yield return StartCoroutine(FadeScreen(1f)); 
        if (isFinalExit)
        {
            // If final exit, load the next level
            SceneManager.LoadScene(nextLevelScene);
        }
        else if (destination != Vector3.zero)
        {
            // Move the player to the destination teleporter position
            player.position = destination;
        }
        yield return StartCoroutine(FadeScreen(0f));
        yield return new WaitForSeconds(teleportCooldown);
        isTeleporting = false;
    }
    public void SetDestination(Vector3 targetPosition)
    {
        destination = targetPosition;

        if(destination == Vector3.zero)
    {
            gameObject.SetActive(false); // Disable teleporters that don't get a valid destination
        }

    }
    private IEnumerator FadeScreen(float targetAlpha)
    {
        if (fader == null) yield break;

        float duration = 0.3f;
        float startAlpha = fader.color.a;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            fader.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fader.color = new Color(0, 0, 0, targetAlpha);
    }
}





















