using UnityEngine;
using UnityEngine.UI; // Bắt buộc phải có để dùng Slider

public class SoundManager : MonoBehaviour
{
    // Singleton Instance
    public static SoundManager instance;

    [Header("UI References")]
    public Slider volumeSlider; // Kéo Slider UI vào đây

    [Header("Music Tracks")]
    [SerializeField] private AudioClip introMusic;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip gameOverMusic;

    [Header("Sound Effects (SFX)")]
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip coinSound;
    [SerializeField] private AudioClip itemSound;
    [SerializeField] private AudioClip playerDieSound;

    // Components
    private AudioSource musicSource;
    private AudioSource sfxSource;

    // Public property để các script khác có thể kiểm tra trạng thái tắt/mở
    public bool IsMuted
    {
        get { return AudioListener.volume <= 0; }
    }

    void Awake()
    {
        // --- Thiết lập Singleton ---
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

        // --- Lấy và cài đặt AudioSource ---
        AudioSource[] sources = GetComponents<AudioSource>();
        if (sources.Length >= 2)
        {
            musicSource = sources[0];
            sfxSource = sources[1];
            // Đảm bảo âm thanh vẫn hoạt động khi game pause (Time.timeScale = 0)
            musicSource.ignoreListenerPause = true;
            sfxSource.ignoreListenerPause = true;
        }
        else
        {
            Debug.LogError("SoundManager cần 2 AudioSource (1 cho Music, 1 cho SFX).");
        }
    }

    void Start()
    {
        // Lấy âm lượng đã lưu, nếu không có thì mặc định là 1 (tối đa)
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);

        // Áp dụng âm lượng đã lưu cho game
        SetVolume(savedVolume);

        // Cập nhật vị trí của thanh trượt cho khớp với âm lượng đã lưu
        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
        }

        PlayIntroMusic();
    }

    // --- CÁC HÀM ĐIỀU KHIỂN CHÍNH ---

    // Hàm này được gọi bởi Slider mỗi khi giá trị thay đổi
    public void SetVolume(float volume)
    {
        // Giới hạn giá trị âm lượng từ 0 đến 1
        volume = Mathf.Clamp01(volume);

        // Áp dụng âm lượng cho toàn bộ game
        AudioListener.volume = volume;

        // Lưu lại giá trị mới
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    // Hàm này được gọi bởi nút Sound (tắt/mở tiếng)
    public void ToggleSound()
    {
        // Nếu đang có tiếng
        if (AudioListener.volume > 0)
        {
            // Tắt tiếng
            SetVolume(0f);
        }
        // Nếu đang bị tắt
        else
        {
            // Bật lại âm lượng đã lưu trước đó, hoặc 100% nếu chưa có
            float lastVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            // Nếu chẳng may âm lượng đã lưu là 0, thì bật lại 100%
            if (lastVolume <= 0)
            {
                lastVolume = 1f;
            }
            SetVolume(lastVolume);
        }

        // Cập nhật lại thanh trượt UI
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
        }
    }

    // --- CÁC HÀM PUBLIC ĐỂ GỌI ÂM THANH TỪ BÊN NGOÀI ---

    public void PlayIntroMusic() => PlayMusic(introMusic);
    public void PlayGameMusic() => PlayMusic(gameMusic);
    public void PlayGameOverMusic() => PlayMusic(gameOverMusic);
    public void StopMusic() => musicSource.Stop();

    public void PlayClick() => PlaySFX(clickSound);
    public void PlayJump() => PlaySFX(jumpSound);
    public void PlayCoin() => PlaySFX(coinSound);
    public void PlayItem() => PlaySFX(itemSound);
    public void PlayPlayerDie() => PlaySFX(playerDieSound);

    // --- Các hàm nội bộ ---

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }
}