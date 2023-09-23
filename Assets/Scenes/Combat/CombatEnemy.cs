using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CombatEnemy : MonoBehaviour
{
    private int startingHP=100;
    private const string enemySongFolderPath="Song_Info/Enemy_Songs";
    private const string enemyAudioFolderPath="Music/Enemy_Songs";
    private int hp=10;
    
    public TMP_Text hpIndicator;

    public List<string> songNames=new List<string>();
    public Dictionary<string, Song> songs=new Dictionary<string, Song>();


    private bool attacking=false;
    private bool beingHit=false;
    public int numberOfAttacks{get;set;}=1;
    public string name{get;set;}="PlantMonster";
    
       

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

    private void attackingUpdate(){
        //handle the animation
        //make sure to turn attacking off and reset everything afterwards
        //storing the time the attack starts might be useful
        //that would occur in the activateAttacking animation function
    }

    private void beingHitUpdate(){

    }
    
    public void activateGettingHitAnimation(){

    }

    public void activateAttackingAnimation(){

    }
    
    public void loadEnemyData(){
        string filePath="Enemy_Files/"+name;        
        TextAsset saveFile = Resources.Load(filePath) as TextAsset;
        Debug.Log(name+saveFile.text);
        string element="";
        int line=0;
        foreach(char c in saveFile.text.ToCharArray()){
            if(c==','){
                songNames.Add(element.Trim('\r','\n'));
                element="";
            }
            else if(c==';'&&line==0){
                songNames.Add(element.Trim('\r','\n'));
                element="";
                line++;
            }
            else if(c==';'&&line==1){
                startingHP=int.Parse(element.Trim('\r','\n'));
                element="";
                line++;
            }
            else {element+=c;}
        }
        foreach(string song in songNames){
            Debug.Log("song"+song);
        }  
        fillInDataAssociatedWithSongs();
        reset();
            
    }

    public void takeDamage(int dmg){
        activateGettingHitAnimation();
        hp-=dmg;
        hpIndicator.text="hp:"+hp;
        //Debug.Log(hp);
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
    

    public void setSprite(string characterName){
        Sprite sprite= Resources.Load<Sprite>("Visual_Assests/Combat/Characters/"+characterName);
        this.GetComponent<SpriteRenderer>().sprite=sprite;
    }

    public void fillInDataAssociatedWithSongs(){      
        foreach (string songName in songNames){       
            songs.Add(songName,new Song(songName,enemySongFolderPath,enemyAudioFolderPath));
        }
    }

    public Dictionary<string,Song> getSongs(){
        return songs;
    }
}
