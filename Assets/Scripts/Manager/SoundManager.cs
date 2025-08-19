using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Singleton Instance
    public static SoundManager instance;

    [Header("Nhạc Nền (Music)")]
    [SerializeField] private AudioClip introMusic;
    [SerializeField] private AudioClip gameMusic;

    [Header("Hiệu ứng Âm thanh (SFX)")]
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip coinSound;
    [SerializeField] private AudioClip itemSound;
    [SerializeField] private AudioClip playerDieSound;
    [SerializeField] private AudioClip gameOverMusic;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private bool isMuted = false;

    public bool IsMuted { get { return isMuted; } }


    void Awake()
    {
        // Thiết lập Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Lấy AudioSource
        AudioSource[] sources = GetComponents<AudioSource>();
        if (sources.Length >= 2)
        {
            musicSource = sources[0];
            sfxSource = sources[1];
            musicSource.ignoreListenerPause = true;
            sfxSource.ignoreListenerPause = true;
        }
        else
        {
            Debug.LogError("SoundManager cần 2 AudioSource (Music + SFX).");
        }

    }

    void Start()
    {
        isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;
        ApplyMuteState();

        PlayIntroMusic(); 
    }

    private void ApplyMuteState()
    {
        AudioListener.volume = isMuted ? 0f : 1f;
        //Debug.Log("4. Đã áp dụng trạng thái. AudioListener.volume bây giờ là: " + AudioListener.volume);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayIntroMusic() => PlayMusic(introMusic);
    public void PlayGameMusic() => PlayMusic(gameMusic);

    public void PlayClick() => PlaySFX(clickSound);
    public void PlayJump() => PlaySFX(jumpSound);
    public void PlayCoin() => PlaySFX(coinSound);
    public void PlayItem() => PlaySFX(itemSound);
    public void PlayPlayerDie() => PlaySFX(playerDieSound);
    public void PlayGameOverMusic() => PlayMusic(gameOverMusic);
    public void StopMusic()
    {
        musicSource.Stop();
    }
    public void ToggleSound()
    {
        //Debug.Log("1. ToggleSound được gọi. Trạng thái isMuted hiện tại là: " + isMuted);

        isMuted = !isMuted;
        //Debug.Log("2. Trạng thái isMuted đã được đảo ngược thành: " + isMuted);

        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        //Debug.Log("3. Đã lưu trạng thái mới vào PlayerPrefs.");

        ApplyMuteState();
    }
}
