using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource SFXSource;
    public AudioSource WeaponSFXSource;

    public AudioClip background;

    private void Start()
    {
        //musicSource.clip = background;
        //musicSource.Play();
    }

    public void playMusic(AudioClip song)
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
        musicSource.clip = song;
        musicSource.Play();
    }

    public void PlayWeaponSFX(AudioClip clip)
    {
        if (WeaponSFXSource.isPlaying)
        {
            WeaponSFXSource.Stop();
        }
        WeaponSFXSource.PlayOneShot(clip);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (SFXSource.isPlaying)
        {
            SFXSource.Stop();
        }
        SFXSource.PlayOneShot(clip);
    }
}
