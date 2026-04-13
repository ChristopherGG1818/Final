using UnityEngine;

public class MusicLayers : MonoBehaviour
{
    public AudioSource calm;
    public AudioSource tension;
    public AudioSource wind;
    public AudioSource danger;

    void Start()
    {
        calm.Play();
        tension.Play();
        wind.Play();
        danger.Play();

        SetAllVolumes(0f);
        calm.volume = 1f;
    }

    public void SetCalm()
    {
        FadeTo(calm);
    }

    public void SetTension()
    {
        FadeTo(tension);
    }

    public void SetDanger()
    {
        FadeTo(danger);
    }

    void FadeTo(AudioSource target)
    {
        AudioSource[] all = { calm, tension, wind, danger };

        foreach (var a in all)
        {
            StartCoroutine(Fade(a, a == target ? 1f : 0f));
        }
    }

    System.Collections.IEnumerator Fade(AudioSource audio, float targetVolume)
    {
        float speed = 1f;

        while (Mathf.Abs(audio.volume - targetVolume) > 0.01f)
        {
            audio.volume = Mathf.Lerp(audio.volume, targetVolume, Time.deltaTime * speed);
            yield return null;
        }

        audio.volume = targetVolume;
    }

    void SetAllVolumes(float v)
    {
        calm.volume = v;
        tension.volume = v;
        wind.volume = v;
        danger.volume = v;
    }
}