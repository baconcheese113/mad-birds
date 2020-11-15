using UnityEngine;

[System.Serializable]
public class EnemySerialized : SerializeBase { }

public class Enemy : TypeWithSerialize<EnemySerialized>
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

  public override void Deserialize(EnemySerialized data)
  {
    base.Deserialize(data);
    transform.position = new Vector3(data.position.x, data.position.y, 0);
    transform.rotation = Quaternion.Euler(0, 0, data.rotation);
  }

  public override EnemySerialized Serialize()
  {
    EnemySerialized enemy = new EnemySerialized();
    enemy.position = new Vector2(transform.position.x, transform.position.y);
    enemy.rotation = transform.eulerAngles.z;
    return enemy;
  }
}
