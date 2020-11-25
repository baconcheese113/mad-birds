using System.Collections;
using UnityEngine;

[System.Serializable]
public class EnemySerialized : SerializeBase { }

public class Enemy : ITypeWithSerialize<EnemySerialized>
{
  [SerializeField] private GameObject _cloudParticlePrefab;
  [SerializeField] private AudioSource _enemyDeath;
  [Tooltip("How slow to slow")]
  [SerializeField] private float _slowmoSpeed = .2f;
  [Tooltip("How long to slow")]
  [SerializeField] private float _slowmoLength = .3f;

  private void OnCollisionEnter2D(Collision2D other)
  {
    Bug bug = other.collider.GetComponent<Bug>();
    if (bug)
    {
      Destroy(GetComponent<Collider2D>());
      Instantiate(_cloudParticlePrefab, transform.position, Quaternion.identity);
      IEnumerator EndSlowMo()
      {
        yield return new WaitForSecondsRealtime(_slowmoLength);
        if (Time.timeScale > 0.001f) Time.timeScale = 1f; // Just in case we're paused
        Destroy(gameObject);
      }
      Time.timeScale = _slowmoSpeed;
      StartCoroutine(EndSlowMo());
      return;
    }
    Enemy enemy = other.collider.GetComponent<Enemy>();
    if (enemy != null) return;

    if (other.contacts[0].normal.y < -.5f)
    {
      Instantiate(_cloudParticlePrefab, transform.position, Quaternion.identity);
      Destroy(gameObject);
    }
  }

  private void OnDestroy()
  {
    LevelController.PlayOnNewObject(transform.position, _enemyDeath);
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
