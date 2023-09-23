using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

//Glossary note: tapper is the name for a beat the player is expected to press a key on
public class CombatController : MonoBehaviour
{
    private string defaultEnemyName="PlantMonster";
    private int defaultDifficulty=1;



    public AudioSource musicSource;
    public CombatPlayer playerScript;
    public CombatEnemy enemyScript;    
    
    public StunIndicatorController StunIndicatorController;
    public MenuOptionController[] menuOptionControllers=new MenuOptionController[4]; 
    public MenuIndicatorController menuIndicatorController;    
    public RythmInterfaceController rythmInterfaceController;
    public BeatFinishLineController beatFinishLineControllerGood;
    public BeatFinishLineController beatFinishLineControllerPerfect;
    bool dead;

    
    //song resource data
    //the idea is the enemy has a set number of option, who's path is listed here
    //and the player can prepare 4 songs in the overworld then their paths would be stored here
    //if the loading time is an issue preload them in the overworld, but they only load once per combat so it should be fine    
    
    private int count=0;
    private int difficulty;

    //song data
    public Dictionary<string,Song> playerSongs=new Dictionary<string,Song>();
    public Dictionary<string,Song> enemySongs=new Dictionary<string,Song>();
    public List<string> playerSongNames=new List<string>(); 
    public List<string> enemySongNames=new List<string>();   
    public AudioClip playerIntro;
    public AudioClip enemyIntro;    

    //data related to the current song
    public List<KeyValuePair<float,string>> tappers=new List<KeyValuePair<float,string>>();
    private string currentSongName;
    private float bpm;
    private float secPerBeat;
    public float songPositionInSec;
    private float songPositionInBeats;
    private float dspSongTime;        

    private float rangeOfPerfectHitInSeconds=0.2f;
    private int pointsForPerfectHit=30;

    private static float rangeOfGoodHitInSeconds=0.4f;
    private float postLeewayInSeconds=rangeOfGoodHitInSeconds/2;
    private const int pointsForGoodHit=10;

    //this is the placeholder for the length of the bridge each Song has, so we skip it when we move directly to defending with no menuing
    private const float lengthOfSilenceBetweenSongAndDefence=0;
    private float lengthOfSilenceAtEndOfDefenceInbeats=8;
    private float lengthOfSilenceAtEndOfDefenceInSeconds=8f;

    private int scoreNum;  
    //tapper index points to the tapper the player is on, should be similar to actual, unless the player presses multiple times before one beat
    private int tapperIndex;
    //actual tapper inidex points to the next tapper the song will pass
    private int actualTapperIndex;
    //future tapper index points to the next tapper to load into the rhythm interface
    private int futureTapperIndex;
    private float timeMenuStart;
    private float menuingDuration=5.053f;
    private bool attacking;
    private bool defending;
    private bool firstRoundMenuing;
    private bool menuing;

    private int stunChance;
    private int cleanseChance;
    private bool playerDebuffed;
    private int threshHold=26;

    private static float secondsEarlyToDisplayTappers;
    private static float movementTicksPerSecond=60f;
    private float beatsAdvanceCount;
    //distance between beat spawn point and finish line
    public static float lengthOfInterface=17.6f;
    private float secondsPerMovementTick=1/movementTicksPerSecond;
    private float beatsEarlyToDisplayTappers;
    private float distanceToMovePerTick;
    //movementTicksPerSecond*
    //no longer necessary, selected clip is loaded in menu doesnt need to be saved
    //public AudioClip selectedClip;

    public int numberOfSongs=1;
    private int currentNumberOfSongs;

    private string enemyName="soldierDude";
    public Animator animator;
    public Animator enemyAnimator;

