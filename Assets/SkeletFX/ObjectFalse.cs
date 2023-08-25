using UnityEngine;

namespace SkeletFX
{
  public class ObjectFalse : MonoBehaviour
  {
    public void GamObjFalse()
    {
      if (gameObject.activeInHierarchy)
      {
        gameObject.SetActive(false);
      }
    }
  }
}
