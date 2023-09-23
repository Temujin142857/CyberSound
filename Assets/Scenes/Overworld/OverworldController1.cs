using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class OverworldController1 : MonoBehaviour
{
    

    public float upBorder;
    public float rightBorder;
    public float downBorder;
    public float leftBorder;
    public bool inInteraction;
    public float movementPerSecond;
    
    public float interactionRadius;
    public bool isInControl=true;
    public Camera camera;
    public TMP_Text dialogueBox;
    public float cameraLength=23.4f;
    public Dictionary<string,AnimationClip> animations=new Dictionary<string,AnimationClip>();
    public AnimationClip animUp;
    public AnimationClip animRight;
    public AnimationClip animDown;
    public AnimationClip animLeft;
    public Animator animator;
    public GameObject Piskel;
    public float piskelLineRight;
    
    

    // Start is called before the first frame update
    void Start()
    {
        animator=GetComponent<Animator>();        
    }

    // Update is called once per frame
    void Update()
    {        
        normalUpdate();
    }
    
    public void endInteraction(){
        isInControl=true;
    }



    void normalUpdate(){
        float movementThisFrame=movementPerSecond*Time.deltaTime;
        if(isInControl){
            if(Input.GetButton("up")&&transform.position.y<upBorder){
                transform.position+=Vector3.up*movementThisFrame; 
                changeAnimation("up");
            }
            else if(Input.GetButton("down")&&transform.position.y>downBorder){
                transform.position+=Vector3.down*movementThisFrame; 
                changeAnimation("down");
            }
            if(Input.GetButton("left")&&transform.position.x>leftBorder){
                changeAnimation("left");
                transform.position+=Vector3.left*movementThisFrame;
                if(transform.position.x>piskelLineRight){
                    Piskel.transform.position+=Vector3.left*movementThisFrame;
                }
                if(camera.transform.position.x>leftBorder+cameraLength/2&&transform.position.x<camera.transform.position.x){
                        camera.transform.position+=Vector3.left*movementThisFrame;
                        dialogueBox.transform.position+=Vector3.left*movementThisFrame;
                }
                
            }
            else if(Input.GetButton("right")&&transform.position.x<rightBorder){
                changeAnimation("right");
                transform.position+=Vector3.right*movementThisFrame;
                if(transform.position.x>piskelLineRight){
                    Piskel.transform.position+=Vector3.right*movementThisFrame;
                }
                if(camera.transform.position.x<rightBorder-cameraLength/2&&transform.position.x>camera.transform.position.x){
                    camera.transform.position+=Vector3.right*movementThisFrame;
                    dialogueBox.transform.position+=Vector3.right*movementThisFrame;
                }
                
            }
            if(Input.GetButtonDown("interact")){   
                Debug.Log("pressed"); 
                List<GameObject> possibleTargets=new List<GameObject>();        
                GameObject[] interactableObjects = GameObject.FindGameObjectsWithTag("Interactable");
                for (int i=0;i<interactableObjects.Length;i++){
                    if((interactableObjects[i].transform.position-transform.position).magnitude<(Vector3.one*interactionRadius).magnitude){                        
                        possibleTargets.Add(interactableObjects[i]);
                    }
                }
                if(possibleTargets.Count==1){
                    Debug.Log("count1"); 
                    possibleTargets[0].GetComponent<AbstractInteractable>().beginInteraction();
                    isInControl=false;
                }
                else if(possibleTargets.Count==0){
                    return;
                }
                else {
                    float closestDistance=interactionRadius+1f;
                    GameObject closestTarget=null;
                    foreach (GameObject target in possibleTargets){
                        float targetDistance=(target.transform.position-transform.position).magnitude;
                        if(targetDistance<=closestDistance){
                            closestDistance=targetDistance;
                            closestTarget=target;
                        }
                    }
                    Debug.Log("countMany"); 
                    closestTarget.GetComponent<AbstractInteractable>().beginInteraction();
                    isInControl=false;
                }

            }
            if(Input.GetButtonDown("combatShortcut")){
                Overlord.Instance.enemyName="PLantMonster";
                SceneManager.LoadScene("Combat");
            }
            
        }
    }

    private void changeAnimation(string newDirection){    
        switch(newDirection){
            case "up":
                animator.SetInteger("Direction",0);
                break;
            case "right":
                animator.SetInteger("Direction",1);
                break;
            case "down":
                animator.SetInteger("Direction",2);
                break;
            case "left":
                animator.SetInteger("Direction",3);
                break;
        }
    }

    //since we don't know the exact details of what each game object will do
    //every object tagged with interactable must have a script which implements AbstractInteractable
    //all of those scripts will have a functon called beginInteraction
    //then that script will handle dialogue or combat as necessary
    

    

}
