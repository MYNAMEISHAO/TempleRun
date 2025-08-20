using UnityEngine;

public class CoinController : MonoBehaviour
{
    public int scoreValue = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.instance.PlayCoin();
            ScoreManager.instance.AddPoints(scoreValue);
            Destroy(gameObject);
        }
    }
}