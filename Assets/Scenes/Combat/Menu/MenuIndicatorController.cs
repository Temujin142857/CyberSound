using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuIndicatorController : MonoBehaviour
{
    public float orientation=0f;
    public float idleRotationSpeed=1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void activate(bool b){
        gameObject.SetActive(b);
    }

    //ramps up in speed for some reason?
    public void idleRotate(){
        orientation-=idleRotationSpeed;
        transform.Rotate(0f,0f,orientation,Space.Self);           
    }

    public void faceUp(){
        float rotationNeeded=0-orientation;
        transform.Rotate(0f,0f,rotationNeeded,Space.Self);   
        orientation=0;     
    }

    public void faceRight(){
        float rotationNeeded=270-orientation;
        transform.Rotate(0f,0f,rotationNeeded,Space.Self);
        orientation=270;
    }

    public void faceDown(){
        float rotationNeeded=180-orientation;
        transform.Rotate(0f,0f,rotationNeeded,Space.Self);
        orientation=180;
    }

    public void faceLeft(){
        float rotationNeeded=90-orientation;
        transform.Rotate(0f,0f,rotationNeeded,Space.Self);
        orientation=90;
    }
}
