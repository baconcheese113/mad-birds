using UnityEngine;

[System.Serializable]
public class EnemySerialized : SerializeBase { }

public class Enemy : ITypeWithSerialize<EnemySerialized>
{
  [SerializeField] private GameObject _cloudParticlePrefab;
  [SerializeField] private AudioClip _enemyDeath;

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
    if (_enemyDeath) AudioSource.PlayClipAtPoint(_enemyDeath, transform.position);
    LevelController controller = FindObjectOfType<LevelController>();
    if (controller) controller.NotifyEnemyDestroyed(this);
  }

  public override EnemySerialized Serialize()
  {
    EnemySerialized enemy = new EnemySerialized();
    enemy.x = transform.position.x;
    enemy.y = transform.position.y;
    enemy.rotation = transform.eulerAngles.z;
    return enemy;
  }
}
