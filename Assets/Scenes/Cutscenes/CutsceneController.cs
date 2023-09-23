using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    public GameObject slide1;
    public GameObject slide2;
    public GameObject slide3;
    public GameObject slide4;
    public GameObject slide5;
    private List<GameObject> slides=new List<GameObject>();
    private int spriteIndex=0;

    // Start is called before the first frame update
    void Start()
    {        
        slides.Add(slide1);
        slides.Add(slide2);
        slides.Add(slide3);
        slides.Add(slide4);
        slides.Add(slide5);
    }

    // Update is called once per frame
    void Update()
    {

     if(Input.GetButtonDown("interact")){
        if(spriteIndex>=slides.Count){Application.Quit();}
        slides[spriteIndex].SetActive(false);
        spriteIndex++;        
        
     }   
    }
}