    // Start is called before the first frame update
    void Start()
    {        
        StunIndicatorController=GameObject.Find("StunIndicator").GetComponent<StunIndicatorController>();
        StunIndicatorController.setActive(false);

        animator=GameObject.Find("Player").GetComponent<Animator>();
        enemyAnimator=GameObject.Find("Enemy").GetComponent<Animator>();
        playerScript=GameObject.Find("Player").GetComponent<CombatPlayer>(); 
        enemyScript=GameObject.Find("Enemy").GetComponent<CombatEnemy>();
        menuIndicatorController=GameObject.Find("MenuIndicator").GetComponent<MenuIndicatorController>();
        rythmInterfaceController=GameObject.Find("RythmInterface").GetComponent<RythmInterfaceController>();
        beatFinishLineControllerGood=GameObject.Find("BeatFinishLineGood").GetComponent<BeatFinishLineController>();
        beatFinishLineControllerPerfect=GameObject.Find("BeatFinishLinePerfect").GetComponent<BeatFinishLineController>();   

        playerIntro=Resources.Load<AudioClip>("Music/Player_Songs/Intro");
        enemyIntro=Resources.Load<AudioClip>("Music/Enemy_Songs/Intro");

        for (int i=0;i<4;i++){
            int num= (i+1);
            string suffix=num.ToString();
            menuOptionControllers[i]=GameObject.Find("MenuOption"+suffix).GetComponent<MenuOptionController>();
            menuOptionControllers[i].activate(false);                      
        }
        menuIndicatorController.activate(false);
        try{
            enemyName=Overlord.Instance.enemyName;
            difficulty=Overlord.Instance.difficulty;
        }
        catch(NullReferenceException e){
            enemyName=defaultEnemyName;
            difficulty=defaultDifficulty;
        }
        threshHold+=5*difficulty;
        enemyScript.setSprite(enemyName);
        enemyScript.name=enemyName;
        enemyScript.loadEnemyData();
        playerScript.loadSongData();
        playerScript.updateHP();
        loadSongs();        
        //Load the AudioSource attached to the Conductor GameObject
        musicSource = GetComponent<AudioSource>(); 
        firstRoundMenu();
        //setupMenu(); 
    }

    void loadSongs(){
        enemySongs=enemyScript.songs;
        playerSongs=playerScript.songs;        
        playerSongNames=playerScript.songNames;   
        enemySongNames=enemyScript.songNames; 
    }

    //Update is called once per frame
    void Update()
    {
        
        //determine how many seconds since the song started
        songPositionInSec = (float)(AudioSettings.dspTime - dspSongTime);

        //determine how many beats since the song started
        songPositionInBeats = songPositionInSec/secPerBeat;

        //updates the current tapper we're looking for        
        if(tappers.Count>actualTapperIndex&&songPositionInSec>(tappers[actualTapperIndex].Key*secPerBeat)+postLeewayInSeconds){
            actualTapperIndex++;
        }

        if(firstRoundMenuing){
            firstRoundMenuUpdate();
        }

        else if(menuing){
            menuUpdate();
        }

        else if(attacking){            
            attackingUpdate();
        }

        else if (defending){
            defendingUpdate();
        }
        //Debug.Log(AudioSettings.dspTime - dspSongTime-songPositionInSec);
        
    }
   


    //this portion handles menuing
    void firstRoundMenu(){     
        firstRoundMenuing=true;   
        displayMenuOverlay();      
    }

    void setupMenu(){
        resetValuesForMenuing();
        displayMenuOverlay();        
    }    

    void resetValuesForMenuing(){
        menuing=true;
        defending=false;
        attacking=false;
        timeMenuStart=(float)(AudioSettings.dspTime - dspSongTime);
    }

    void displayMenuOverlay(){
        for (int i=0;i<menuOptionControllers.Length;i++){            
            menuOptionControllers[i].activate(true); 
            menuOptionControllers[i].setUpSprite(playerSongNames[i]);                       
        }
        menuIndicatorController.activate(true);  
        currentSongName=playerSongNames[0];
        highlightOption(0);
    }

    void removeMenu(){
        for (int i=0;i<menuOptionControllers.Length;i++){            
            menuOptionControllers[i].activate(false);  
            menuIndicatorController.activate(false);                       
        }
    }

    void firstRoundMenuUpdate(){
        //menuIndicatorController.idleRotate();
        if(Input.GetButtonDown("up")){
            
            Debug.Log("whtf"+(playerSongs.Count));
            musicSource.clip=playerSongs[playerSongNames[0]].getAudioClip();
            currentSongName=playerSongNames[0];                       
            attack();
        }
        else if(Input.GetButtonDown("right")){
            musicSource.clip=playerSongs[playerSongNames[1]].getAudioClip();
            currentSongName=playerSongNames[1];
            attack();
        }
        else if(Input.GetButtonDown("down")){
            musicSource.clip=playerSongs[playerSongNames[2]].getAudioClip();
            currentSongName=playerSongNames[2];
            attack();
        }
        else if(Input.GetButtonDown("left")){
            musicSource.clip=playerSongs[playerSongNames[3]].getAudioClip();
            currentSongName=playerSongNames[3];
            attack();
        }        
    }

