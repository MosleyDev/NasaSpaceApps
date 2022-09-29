using System;
using System.Collections;
using System.Collections.Generic;
using binc.PixelAnimator;
using UnityEngine;

public class RoverController : MonoBehaviour{


    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private AnimationManager anim;
    [SerializeField] private PixelAnimation river;
    [SerializeField] private PixelAnimation riverReversed;
    
    private float xInput;
    private int facingDirection;

    private void Update(){
        xInput = Input.GetAxisRaw("Horizontal");

        facingDirection = xInput switch{
            < 0 => -1,
            > 0 => 1,
            _ => facingDirection
        };


        if (xInput > 0) {
            anim.Play(river, anim.ActiveFrame);
        }
        else if (xInput < 0) {
            anim.Play(riverReversed, anim.ActiveFrame);
        }
        else {
            anim.Stop();
        }
        
    }

    private void FixedUpdate(){
        body.velocity = new Vector2(xInput * speed, body.velocity.y);
    }
}
