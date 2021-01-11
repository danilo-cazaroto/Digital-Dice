using System;
using UnityEngine;

public class AmbienceController : MonoBehaviour
{
    public Color upperDayColor, downerDayColor, middleColor, downerNightColor, upperNightColor;
    public AudioClip dayAmbienceClip;
    public AudioClip nightAmbienceClip;

    Color lightColor;
    AudioSource audioSource;
    int now;
    float offset;

    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        now = DateTime.Now.Hour;
        //Debug.Log("Now is " +now);

        SetSkyOffSet();
        AmbienceSound();
    }

    /// <summary>
    /// Method to set offset of the sky material and lighting renderer color based on the system hour.
    /// </summary>
    void SetSkyOffSet()
    {
        switch (now)
        {
            case 0:
            case 1:
            case 2:
            case 3: offset = .5f; lightColor = upperNightColor; break;
            case 4: offset = .4f; lightColor = downerNightColor; break;
            case 5: offset = .3f; lightColor = middleColor; break;
            case 6: offset = .2f; lightColor = downerDayColor; break;
            case 7: offset = .1f; lightColor = upperDayColor; break;
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
            case 15: offset = 0f; lightColor = upperDayColor; break;
            case 16: offset = .1f; lightColor = downerDayColor; break;
            case 17: offset = .2f; lightColor = middleColor; break;
            case 18: offset = .3f; lightColor = downerNightColor; break;
            case 19: offset = .4f; lightColor = upperNightColor; break;
            case 20:
            case 21:
            case 22:
            case 23: offset = .5f; lightColor = upperNightColor; ; break;
            default: offset = 0f; lightColor = upperDayColor; break;
        }

        GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
        RenderSettings.ambientSkyColor = lightColor;
    }

    void AmbienceSound()
    {
        switch (now)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
            case 5: audioSource.clip = nightAmbienceClip; break;
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
            case 15:
            case 16:
            case 17: audioSource.clip = dayAmbienceClip; break;
            case 18:
            case 19:
            case 20:
            case 21:
            case 22:
            case 23: audioSource.clip = nightAmbienceClip; break;
            default: audioSource.clip = dayAmbienceClip; break;
        }

        audioSource.Play();
    }
}
