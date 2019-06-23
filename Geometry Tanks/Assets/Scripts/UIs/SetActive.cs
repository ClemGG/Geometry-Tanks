using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActive : MonoBehaviour
{
    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
