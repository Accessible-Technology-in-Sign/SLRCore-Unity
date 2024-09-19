using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSManager : MonoBehaviour {
    
    [SerializeField] private int targetFPS = 60;
    //default FPS Setting

    void Start() {
        Application.targetFrameRate = targetFPS;
    }
}
