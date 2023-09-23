using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuOptionController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
           
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void highlight(){
        //switch image between highlighted and unhighlighted verision?
    }

    public void unHighlight(){

    }

    public void activate(bool b){
        gameObject.SetActive(b);
    }

    public void setUpSprite(string songName){
        //displays song name      
        Sprite sprite= Resources.Load<Sprite>("Visual_Assests/Combat/Song_Names/"+songName);
        this.GetComponent<SpriteRenderer>().sprite=sprite;
    }
    
}
