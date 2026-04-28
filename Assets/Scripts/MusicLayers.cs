using UnityEngine;

public class MusicLayers : MonoBehaviour
{
    public AudioSource calm;
    public AudioSource tension;
    public AudioSource wind;
    public AudioSource danger;

    public float fadeSpeed = 2f;

    private AudioSource current;

    void Start()
    {
        current = calm;

        if (current != null)
        {
            current.volume = 1f;
            current.Play();
        }

        Debug.Log("Now Playing: Calm");
    }

    public void SetCalm()
    {
        SwitchTo(calm, "Calm");
    }

    public void SetTension()
    {
        SwitchTo(tension, "Tension");
    }

    public void SetDanger()
    {
        SwitchTo(danger, "Danger");
    }

    public void SetWind()
    {
        SwitchTo(wind, "Wind");
    }

    void SwitchTo(AudioSource newTrack, string name)
    {
        if (newTrack == current) return;

        newTrack.volume = 0f;
        newTrack.Play();

        if (current != null)
        {
            StartCoroutine(FadeOut(current));
        }

        StartCoroutine(FadeIn(newTrack));

        current = newTrack;

        Debug.Log("Now Playing: " + name);
    }

    public void StopAllMusic()
    {
        StopAllCoroutines();

        if (calm != null) calm.Stop();
        if (tension != null) tension.Stop();
        if (wind != null) wind.Stop();
        if (danger != null) danger.Stop();

        current = null;

        Debug.Log("All music stopped (boss fight start)");
    }

    System.Collections.IEnumerator FadeIn(AudioSource audio)
    {
        if (audio == null) yield break;

        audio.volume = 0f;
        audio.Play();

        while (audio.volume < 0.99f)
        {
            audio.volume = Mathf.Lerp(audio.volume, 1f, Time.deltaTime * fadeSpeed);
            yield return null;
        }

        audio.volume = 1f;
    }

    System.Collections.IEnumerator FadeOut(AudioSource audio)
    {
        if (audio == null) yield break;

        while (audio.volume > 0.01f)
        {
            audio.volume = Mathf.Lerp(audio.volume, 0f, Time.deltaTime * fadeSpeed);
            yield return null;
        }

        audio.volume = 0f;
        audio.Stop();
    }
}