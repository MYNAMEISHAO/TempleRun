using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    [Header("Sound Button Settings")]
    [SerializeField] private Image soundButtonImage;
    [SerializeField] private Color soundOffColor = Color.gray;

    private Color soundButtonOriginalColor;

    void Awake()
    {
        if (soundButtonImage != null)
        {
            soundButtonOriginalColor = soundButtonImage.color;
        }
    }

    void OnEnable()
    {

        UpdateSoundButtonVisual();
    }

    public void Sound()
    {
        Debug.Log("--- HÀM Sound() TRONG PauseController ĐÃ ĐƯỢC GỌI! ---");

        if (SoundManager.instance == null)
        {
            //Debug.LogError("LỖI NGHIÊM TRỌNG: SoundManager.instance không tồn tại (bị null)!");
            return; 
        }
        // ------------------------------------

        //Debug.Log("SoundManager.instance đã tồn tại. Đang gọi hàm ToggleSound()...");
        SoundManager.instance.ToggleSound();
        UpdateSoundButtonVisual();
    }

    private void UpdateSoundButtonVisual()
    {
        if (soundButtonImage != null)
        {
            soundButtonImage.color = SoundManager.instance.IsMuted ? soundOffColor : soundButtonOriginalColor;
        }
    }

    public void Resume()
    {
        GameManager.instance.UpdateGameState(GameState.Playing);
    }

    public void BackHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}