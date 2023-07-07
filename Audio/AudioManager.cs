/*
This script allows for the creation of AudioItems, each containing necessary information for an audio clip, and uses a 
singleton pattern for easy access. The script provides methods to play, stop, fade in, and fade out audio clips and 
manages the creation and removal of AudioSource components and their associated game objects. Additionally, it offers 
options to attach audio clips to specific transforms and custom rolloff curves for greater control over audio playback.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Serializable]
    public struct AudioItem
    {
        public string Name;
        public AudioClip Clip;
        [Range(0f, 1f)] public float Volume;
        public bool Loop;
        [Range(0f, 1f)] public float SpatialBlend;
        public float MinDistance;
        public float MaxDistance;
        [Range(0f, 10f)] public float RolloffFactor;
        public AudioMixerGroup AudioMixerGroup;
        [Range(0f, 5f)] public float Delay;

        public AudioItem(string name, AudioClip clip, AudioMixerGroup audioMixerGroup, float volume = 1f, bool loop = false, float spatialBlend = 1f,
            float minDistance = 1f, float maxDistance = 500f, float rolloffFactor = 1f)
        {
            Name = name;
            Clip = clip;
            AudioMixerGroup = audioMixerGroup;
            Volume = volume;
            Loop = loop;
            SpatialBlend = spatialBlend;
            MinDistance = minDistance;
            MaxDistance = maxDistance;
            RolloffFactor = rolloffFactor;
            Delay = 0f;
        }
    }

    public List<AudioItem> AudioItems = new List<AudioItem>();
    private readonly Dictionary<string, AudioSource> audioSources = new Dictionary<string, AudioSource>();

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        foreach (var item in AudioItems)
        {
            var obj = new GameObject(item.Name, typeof(AudioSource));
            obj.transform.SetParent(transform);
            var source = obj.GetComponent<AudioSource>();

            try
            {
                source.clip = item.Clip;
                source.outputAudioMixerGroup = item.AudioMixerGroup;
                source.volume = item.Volume;
                source.loop = item.Loop;
                source.spatialBlend = item.SpatialBlend;
                source.minDistance = item.MinDistance;
                source.maxDistance = item.MaxDistance;
                source.playOnAwake = false;
                audioSources.Add(item.Name, source);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to add AudioSource to game object '{item.Name}': {ex.Message}");
            }
        }
    }
    

    
    public void Play(string name, Transform parent)
    {
        if (AudioItems.Exists(item => item.Name == name))
        {
            AudioItem item = AudioItems.Find(item => item.Name == name);
            StartCoroutine(PlayWithDelay(item, parent));
        }
    }

    private IEnumerator PlayWithDelay(AudioItem item, Transform parent)
    {
        yield return new WaitForSeconds(item.Delay);

        GameObject obj = new GameObject(item.Name + " (Temp)", typeof(AudioSource));
        obj.transform.SetParent(parent);
        obj.transform.localPosition = Vector3.zero;

        var source = obj.GetComponent<AudioSource>();

        source.clip = item.Clip;
        source.outputAudioMixerGroup = item.AudioMixerGroup;
        source.volume = item.Volume;
        source.loop = item.Loop;
        source.spatialBlend = item.SpatialBlend;
        source.minDistance = item.MinDistance;
        source.maxDistance = item.MaxDistance;
        source.playOnAwake = false;
        source.rolloffMode = AudioRolloffMode.Custom;

        AnimationCurve customCurve = new AnimationCurve();
        customCurve.AddKey(0f, 1f);
        customCurve.AddKey(item.RolloffFactor, 0f);
        source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, customCurve);

        // Play the AudioSource.
        source.Play();
    }

    public void PlayOneShot(string name)
    {
        if (audioSources.TryGetValue(name, out var source))
        {
            AudioItem item = AudioItems.Find(item => item.Name == name);
            StartCoroutine(PlayOneShotWithDelay(source, item.Delay));
        }
    }
    public void PlayRandomOneShot(params string[] names)
    {
        if (names.Length == 0) return;

        string randomName = names[Random.Range(0, names.Length)];

        if (audioSources.TryGetValue(randomName, out var source))
        {
            AudioItem item = AudioItems.Find(item => item.Name == randomName);
            StartCoroutine(PlayOneShotWithDelay(source, item.Delay));
        }
    }

    private IEnumerator PlayOneShotWithDelay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        source?.PlayOneShot(source.clip, source.volume);
    }

    public void PlayRandomLooped(params string[] names)
    {
        if (names.Length == 0) return;

        StartCoroutine(PlayRandomLoopedCoroutine(names));
    }

    private IEnumerator PlayRandomLoopedCoroutine(string[] names)
    {
        while (true)
        {
            string randomName = names[Random.Range(0, names.Length)];

            if (audioSources.TryGetValue(randomName, out var source))
            {
                AudioItem item = AudioItems.Find(item => item.Name == randomName);
                StartCoroutine(PlayOneShotWithDelay(source, item.Delay));
            }

            yield return new WaitForSeconds(5f);
        }
    }

public void Stop(string name)
{
    if (audioSources.TryGetValue(name, out var source))
    {
        source?.Stop();
    }
}

public void StopAndRemove(string name)
{
    if (audioSources.TryGetValue(name, out var source))
    {
        source?.Stop();
        audioSources.Remove(name);
        Destroy(source.gameObject);
    }
}

public void FadeIn(string name, float duration)
{
    if (audioSources.TryGetValue(name, out var source))
    {
        StartCoroutine(FadeAudio(source, true, duration));
    }
}

public void FadeOut(string name, float duration)
{
    if (audioSources.TryGetValue(name, out var source))
    {
        StartCoroutine(FadeAudio(source, false, duration));
    }
}

private IEnumerator FadeAudio(AudioSource source, bool fadeIn, float duration)
{
    var startVolume = fadeIn ? 0f : source.volume;
    var targetVolume = fadeIn ? source.volume : 0f;

    source.volume = startVolume;
    source.Play();

    var currentTime = 0f;

    while (currentTime < duration)
    {
        currentTime += Time.deltaTime;
        source.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);

        yield return null;
    }

    if (!fadeIn)
    {
        source.Stop();
    }

    if (source.volume <= 0f)
    {
        source.volume = startVolume;
    }
}
}
