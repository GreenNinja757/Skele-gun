using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource SFXSource;

    public AudioClip background;
    public AudioClip energyWeapon;
    public AudioClip kineticWeapon;

    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    public void playShootSound()
    {
        SFXSource.clip = energyWeapon;
        SFXSource.Play();
    }
}
