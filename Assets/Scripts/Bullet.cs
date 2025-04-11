using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float life;
    private float screenRight;

    void Awake(){
       
    // Calculate the screen's right boundary in world space
    screenRight = Camera.main.ViewportToWorldPoint(new Vector3(2, 3, 0)).x;
    /*if(transform.position.x > screenRight){
        Destroy(gameObject);
    }*/
        Destroy(gameObject, life);
    }


    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "Asteroid"){
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if(collision.gameObject.tag == "Rock" || collision.gameObject.tag == "CorrectAnswer"){
            Destroy(gameObject);
        }
        
    }
}
