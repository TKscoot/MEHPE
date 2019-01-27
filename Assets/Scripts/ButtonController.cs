using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour {

    BeatScaler BS = null;
    [SerializeField] float normalBS = 0.1f;
    [SerializeField] float hoverBS = 0.25f;

    private void Awake()
    {
        BS = GetComponent<BeatScaler>();
    }

    public void EnterButton()
    {
        BS.scaleFactor = hoverBS;
    }

    public void LeftButton()
    {
        BS.scaleFactor = normalBS;
    }
}
