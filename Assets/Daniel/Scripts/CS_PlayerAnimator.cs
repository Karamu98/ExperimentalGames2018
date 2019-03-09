using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_PlayerAnimator : MonoBehaviour {

    public static Animator aAnim;
    public float speed = 10;
    public float rotationSpeed = 100;
    // Use this for initialization
    void Start () {
        aAnim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * speed;

        aAnim.SetBool("forward", (translation > 0 ? true : false));
        aAnim.SetBool("back", (translation < 0 ? true : false));
        aAnim.SetBool("left", (rotation < 0 ? true : false));
        aAnim.SetBool("right", (rotation > 0 ? true : false));

        //aAnim.SetFloat("direction", translation);
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;
        transform.Translate(rotation, 0, translation);
        //transform.Rotate(0, rotation, 0);


        

        if (Input.GetButtonDown("Jump"))
        {
            aAnim.SetTrigger("isJumping");
        }

        if (translation != 0 || rotation != 0)
        {
            aAnim.SetBool("isRunning", true);
            if(rotation > 0)
            {
                //aAnim.SetBool("isRunningLeft", true);
            }
            else if( rotation < 0)
            {
                //aAnim.SetBool("isRunningRight", true);
            }
        }
        else
        {
            //aAnim.SetBool("isRunningLeft", false);
            //aAnim.SetBool("isRunningRight", false);
            aAnim.SetBool("isRunning", false);

        }




    }
}
