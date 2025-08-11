using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinController : MonoBehaviour
{
    public int scoreValue = 10; // Số điểm nhận được khi nhặt
    public AudioClip collectSound; // Âm thanh phát ra khi nhặt

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Gọi đến ScoreManager để cộng điểm
            ScoreManager.instance.AddPoints(scoreValue);

            // Phát âm thanh tại vị trí của đồng xu
            // Âm thanh sẽ không bị mất đi khi đồng xu biến mất
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }

            // Hủy đồng xu
            Destroy(gameObject);
        }
    }
}