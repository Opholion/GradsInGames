using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizedSoundComponent : MonoBehaviour
{
    [SerializeField] AudioClip[] movementClip;
    [SerializeField] AudioClip[] onDamageClip;
    [SerializeField] float movementClipVolume = 1.0f;
    [SerializeField] float PitchRandomizer = 0.25f;
    private List<AudioSource> activeAudioSources;


    GameObject tempObject;
    public void PlaySound(int index)
    {
        tempObject = new GameObject("SoundEffect_" + activeAudioSources.Count);
        // Instantiate(tempObject, transform.position, transform.rotation);

        activeAudioSources.Add(tempObject.AddComponent<AudioSource>());

        if (index <= 0)
        {
            if (movementClip.Length > 0)
            {
                activeAudioSources[activeAudioSources.Count - 1].PlayOneShot(movementClip[Random.Range(0, movementClip.Length)], movementClipVolume);
                activeAudioSources[activeAudioSources.Count - 1].pitch = UnityEngine.Random.Range(1 - PitchRandomizer, 1 + PitchRandomizer);
            }
        }
        else if (index <= 1)
            if (onDamageClip.Length > 0)
            {
                activeAudioSources[activeAudioSources.Count - 1].PlayOneShot(onDamageClip[Random.Range(0, onDamageClip.Length)], 0.875f * 0.33f);
                activeAudioSources[activeAudioSources.Count - 1].pitch = UnityEngine.Random.Range(1 - PitchRandomizer, 1 + PitchRandomizer);
            }
    }
    // Start is called before the first frame update
    private void Awake()
    {
        activeAudioSources = new List<AudioSource>();
    }
    private void FixedUpdate()
    {
        if (activeAudioSources.Count > 0)
        {
            for (int i = 0; i < activeAudioSources.Count; ++i)
            {
                if (!activeAudioSources[i] || !activeAudioSources[i].isPlaying)
                {
                    Destroy(activeAudioSources[i].gameObject);
                    activeAudioSources.RemoveAt(i);
                }
            }
        }
    }

    ~RandomizedSoundComponent()
    {
        for (int i = 0; i < activeAudioSources.Count; ++i)
        {
            Destroy(activeAudioSources[i].gameObject);
            activeAudioSources.RemoveAt(i);
        }
    }
}
