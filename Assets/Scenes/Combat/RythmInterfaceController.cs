using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RythmInterfaceController : MonoBehaviour
{
    public List<GameObject> beats=new List<GameObject>();
    public GameObject beatPrefab;
    private float edgeOfFrame=-10;
    public Dictionary<string, Sprite> redSprites=new Dictionary<string,Sprite>();    
    public Dictionary<string, Sprite> yellowSprites=new Dictionary<string,Sprite>();
    public Dictionary<string, float> heights=new Dictionary<string,float>{
        {"up",2f},
        {"right",1.333f},
        {"down",0f},
        {"left",0.666f}
    };
    string upArrowPath="Visual_Assests/Combat/Arrows/upArrow";
    string rightArrowPath="Visual_Assests/Combat/Arrows/rightArrow";
    string downArrowPath="Visual_Assests/Combat/Arrows/downArrow";
    string leftArrowPath="Visual_Assests/Combat/Arrows/leftArrow";
    string yellowUpArrowPath="Visual_Assests/Combat/Arrows/yellowUpArrow";
    string yellowRightArrowPath="Visual_Assests/Combat/Arrows/yellowRightArrow";
    string yellowDownArrowPath="Visual_Assests/Combat/Arrows/yellowDownArrow";
    string yellowLeftArrowPath="Visual_Assests/Combat/Arrows/yellowLeftArrow";
    private float distanceToMove;
    public int nonYellowIndex=0;
    public int backupCount=0;

    // Start is called before the first frame update
    void Start()
    {
        //load the sprites
        redSprites["up"]=Resources.Load<Sprite>(upArrowPath);
        redSprites["right"]=Resources.Load<Sprite>(rightArrowPath);
        redSprites["down"]=Resources.Load<Sprite>(downArrowPath);
        redSprites["left"]=Resources.Load<Sprite>(leftArrowPath);
        yellowSprites["up"]=Resources.Load<Sprite>(yellowUpArrowPath);
        yellowSprites["right"]=Resources.Load<Sprite>(yellowRightArrowPath);
        yellowSprites["down"]=Resources.Load<Sprite>(yellowDownArrowPath);
        yellowSprites["left"]=Resources.Load<Sprite>(yellowLeftArrowPath);
    }

    public void reset(){      
        //nonYellowIndex=0;        
        //beats=new List<GameObject>();
    }

    public void addBeat(string key){ 
        GameObject newBeat=Instantiate(beatPrefab);
        if(beats.Count!=0){newBeat.GetComponent<SpriteRenderer>().sprite = redSprites[key];}  
        else{newBeat.GetComponent<SpriteRenderer>().sprite = yellowSprites[key];}
        newBeat.transform.position+=Vector3.up*heights[key];
        newBeat.tag=key;
        //this will require some math and trial and error, but will control height of the beat indicators
        beats.Add(newBeat);      
    }

    //mark for cleanup
    public void advanceBeats(float distanceToMove,float time){
        backupCount++;        
        ArrayList beatsToRemove=new ArrayList();
        foreach (GameObject beat in beats){
            beat.transform.position+=Vector3.left*distanceToMove;
            if(beat.transform.position.x<=edgeOfFrame){
                beatsToRemove.Add(beat);                
            } 
           //if(beat.transform.position.x<=-5.93) {Debug.Log(time);}        
        }
        foreach (GameObject beat in beatsToRemove){
            nonYellowIndex--;            
            beats.Remove(beat);
            Destroy(beat);            
        }
        if(nonYellowIndex<0){
            nonYellowIndex=0;
            if(beats.Count>0){beats[nonYellowIndex].GetComponent<SpriteRenderer>().sprite = yellowSprites[beats[nonYellowIndex].tag];}         
        }

    }

    
    //mark for cleanup with the previous one
    public void makeNextBeatYellow(){    
        if(beats.Count>nonYellowIndex){
            Debug.Log("beatCount"+beats.Count);
            Debug.Log("index"+nonYellowIndex);
            beats[nonYellowIndex].GetComponent<SpriteRenderer>().sprite = redSprites[beats[nonYellowIndex].tag];            
        }           
        if(beats.Count>nonYellowIndex+1){            
            nonYellowIndex++;
            beats[nonYellowIndex].GetComponent<SpriteRenderer>().sprite = yellowSprites[beats[nonYellowIndex].tag];                       
        }
        else if(beats.Count==1){beats[0].GetComponent<SpriteRenderer>().sprite = redSprites[beats[0].tag];}
    }

    public void printBeats(){
        foreach (GameObject beat in beats){
            Debug.Log(beat.transform.position.x);
        }       
    }
}
