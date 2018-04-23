using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : BaseObject
{
    public int PlayingSoundCount = 3;
    public bool Mute = false;
    [Range(0, 1)]
    public float Volume = 0.5f;

    [Header("Word Find Sounds")]
    public List<AudioClip> WordFindAudioClips;

    [Header("Other Clips")]
    public List<AudioClip> OtherClips;


    private int _currentWordIndex = 0;
    private float _lastVolume = 0.5f;
    private List<AudioSource> _audioSources = new List<AudioSource>();
    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < PlayingSoundCount; i++)
        {
            AudioSource aS = gameObject.AddComponent<AudioSource>();
            aS.volume = Volume;
            aS.mute = Mute;
            _audioSources.Add(aS);
        }
    }

    public void PlayWordCompeletClip()
    {
        PlayAudioClip(WordFindAudioClips[_currentWordIndex]);
        _currentWordIndex++;
        _currentWordIndex = Mathf.Min(_currentWordIndex, WordFindAudioClips.Count);
    }

    public void PlayAudioClip(AudioClip clip)
    {
        foreach (AudioSource source in _audioSources)
        {
            if (!source.isPlaying)
            {
                source.Stop();
                source.clip = clip;
                source.Play();
                return;
            }
        }
        AudioSource rndAudioSource = _audioSources[UnityEngine.Random.Range(0, _audioSources.Count)];
        rndAudioSource.Stop();
        rndAudioSource.clip = clip;
        rndAudioSource.Play();
    }

    public void PlayOtherClip(int index)
    {
        if (index >= OtherClips.Count)
            return;

        PlayAudioClip(OtherClips[index]);
    }

    // UpdateData is called once per frame
    void Update()
    {
        foreach (AudioSource audioSource in _audioSources)
            audioSource.mute = Mute;


        if (Math.Abs(_lastVolume - Volume) > .01)
            foreach (AudioSource audioSource in _audioSources)
            {
                audioSource.volume = Volume;
                _lastVolume = Volume;
            }


    }

    public void ResetSounds()
    {
        _currentWordIndex = 0;
    }
}
