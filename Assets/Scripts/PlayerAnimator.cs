using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour {

    [SerializeField]
    private Animator anim;
    [SerializeField]
    private ParticleSystem muzzleFlash;
    [SerializeField]
    private bool LMB_down;

    private void Awake() {
        anim = GetComponent<Animator>();
        LMB_down = false;
    }

    private void Update() {
        LMB_down = Input.GetMouseButtonDown(0);
        
        //anim.SetTrigger("fire");
        anim.SetBool("LMB_down", LMB_down); // starts fire animation

        if (LMB_down) {
            muzzleFlash.Play();
        }
        
    }

}
