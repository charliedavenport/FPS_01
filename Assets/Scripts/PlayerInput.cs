using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private PlayerAnimator playerAnim;

    public bool w, a, s, d,
        shift_down, shift, shift_up, space;
    public float mouseX, mouseY;

    private void Update() {
        w = Input.GetKey(KeyCode.W);
        a = Input.GetKey(KeyCode.A);
        s = Input.GetKey(KeyCode.S);
        d = Input.GetKey(KeyCode.D);
        space = Input.GetKeyDown(KeyCode.Space);
        shift = Input.GetKey(KeyCode.LeftShift);
        shift_down = Input.GetKeyDown(KeyCode.LeftShift);
        shift_up = Input.GetKeyUp(KeyCode.LeftShift);
        mouseY = -Input.GetAxis("Mouse Y");
        mouseX = Input.GetAxis("Mouse X");
    }

}