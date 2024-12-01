using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralSoundtrack : MonoBehaviour
{
    public AudioSource calmLayer;      // Calm music source
    public AudioSource intenseLayer;   // Intense music source
    public AudioSource tickingLayer;   // Clock ticking for intensity
    public AudioSource effectsLayer;   // Sound effects (deaths, upgrades)

    public AudioClip[] calmClips;      // 3 calm clips
    public AudioClip[] intenseClips;   // 3 intense clips
    public AudioClip[] enemyDeathClips; // 3 enemy death clips
    public AudioClip[] defenderDeathClips; // 3 defender death clips
    public AudioClip[] upgradeClips; // 3 upgrade clips

    public Hourglass hourglass;        // Reference to Hourglass script

    private float maxHealth;
    private float gameStartTime;       // Time when the game started

    private void Start()
    {
        
        gameStartTime = Time.time;

        // Start with calm music
        PlayRandomClip(calmLayer, calmClips);
        intenseLayer.Stop();
        

        //Ticking sound for intensity
        if (tickingLayer != null) tickingLayer.Play();
    }

    private void Update()
    {
        AdjustMusicLayers();
        IntensifyMusicOverTime();
    }

    private void AdjustMusicLayers()
    {
        float healthPercentage = hourglass.hourglassHealth / hourglass.maxHourglassHealth;

        // Calm music fades out between 100% and 50% health
        float calmVolume = Mathf.Clamp01((healthPercentage - 0.5f) * 2f); // Full volume above 50%, fades out below

        // Intense music fades in between 60% and 0% health
        float intenseVolume = Mathf.Clamp01((0.6f - healthPercentage) / 0.6f); // Starts at 60%, max at 0%

        calmLayer.volume = calmVolume;

        // Only play intense layer if its volume is greater than 0
        if (intenseVolume > 0)
        {
            intenseLayer.volume = intenseVolume;
            if (!intenseLayer.isPlaying) PlayRandomClip(intenseLayer, intenseClips);
        }
        else
        {
            intenseLayer.volume = 0;
            if (intenseLayer.isPlaying) intenseLayer.Stop();
        }

        // Ensure calm layer is playing
        if (!calmLayer.isPlaying) PlayRandomClip(calmLayer, calmClips);
    }



    void PlayRandomClip(AudioSource layer, AudioClip[] clips)
    {
        if (clips.Length == 0) return;

        int index = Random.Range(0, clips.Length);
        layer.clip = clips[index];

        // Apply random pitch and volume for variation
        layer.pitch = Random.Range(0.9f, 1.1f);
        layer.volume = Random.Range(0.8f, 1f);

        layer.Play();
    }

    public void PlayEnemyDeathEffect()
    {
        PlayRandomEffect(enemyDeathClips);
    }

    public void PlayDefenderDeathEffect()
    {
        PlayRandomEffect(defenderDeathClips);
    }
    public void PlayUpgradeEffect()
    {
        PlayRandomEffect(upgradeClips);
    }

    void PlayRandomEffect(AudioClip[] clips)
    {
        if (clips.Length == 0) return;

        int index = Random.Range(0, clips.Length);
        effectsLayer.pitch = Random.Range(0.9f, 1.1f);
        effectsLayer.volume = Random.Range(0.1f, 0.6f);
        effectsLayer.PlayOneShot(clips[index]);
    }

    private void IntensifyMusicOverTime()
    {
        float elapsedTime = Time.time - gameStartTime;

        // Gradually increase ticking volume or tempo
        if (tickingLayer != null)
        {
            tickingLayer.pitch = Mathf.Lerp(1f, 2f, elapsedTime / 600f); // Double speed over 10 minutes
            tickingLayer.volume = Mathf.Lerp(0.5f, 1f, elapsedTime / 600f); // Increase volume over 10 minutes
        }

        // Link pitch to health percentage for intensity
        float healthPercentage = hourglass.hourglassHealth / maxHealth;
        calmLayer.pitch = Mathf.Lerp(1f, 1.2f, 1f - healthPercentage); // Slight pitch increase as health decreases
        intenseLayer.pitch = Mathf.Lerp(1f, 1.5f, 1f - healthPercentage); // More aggressive pitch increase
    }
}