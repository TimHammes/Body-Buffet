using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestroy : MonoBehaviour
{
  // Start is called before the first frame update
  public static DoNotDestroy bCanvas;
  private void Awake()
  {
    if (bCanvas && bCanvas != this)
      
      Destroy(gameObject);
    else
      bCanvas = this;

    DontDestroyOnLoad(bCanvas);
  }
 

   
}
