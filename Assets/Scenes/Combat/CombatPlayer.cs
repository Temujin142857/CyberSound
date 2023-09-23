using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CombatPlayer : MonoBehaviour
{

    private Dictionary<string, Song> defaultSongs;
    private List<string> defaultSongNames=new List<string>(){"Song1","Song2","Song3","Song4"};
    private const string playerSongFolderPath="Song_Info/Player_Songs";
    private const string playerAudioFolderPath="Music/Player_Songs";

    public int startingHP=100;
    public int hp;

    public TMP_Text hpIndicator;

    public Dictionary<string,Song> songs=new Dictionary<string, Song>();
    public List<string> songNames = new List<string>();

    private bool attacking=false;
    private bool beingHit=false;

    void Awake(){
        defaultSongs=new Dictionary<string, Song>(){{"Song1", new Song("Song1",playerSongFolderPath,playerAudioFolderPath)},{"Song2", new Song("Song2",playerSongFolderPath,playerAudioFolderPath)},{"Song3", new Song("Song3",playerSongFolderPath,playerAudioFolderPath)},{"Song4", new Song("Song4",playerSongFolderPath,playerAudioFolderPath)}};
     }

    // Start is called before the first frame update
    void Start()
    {
       reset();
    }

    public void reset(){
        hp=startingHP;
        hpIndicator.text="hp:"+hp;
    }    

    // Update is called once per frame
    void Update()
    {
        if(attacking){
            attackingUpdate();
        }
        else if(beingHit){
            beingHitUpdate();
        }
    }

    public void loadSongData(){  
        try{        
            songs=Overlord.Instance.songsPrepared;
            songNames=Overlord.Instance.songsPreparedL;
        }
        catch(NullReferenceException e){
            songs=defaultSongs;            
            songNames=defaultSongNames;
        }
    }

    public void updateHP(){
        try{startingHP=Overlord.Instance.hp;}
        catch(NullReferenceException e){}
        reset();
    }

    private void attackingUpdate(){
        //handle the animation
        //make sure to turn attacking off and reset everything afterwards
        //storing the time the attack starts might be useful
        //that would occur in the activateAttacking animation function
    }

    private void beingHitUpdate(){

    }
    
    //the controller will trigger these
    public void activateGettingHitAnimation(){

    }

    public void activateAttackingAnimation(){

    }

    public void takeDamage(int dmg){
        activateGettingHitAnimation();
        hp-=dmg;
        hpIndicator.text="hp:"+hp;
        Debug.Log(hp);
    }

    public void heal(int healAmount){
        hp+=healAmount;
        if(hp>startingHP){
            hp=startingHP;
        }
        hpIndicator.text="hp:"+hp;
    }

    public bool isAlive(){
        return hp>0;
    }


    //setters
    public void setStartingHP(int startingHP){
        this.startingHP=startingHP;
    }


    //getters    
    public int getHP(){
        return hp;
    }

    public Dictionary<string,Song> getSongs(){
        return songs;
    }

    

}
