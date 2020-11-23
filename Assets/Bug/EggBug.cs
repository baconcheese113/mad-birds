using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggBug : Bug
{
  [SerializeField] private byte _numEggs = 3;
  [SerializeField] private GameObject _eggBomb;

  protected override void Update()
  {
    base.Update();
    if (Input.GetKeyDown(KeyCode.Mouse0) && _birdWasLaunched && _numEggs > 0)
    {
      Instantiate(_eggBomb, transform.position - new Vector3(0f, 2f, 0f), transform.rotation);
      _numEggs--;
    }
  }
}