    void menuUpdate(){
        if(songPositionInSec/secondsPerMovementTick>=beatsAdvanceCount){
            rythmInterfaceController.advanceBeats(distanceToMovePerTick,songPositionInBeats);
            beatsAdvanceCount++;
        } 
        if(Input.GetButtonDown("up")){
            currentSongName=playerSongNames[0];
            highlightOption(0);       
        }
        else if(Input.GetButtonDown("right")){            
            currentSongName=playerSongNames[1];
            highlightOption(1);
        }
        else if(Input.GetButtonDown("down")){
            currentSongName=playerSongNames[2];
            highlightOption(2);
        }
        else if(Input.GetButtonDown("left")){
            currentSongName=playerSongNames[3];
            highlightOption(3);
        } 
        if(songPositionInSec-timeMenuStart>menuingDuration){            
            musicSource.clip=playerSongs[currentSongName].getAudioClip();
            attack();
        }       
    }

    private void highlightOption(int indexOfAssociatedOptions){
        for(int i=0;i<menuOptionControllers.Length;i++){
            if(i==indexOfAssociatedOptions){
                menuOptionControllers[indexOfAssociatedOptions].highlight();
            }
            else {
                menuOptionControllers[i].unHighlight();
            }
        }
        switch (indexOfAssociatedOptions){
            case 0:
                menuIndicatorController.faceUp();
                break;
            case 1:
                menuIndicatorController.faceRight();
                break;
            case 2:
                menuIndicatorController.faceDown();
                break;
            case 3:
                menuIndicatorController.faceLeft();
                break;
        }
    }

    //this portions handles attacking
    void attack(){       
        resetValuesForAttacking();        
        musicSource.Play(); 
    }    

    void resetValuesForAttacking(){
        bpm=playerSongs[currentSongName].getBPM();
        Debug.Log("bpm: "+bpm);
        Debug.Log(currentSongName);
        Debug.Log(playerSongs[currentSongName].getTappers().Count);
        //Calculate the number of seconds in each beat
        secPerBeat = 60f / bpm;         
        //I think this plays from the start each time it's called
        //do some sort of countdown 
        dspSongTime = (float)AudioSettings.dspTime;

        //values fro arrows
        beatsEarlyToDisplayTappers=5;//maybe get that from the time signature?
        secondsEarlyToDisplayTappers=beatsEarlyToDisplayTappers*secPerBeat;
        distanceToMovePerTick=lengthOfInterface/(movementTicksPerSecond*secondsEarlyToDisplayTappers); 
        beatsAdvanceCount=0; 
        // Debug.Log("distancePerTick"+distanceToMovePerTick);     
        // Debug.Log("bpm"+bpm); 
        // Debug.Log("movementTicksPerSeconds"+movementTicksPerSecond);
        // Debug.Log("secondsEarlyToDisplayTappes"+secondsEarlyToDisplayTappers);
        beatFinishLineControllerGood.setWidth(rangeOfGoodHitInSeconds*(distanceToMovePerTick*movementTicksPerSecond));
        beatFinishLineControllerPerfect.setWidth(rangeOfPerfectHitInSeconds*(distanceToMovePerTick*movementTicksPerSecond));
        tappers=playerSongs[currentSongName].getTappers();
        rythmInterfaceController.reset();

        stunChance=0;
        cleanseChance=0;

        firstRoundMenuing=false; 
        menuing=false;
        removeMenu();        
        attacking=true;
        tapperIndex=0;
        actualTapperIndex=0;
        futureTapperIndex=0;   
        currentNumberOfSongs++; 
    }   

