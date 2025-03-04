using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TeleporterScript : MonoBehaviour
{
    public Transform targetPosition; 
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
            //Fade back in

        }
    }
    private System.Collections.IEnumerator TeleportPlayer(Transform player)
    {
        isTeleporting = true;  //Prevents loop

        // Fade to black
        yield return StartCoroutine(FadeScreen(1));
        //Move
        player.position = targetPosition.position;
        // Fade back in
        yield return StartCoroutine(FadeScreen(0));

        yield return new WaitForSeconds(teleportCooldown);  //delay

        isTeleporting = false;  //Allow teleporting again
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

