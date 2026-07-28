using UnityEngine;

// Plays one-shot sounds without needing AudioSource components wired up in each scene.
public static class SfxPlayer
{
    private static AudioClip hitClip;

    private static AudioClip HitClip
    {
        get
        {
            if (hitClip == null) hitClip = Resources.Load<AudioClip>("Audio/golfHit");
            return hitClip;
        }
    }

    public static void PlayHit(Vector3 position, float volume, float pitch = 1f)
    {
        AudioClip clip = HitClip;
        if (clip == null) return;

        GameObject go = new GameObject("Sfx_golfHit");
        go.transform.position = position;

        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = Mathf.Clamp01(volume);
        source.pitch = pitch;
        source.spatialBlend = 0f; // plain 2D sound, same volume everywhere
        source.Play();

        Object.Destroy(go, clip.length / Mathf.Max(pitch, 0.01f) + 0.1f);
    }
}
