using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatFinishLineController : MonoBehaviour
{
    //note, x coordinate, center of object is always -6


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setWidth(float width){
        transform.localScale=new Vector3(width,transform.localScale.y,transform.localScale.z);
    }
}
