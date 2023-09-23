using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChildController : MonoBehaviour, AbstractInteractable
{
    private string[] dialogue=new string[]{
        "Child: You're the soldier, Hello!",
        "Tuto: It's not safe out here kid, go back to the camp.",
        "Child: I'm not kid, I'm Piskel",
        "Tuto: Well Piskel, there's some dangerous plants ahead, so go back to the camp.",
        "Piskel: Alright!"
    };

    
    private int dialogueIndex;
    public TMP_Text dialogueBox;
    private bool interactionActive=false;
    private OverworldController1 master;
    // Start is called before the first frame update
    void Start()
    {       
        master=GameObject.Find("Player").GetComponent<OverworldController1>();
    }

    // Update is called once per frame
    void Update()
    {
        if(interactionActive&&Input.GetButtonDown("interact")){
            if(dialogueIndex<dialogue.Length){
                displayNextLineOfDialogue();
            }
            else {
                interactionActive=false;
                dialogueBox.text="";
                master.endInteraction();
            }        
        }        
    }

    public void beginInteraction(){        
        dialogueIndex=0;         
        interactionActive=true;
    }

    public void displayNextLineOfDialogue(){
        dialogueBox.text=dialogue[dialogueIndex];
        dialogueIndex++;        
    }

    public void resetDialogueIndex(){
        dialogueIndex=0; 
    }

    private void requestPlayerChooseAnOption(){

    }

}
