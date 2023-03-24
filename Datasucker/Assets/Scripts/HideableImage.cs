using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideableImage : MonoBehaviour
{
    public bool Hidden
    {
        get { return !GetComponent<Image>().enabled; }
        set { GetComponent<Image>().enabled = !value; }
    }
}