    void attackingUpdate(){        
        
        if(tappers.Count>futureTapperIndex&&tappers[futureTapperIndex].Key<=songPositionInBeats+beatsEarlyToDisplayTappers){
            rythmInterfaceController.addBeat(tappers[futureTapperIndex].Value);
            // Debug.Log("Appeard at:"+songPositionInBeats); 
            // Debug.Log(tappersPlayerSongs[currentSongName].Count);
            // Debug.Log(buttonNameAssociatedWithTappersPlayerSongs[currentSongName].Count);
            futureTapperIndex++;
        }    
        if(songPositionInSec/secondsPerMovementTick>=beatsAdvanceCount){
            rythmInterfaceController.advanceBeats(distanceToMovePerTick,songPositionInBeats);
            beatsAdvanceCount++;
        }        
        
        //string is the code for different keys on the board
        //look up a table when we decide on inputs
        //also experiment with what happens if you try to hold down the key
        if (tappers.Count>tapperIndex&&Input.GetButtonDown(tappers[tapperIndex].Value)){          

            //check how close to beat the input was, will definitely need ranges with at least a millisecond or two of leeway        
            //start with 1milli, but fine tune it with testing
            float timing=Math.Abs((tappers[tapperIndex].Key*secPerBeat)-(songPositionInSec));            
            

            //detect if it's a hit miss ect, activate the pressed animation based on the result, record the result
            if(timing<=rangeOfPerfectHitInSeconds){
                //this would qualify as a perfect hit
                activateAttactEffect("Perfect");
            } 
            else if(timing<=rangeOfGoodHitInSeconds){
                //this would qualify as a "good" hit or a close miss        
                activateAttactEffect("Good");
            }
            else{
                //this is the miss
            }          
            tapperIndex++;
            rythmInterfaceController.makeNextBeatYellow();            
            animator.SetTrigger("Shoot");
            enemyAnimator.SetTrigger("Activate");
        }
        else if(Input.anyKeyDown){
            //this is what happens when they press the wrong key
            Debug.Log("Wrong key");
            tapperIndex++;
            rythmInterfaceController.makeNextBeatYellow();            
        }
        else if(actualTapperIndex>tapperIndex){
            //this is what happens when they don't press anything for a beat
            Debug.Log("missed beat at: "+songPositionInSec);     
            rythmInterfaceController.printBeats();
            Debug.Log("printed"); 
            tapperIndex++;      
            rythmInterfaceController.makeNextBeatYellow();     
        }        

        //this assume the player makes only one Song
        //this ends the song once it gets to the end
        if(musicSource.clip.length>7&&musicSource.clip.length-songPositionInSec<lengthOfSilenceBetweenSongAndDefence){   
            System.Random rnd = new System.Random();
            int num = rnd.Next(100);      
            if(num<stunChance){  
                if(difficulty==1){playerScript.takeDamage(10);}  
                StunIndicatorController.setActive(true);
                setupMenu();
            }  
            else{    
                musicSource.Stop();            
                musicSource.clip=enemyIntro;
                dspSongTime = (float)AudioSettings.dspTime;
                beatsAdvanceCount=0;
                musicSource.Play();
            }                
        } 
        else if(musicSource.clip.length<7&&musicSource.clip.length-songPositionInSec<0){
            defend();
        }     
    }

    void activateAttactEffect(string strength){
        switch(playerSongs[currentSongName].getEffect()){
                    case "Damage":
                        if(strength.Equals("Good")){
                            enemyScript.takeDamage(1-difficulty);
                        }
                        else if(strength.Equals("Perfect")){
                            enemyScript.takeDamage(2-difficulty);
                        }
                        break;
                    case "Heal":
                        if(strength.Equals("Good")){
                            playerScript.heal(1-difficulty);
                        }
                        else if(strength.Equals("Perfect")){
                            playerScript.heal(2-difficulty);
                        }
                        break;
                    case "Stun+Damage":
                        if(strength.Equals("Good")){
                            stunChance+=1-difficulty;
                        }
                        else if(strength.Equals("Perfect")){
                            stunChance+=1;
                            enemyScript.takeDamage(1);                            
                        }
                        break;
                    case "Heal+Cleanse":
                        if(strength.Equals("Good")){
                            cleanseChance+=1;
                        }
                        else if(strength.Equals("Perfect")){
                            cleanseChance+=1;
                            playerScript.heal(1);                            
                        }
                        break;
        }
    }

    //this portion handles defending
    void defend(){
        musicSource.Stop();        
        selectEnemySong();
        resetValuesForDefending();        
        musicSource.Play(); 
    }    

    void selectEnemySong(){
        var rand=new System.Random();
        currentSongName=enemySongNames[rand.Next(enemySongNames.Count)];
        musicSource.clip=enemySongs[currentSongName].getAudioClip();
    }

