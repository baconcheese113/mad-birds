using UnityEngine;

public class Enemy : MonoBehaviour
{
  [SerializeField] private GameObject _cloudParticlePrefab;

  private void OnCollisionEnter2D(Collision2D other)
  {
    Bug bug = other.collider.GetComponent<Bug>();
    if (bug != null)
    {
      Instantiate(_cloudParticlePrefab, transform.position, Quaternion.identity);
      Destroy(gameObject);
      return;
    }
    Enemy enemy = other.collider.GetComponent<Enemy>();
    if (enemy != null) return;

    if (other.contacts[0].normal.y < -.5)
    {
      Instantiate(_cloudParticlePrefab, transform.position, Quaternion.identity);
      Destroy(gameObject);
    }
  }

  private void OnDestroy()
  {
    LevelController controller = FindObjectOfType<LevelController>();
    if (controller) controller.NotifyEnemyDestroyed(this);
  }
}
