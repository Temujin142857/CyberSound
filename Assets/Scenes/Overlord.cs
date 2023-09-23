using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Overlord : MonoBehaviour
{
    public static Overlord Instance;
    private static string fileName="SaveFiles/saveFile1";    

    //data for the overworld to battle transition    
    public string enemyName;    
    public int hp=100;    
    public Dictionary<string, Song> songsPrepared;
   
    private const string playerSongFolderPath="Music/Player_Songs";
    private const string playerAudioFolderPath="Song_Info/Player_Songs";

    //loading game from save
    public string sceneName;
    public string partOfScene;
    public List<string> songsKnown;   
    public List<string> songsPreparedL=new List<string>(){"Song1","Song2","Song3","Song4"};
    public int difficulty=0;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);    
        loadSaveData(fileName);        
    }


    public void addToKnownSongs(string songName){
        songsKnown.Add(songName);
    }

    public void getSongInfo(){

    }    

    public void Save(){
        //write data to the savefile in the json format
        string data="{";
        data+="\"hp\":"+hp+", ";
        data+="\"partOfScene\":"+partOfScene+", ";
        data+="\"sceneName\":"+sceneName+", ";
        data+=makeStringForArray(songsKnown,"songsKnown");
       // data+=makeStringForArray(songsPrepared,"songsPrepared");
        data+="}";
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(fileName, true);
        writer.WriteLine(data);
        writer.Close();
        //might need to reload the file to save the changes?
    }

    public void fillInDataAssociatedWithSongsPrepared(){
        //loop through all the known songs and store their data
        //use loadSongData
        songsPrepared=new Dictionary<string, Song>();
        foreach(string songName in songsPreparedL){
            songsPrepared.Add(songName,new Song(songName,playerSongFolderPath,playerAudioFolderPath));
        }
    }

        public void loadSaveData(string filePath)
    {
        TextAsset saveFile = Resources.Load(filePath) as TextAsset;
        JsonUtility.FromJsonOverwrite(saveFile.text, this);
    }

    
    //helper method for saving
    private string makeStringForArray(List<string> array, string arrayName){
        string data="";
        data+="\""+arrayName+"\":[";
        foreach(string song in array){
            data+="\""+song+"\", ";
        }
        data+="], ";
        return data;
    }

    // Given JSON input:
    // {"hp":3, "enemyName":"kevin"}
    // the Load function will change the object on which it is called such that
    //hp == 3 and enemyName == "kevin"
    // all other fields will be left unchanged
    //still some errors with this, but not crucial until after the gamejam



}
