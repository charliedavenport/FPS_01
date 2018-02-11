using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCollider : MonoBehaviour {

    public PlayerController player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Ground") {
            player.jumping = false;
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "Ground") {
            player.grounded = true;
            if (!player.sprinting && player.runSpeed > 5f) {
                player.baseRunSpeed = 5f;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Ground") {
            player.grounded = false;
        }
    }
}
