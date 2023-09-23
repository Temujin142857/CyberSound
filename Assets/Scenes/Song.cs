using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Song
{
    private AudioClip audioClip;
    private string songPath;
    private float bpm;
    private string name;
    private List<KeyValuePair<float,string>> tappers;
    private string effect;
    
    public Song(string songName, string songFolderPath, string audioFolderPath){
        string songPath=songFolderPath+"/"+songName;  
        string audioPath=audioFolderPath+"/"+songName;
        name=songName;      
        tappers=new List<KeyValuePair<float, string>>();
        Debug.Log(songPath);
        TextAsset saveFile = Resources.Load(songPath) as TextAsset;
        Debug.Log(saveFile==null);
        audioClip=Resources.Load<AudioClip>(audioPath);
        reformatSongString(saveFile.text);        
    }

    private void reformatSongString(string songInfo){
        //format, line one is tappers seperated by commas
        //line two is keynames, end each line with a semi colon
        //line 3 is the bpm
        //line 4 is the dmg scaling
        string num="";
        float tempTapper=0F;
        //bool buttonReading=false;
        int lineCount=0;
        foreach(char c in songInfo.ToCharArray()){            
            if(c==','){
                if(lineCount==0){
                    tempTapper=float.Parse(num.Trim('\n','\r'));
                }
                else{
                    tappers.Add(new KeyValuePair<float, string>(tempTapper,num.Trim('\n','\r')));
                }
                num="";                
            }
            else if(c==';'){                
                if(lineCount==0){
                    tempTapper=float.Parse(num.Trim('\n','\r'));  
                }
                else if(lineCount==1){
                    tappers.Add(new KeyValuePair<float, string>(tempTapper,num.Trim('\n','\r')));
                }
                else if(lineCount==2){
                    bpm=float.Parse(num.Trim('\n','\r'));
                }
                else if(lineCount==3){
                    effect=num.Trim('\n','\r');
                }
                num="";         
                lineCount++;
            }
            else{
                num+=c;
            }            
        }
    }

    //getters
    
    public string getName(){
        return name;
    }

    public float getBPM(){
        return bpm;
    }

    public List<KeyValuePair<float,string>> getTappers(){
        return tappers;
    }

    public string getEffect(){
        return effect;
    }

    public AudioClip getAudioClip(){
        return audioClip;
    }
    

}
