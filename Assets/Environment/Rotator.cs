using UnityEngine;

public class Rotator : MonoBehaviour
{
  [SerializeField] private float _degsPerSec = 45f;

  // Update is called once per frame
  void Update()
  {
    transform.Rotate(new Vector3(0, 0, _degsPerSec * Time.deltaTime));
  }
}
