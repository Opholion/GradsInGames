using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundtrackManager : MonoBehaviour
{
    SoundtrackTypes isMainMenu = SoundtrackTypes.MainMenu;
    [SerializeField] AudioClip[] MainMenuSoundtracks;
    [SerializeField] AudioClip[] GameplaySoundtracks;
    [SerializeField] AudioClip[] CombatSoundtracks;
    [SerializeField] float soundtrackVolume = 1.0f;
    float actualSoundtrackVolume = 0.0f;
    private AudioSource activeAudio;

    [SerializeField] int MaxTimeForCombatSound = 5;
    int TurnsSinceCombat;
    float TimeSinceSoundStart = 0;


    public enum SoundtrackTypes
    {
        MainMenu, Gameplay, Combat
    }

    #region SoundtrackManager_Singleton

    public static SoundtrackManager instance;

    private void Awake()
    {
        TurnsSinceCombat = MaxTimeForCombatSound;
        instance = this;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        activeAudio = gameObject.AddComponent<AudioSource>();
    }
    public void UpdateTurns()
    {
        ++TurnsSinceCombat;
    }

    public void SetState(SoundtrackTypes State)
    {
        if (State != isMainMenu)
        {
            isMainMenu = State;
            actualSoundtrackVolume = 0.0f;
            TimeSinceSoundStart = 0;
            activeAudio.Stop();
        }
    }
    // Update is called once per frame
    void LateUpdate()
    {

            TimeSinceSoundStart += Time.deltaTime;


        if (TimeSinceSoundStart <= 1)
        {
            actualSoundtrackVolume = Mathf.Lerp(actualSoundtrackVolume, soundtrackVolume, TimeSinceSoundStart);
        }

        if (!activeAudio)
        {
            activeAudio = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            if (actualSoundtrackVolume > 0)
            {

                if (TimeSinceSoundStart >= MaxTimeForCombatSound && isMainMenu == SoundtrackTypes.Combat)
                {
                    TimeSinceSoundStart = 0;
                    isMainMenu = SoundtrackTypes.Gameplay;
                }

                switch (isMainMenu)
                {
                    case SoundtrackTypes.MainMenu:
                        if (MainMenuSoundtracks.Length > 0 && !activeAudio.isPlaying)
                            activeAudio.PlayOneShot(MainMenuSoundtracks[Random.Range(0, MainMenuSoundtracks.Length)], actualSoundtrackVolume);
                        break;
                    case SoundtrackTypes.Gameplay:
                        if (MainMenuSoundtracks.Length > 0 && !activeAudio.isPlaying)
                            activeAudio.PlayOneShot(GameplaySoundtracks[Random.Range(0, GameplaySoundtracks.Length)], actualSoundtrackVolume * 0.75f);

                        break;
                    case SoundtrackTypes.Combat:
                        if (MainMenuSoundtracks.Length > 0 && !activeAudio.isPlaying)
                            activeAudio.PlayOneShot(CombatSoundtracks[Random.Range(0, CombatSoundtracks.Length)], actualSoundtrackVolume * 0.85f);
                        TurnsSinceCombat = 0;
                        break;
                }
            }

        }
    }
}
