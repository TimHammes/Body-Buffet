using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCanvas : MonoBehaviour
{
  public static ButtonCanvas bCanvas;
  // Start is called before the first frame update

  private void Awake()
  {
    DontDestroyOnLoad(bCanvas);
  }
  void Start()
  {
    if (bCanvas && bCanvas != this)
      Destroy(this);
    else
      bCanvas = this;
  }
}