    void resetValuesForDefending(){
        bpm=enemySongs[currentSongName].getBPM();
        //Calculate the number of seconds in each beat
        secPerBeat = 60f / bpm;         
        //I think this plays from the start each time it's called
        //do some sort of countdown 
        dspSongTime = (float)AudioSettings.dspTime;
        tappers=enemySongs[currentSongName].getTappers();    

        //arrowMovement
        beatsEarlyToDisplayTappers=5;//maybe get that from the time signature?
        secondsEarlyToDisplayTappers=beatsEarlyToDisplayTappers*secPerBeat;
        distanceToMovePerTick=lengthOfInterface/(movementTicksPerSecond*secondsEarlyToDisplayTappers);
        beatFinishLineControllerGood.setWidth(rangeOfGoodHitInSeconds*(distanceToMovePerTick*movementTicksPerSecond));
        beatFinishLineControllerPerfect.setWidth(rangeOfPerfectHitInSeconds*(distanceToMovePerTick*movementTicksPerSecond));
        rythmInterfaceController.reset();
        beatsAdvanceCount=0;
        rythmInterfaceController.reset();

        attacking=false;
        defending=true;
        tapperIndex=0;
        actualTapperIndex=0;
        futureTapperIndex=0;  

        StunIndicatorController.setActive(false);     
    }        
       
    void displayDefenceOverlay(){

    }

    void defendingUpdate(){        
        if(!playerScript.isAlive()){
            if(dead){return;}
            dead=true;
            musicSource.Stop();
            animator.SetTrigger("Die");
            StartCoroutine(startCutscene(5.0f));
            return;
        }
        if(tappers.Count>futureTapperIndex&&tappers[futureTapperIndex].Key<=songPositionInBeats+beatsEarlyToDisplayTappers){
            rythmInterfaceController.addBeat(tappers[futureTapperIndex].Value);
            futureTapperIndex++; 
        }    
        if(songPositionInSec/secondsPerMovementTick>=beatsAdvanceCount){
            rythmInterfaceController.advanceBeats(distanceToMovePerTick,5);
            beatsAdvanceCount++;
        }

        //string is the code for different keys on the board
        //look up a table when we decide on inputs
        //also experiment with what happens if you try to hold down the key
        if (tappers.Count>tapperIndex&&Input.GetButtonDown(tappers[tapperIndex].Value)){          

            //check how close to beat the input was, will definitely need ranges with at least a millisecond or two of leeway        
            //start with 1milli, but fine tune it with testing
            float timing=Math.Abs((tappers[tapperIndex].Key*secPerBeat)-(songPositionInSec));            

            //detect if it's a hit miss ect, activate the pressed animation based on the result, record the result
            if(timing<=rangeOfPerfectHitInSeconds){
                //this would qualify as a perfect hit
                scoreNum+=pointsForPerfectHit;
            } 
            else if(timing<=rangeOfGoodHitInSeconds){
                //this would qualify as a "good" hit or a close miss
                scoreNum+=pointsForGoodHit;            
                playerScript.takeDamage(1);
            }
            else{
                //this is the miss
                playerScript.takeDamage(2+difficulty);                
            }          
            tapperIndex++;
            rythmInterfaceController.makeNextBeatYellow();
            enemyAnimator.SetTrigger("Activate");
        }
        else if(Input.anyKeyDown){
            //this is what happens when they press the wrong key
            Debug.Log("Wrong key");
            playerScript.takeDamage(2+difficulty);
            tapperIndex++;
            rythmInterfaceController.makeNextBeatYellow();     
            enemyAnimator.SetTrigger("Activate");       
        }
        else if(actualTapperIndex>tapperIndex){
            //this is what happens when they don't press anything for a beat
            Debug.Log("missed beat at: "+songPositionInSec);   
            playerScript.takeDamage(2+difficulty);  
            rythmInterfaceController.printBeats();
            Debug.Log("printed");  
            tapperIndex++;      
            rythmInterfaceController.makeNextBeatYellow();     
        }
               

        //this assume the player makes only one Song
        //this ends the song once it gets to the end
        if(musicSource.clip.length-songPositionInSec<lengthOfSilenceAtEndOfDefenceInSeconds){  
            bool stun=false;
            if(scoreNum>threshHold){
                switch(playerSongs[currentSongName].getEffect()){
                        case "Damage":
                            enemyScript.takeDamage(5);
                            break;
                        case "Heal":
                            playerScript.heal(5);
                            break;                        
                } 
            }            
            setupMenu();
            // if(currentNumberOfSongs>=numberOfSongs){
            //     currentNumberOfSongs=0;
            //     defend();
            // }  
            // else{
            //     currentNumberOfSongs=0;
            //     setupMenu();
            // }                
        }
    }

    

    private IEnumerator startCutscene(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("EndTutorial");
    }


            
}
