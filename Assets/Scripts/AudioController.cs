using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioMixerSnapshot idleMixerSnapshot;
    [SerializeField] private AudioMixerSnapshot playMixerSnapshot;
    [SerializeField] private AudioMixerSnapshot anticipationVolumeUp;
    [SerializeField] private AudioMixerSnapshot anticipationVolumeDown;

    [SerializeField] private AudioSource[] audioSources;
    [SerializeField] private GameController gameController;
    [SerializeField] private BonusGameController bonusGameController;
    [SerializeField] private ReelsScroll reelsScroll;
    [SerializeField] private AnimationsManagement animationsManagement;
    [SerializeField] private WinLinesCheck winLinesCheck;
    [SerializeField] private CounterAnimator counterAnimator;
    [SerializeField] private Button playButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private Button startBonusGameButton;
    [SerializeField] private Button stopFreeSpinButton;
    [SerializeField] private Button resetFSAnimationsButton;

    private Dictionary<SoundType, AudioSource> audioDictionary = new Dictionary<SoundType, AudioSource>();

    private void Awake()
    {
        InitializeAudioDictionary();
        SubscribeEvents();
    }

    private void UnShadowMusic()
    {
        idleMixerSnapshot.TransitionTo(0.3f);
    }

    private void PlayReelSpinSound()
    {
        playMixerSnapshot.TransitionTo(0.3f);
        audioDictionary[SoundType.ReelScrolling].Stop();
        audioDictionary[SoundType.ReelScrolling].Play();        
    }

    private void StopPrizeSound()
    {
        audioDictionary[SoundType.PrizeChanging].Stop();
    }

    private void PlayPrizeSound()
    {
        audioDictionary[SoundType.PrizeChanging].Play();
    }

    private void StopAnticipationSound()
    {
        anticipationVolumeDown.TransitionTo(0);
        audioDictionary[SoundType.Anticipation].Stop();
    }

    private void PlayAnticipationSound()
    {
        anticipationVolumeUp.TransitionTo(3f);
        audioDictionary[SoundType.ReelScrolling].Stop();
        audioDictionary[SoundType.Anticipation].Play();
    }

    private void PlayBonusGameMusic()
    {
        audioDictionary[SoundType.Backgroung].Stop();
        audioDictionary[SoundType.BonusBackground].Play();
    }

    private void PlayOrdinaryMusic()
    {
        audioDictionary[SoundType.BonusBackground].Stop();
        audioDictionary[SoundType.Backgroung].Play();
    }

    private void PlayWinLineSound()
    {
        audioDictionary[SoundType.WinLine].Play();
    }

    private void PlayStopReelSound(SubReel obj)
    {
        if (obj.ReelID == 3) audioDictionary[SoundType.ReelScrolling].Stop();
        if (winLinesCheck.CheckScattersOnReel(obj) > 0)
        {
            if (obj.ReelID == 1) audioDictionary[SoundType.ScatterFirst].Play();
            if (obj.ReelID == 2) audioDictionary[SoundType.ScatterSecond].Play();
            if (obj.ReelID == 3) audioDictionary[SoundType.ScatterThird].Play();
        }
        else audioDictionary[SoundType.ReelStop].Play();
    }


    private void PlayClickSound()
    {
        audioDictionary[SoundType.ButtonClick].Play();
    }

    private void InitializeAudioDictionary()
    {
        audioDictionary.Add(SoundType.Backgroung, audioSources[0]);
        audioDictionary.Add(SoundType.BonusBackground, audioSources[1]);
        audioDictionary.Add(SoundType.ButtonClick, audioSources[2]);
        audioDictionary.Add(SoundType.ReelScrolling, audioSources[3]);
        audioDictionary.Add(SoundType.ReelStop, audioSources[4]);
        audioDictionary.Add(SoundType.ScatterFirst, audioSources[5]);
        audioDictionary.Add(SoundType.ScatterSecond, audioSources[6]);
        audioDictionary.Add(SoundType.ScatterThird, audioSources[7]);
        audioDictionary.Add(SoundType.WinLine, audioSources[8]);
        audioDictionary.Add(SoundType.Anticipation, audioSources[9]);
        audioDictionary.Add(SoundType.PrizeChanging, audioSources[10]);        
    }

    private void SubscribeEvents()
    {
        playButton.onClick.AddListener(PlayClickSound);
        stopButton.onClick.AddListener(PlayClickSound);
        startBonusGameButton.onClick.AddListener(PlayClickSound);
        stopFreeSpinButton.onClick.AddListener(PlayClickSound);
        resetFSAnimationsButton.onClick.AddListener(PlayClickSound);

        reelsScroll.SpinStarted += PlayReelSpinSound;
        reelsScroll.ReelStopped += PlayStopReelSound;
        reelsScroll.Anticipation += PlayAnticipationSound;
        reelsScroll.AllReelsStopped += StopAnticipationSound;

        animationsManagement.WinLineAnimationShowing += PlayWinLineSound;
        animationsManagement.AllAnimationsFinished += UnShadowMusic;

        gameController.OrdinaryGameStarted += PlayOrdinaryMusic;
        bonusGameController.BonusGameStarted += PlayBonusGameMusic;
        
        counterAnimator.StartChangingPrize += PlayPrizeSound;
        counterAnimator.FinishChangingPrize += StopPrizeSound;
    }
}
