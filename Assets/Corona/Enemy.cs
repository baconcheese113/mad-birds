using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
  private void OnCollisionEnter2D(Collision2D other)
  {
    Bug bug = other.collider.GetComponent<Bug>();
    if (bug != null)
    {
      Destroy(gameObject);
      return;
    }
    Enemy enemy = other.collider.GetComponent<Enemy>();
    if (enemy != null) return;

    if (other.contacts[0].normal.y < -.5)
    {
      Destroy(gameObject);
    }
  }
}
