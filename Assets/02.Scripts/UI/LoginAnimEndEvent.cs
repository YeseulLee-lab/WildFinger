using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginAnimEndEvent : MonoBehaviour
{
    public void DisableAnimator()
    {
        GetComponent<Animator>().enabled = false;
    }
}
