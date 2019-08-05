using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour {
    
    public PlatformEffector2D groundCollider;
    
    public void SetEffectorPlatform(bool open)
    {
        groundCollider.gameObject.SetActive(open);     //groundCollider.rotationalOffset = 0,180 會有擠出人物的感覺  不適合
    }
}
