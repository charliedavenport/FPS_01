using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public UIController gui;

    public BoxCollider groundCollider;

    private PlayerInput pInput;

    public float runSpeed;

    public float sprint;
    public float maxSprint = 1f;
    public float maxRunSpeed = 15f;
    public float baseRunSpeed = 7f;

    public float mouseSensitivity = 150f;
    public float clampAngle = 80f; // limits rotation up and down (rotX)
    public Transform playerCamera; // child of player gameobj

    public float jumpVelocity = 5f;
    public float fallMultiplier = 3.5f;
    public float maxSpeed = 15f;

    public bool grounded; // make public for debugging

    public bool jumping;

    private Rigidbody rb;

    private bool w, a, s, d, shift_down, shift, shift_up, space;
    private float mouseX, mouseY;

    private float rotY;
    private float rotX;

    private bool moving;
    public bool sprintReady;
    public bool sprinting; // need two bools for 3 cases: sprinting, recharging, ready-not-sprinting
    public float minReadySprint; // min amt of sprint resource needed to start sprinting
    private float sprintTime = 2.0f;
    private float rechargeTime = 5.0f;

    public bool sprintCooldown;

    private float accell = 15f;
    private float decell = -15f;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        grounded = false;
        sprint = maxSprint;
        sprintReady = true;
        minReadySprint = 0.75f * maxSprint;
        gui.sprintBar.GetComponent<Image>().color = Color.green;
        gui.minSprintReadyBar.color = Color.white;
        runSpeed = baseRunSpeed;

        sprintCooldown = false;
        // set in editor instead
        //gui.minSprintReadyBar.transform.Translate((minReadySprint / maxSprint) * gui.maxBarWidth, 0, 0);

        //StartCoroutine(speed_decay()); // gotta go fast
    }

    IEnumerator sprint_cooldown() {
        // player cannot sprint for one second after sprinting ends
        sprintCooldown = true;
        yield return new WaitForSeconds(1.0f);
        sprintCooldown = false;
        yield return null;
    }

    IEnumerator speed_decay() {
        while (true) {
            if (runSpeed > 3) {
                runSpeed -= 1f;
                
            }
            yield return new WaitForSeconds(5f);
        }
    }//speed_decay

    // coroutine for recharging sprint resource
    IEnumerator sprint_recharge_routine() {
        Debug.Log("starting recharge coroutine");
        float n_iter = 50;
        float iter_time = rechargeTime / n_iter;

        sprinting = false;
        while (true) {
            if (sprinting) break;
            if (sprint < maxSprint) {
                sprint += maxSprint / n_iter;
                if (sprint < minReadySprint) {
                    sprintReady = false;
                    gui.sprintBar.GetComponent<Image>().color = Color.red;
                    gui.minSprintReadyBar.color = Color.green;
                }
                else {
                    sprintReady = true;
                    gui.sprintBar.GetComponent<Image>().color = Color.green;
                    gui.minSprintReadyBar.color = Color.white;
                }
            }
            else {
                sprint = maxSprint;
                break;
            }
            yield return new WaitForSeconds(iter_time); // next iteration
        }//while

        sprintReady = true;
    }

    // sprint coroutine
    IEnumerator sprint_routine() {
        Debug.Log("starting sprint coroutine");
        float n_iter = 50;
        float iter_time = sprintTime / n_iter;

        gui.sprintBar.GetComponent<Image>().color = Color.grey;

        sprinting = true;
        sprintReady = false;
        //runSpeed = 10f;

        for (int i = 0; i < n_iter; i++) {
            // note: in Update, I'm checking every frame for shift_up. That's why I check sprinting here
            if (!sprinting) break; // end coroutine if sprinting set to false elsewhere
            if (sprint > 0) sprint -= maxSprint / n_iter;
            else {
                sprinting = false;
                break; // end coroutine if out of sprint
            }
            yield return new WaitForSeconds(iter_time); // next iteration
        }

        //TODO: handle GUI changes in a separate GUI sprint routine, or in OnGUI()
        if (sprint < minReadySprint) {
            gui.sprintBar.GetComponent<Image>().color = Color.red;
            gui.minSprintReadyBar.color = Color.green;
        }
        else {
            gui.sprintBar.GetComponent<Image>().color = Color.green;
            gui.minSprintReadyBar.color = Color.white;
        }

        sprinting = false;

        //runSpeed = 5f;
        //StartCoroutine(sprint_cooldown()); // cooldown and recharge happen simultaneously
        StartCoroutine(sprint_recharge_routine());        
    }//sprint_routine
	
	// Update is called once per frame
	void Update () {
        w = Input.GetKey(KeyCode.W);
        a = Input.GetKey(KeyCode.A);
        s = Input.GetKey(KeyCode.S);
        d = Input.GetKey(KeyCode.D);

        moving = (w || a || s || d);

        space = Input.GetKeyDown(KeyCode.Space);

        shift = Input.GetKey(KeyCode.LeftShift);
        shift_down = Input.GetKeyDown(KeyCode.LeftShift);
        shift_up = Input.GetKeyUp(KeyCode.LeftShift);

        mouseY = -Input.GetAxis("Mouse Y");
        mouseX = Input.GetAxis("Mouse X");

	}// update

    private void LateUpdate() {

        rotY += mouseX * mouseSensitivity * Time.deltaTime;
        rotX += mouseY * mouseSensitivity * Time.deltaTime;

        // limit rotation about the X axis (up and down)
        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        // only transform player capsule about Y-axis
        Quaternion localRot = Quaternion.Euler(0f, rotY, 0f);
        transform.rotation = localRot;

        // camera gets rotated about X and Y axes
        Quaternion cameraRot = Quaternion.Euler(rotX, rotY, 0f);
        playerCamera.parent.transform.rotation = cameraRot;
    }// LateUpdate


    private void FixedUpdate() {

        if (w && !s) { // forward
            float displacement = runSpeed * Time.deltaTime;
            transform.Translate(0, 0, displacement);
        }
        else if (s && !w) { // backward
            float displacement = -1 * runSpeed * Time.deltaTime;
            transform.Translate(0, 0, displacement);
        }

        if (a && !d) { // left
            float displacement = -1 * runSpeed * Time.deltaTime;
            transform.Translate(displacement, 0, 0);
        }
        else if (d && !a) { // right
            float displacement = runSpeed * Time.deltaTime;
            transform.Translate(displacement, 0, 0);
        }

        // jump
        if (space && grounded) {
            jumping = true;
            rb.velocity = Vector3.up * jumpVelocity;
            // add boost to speed if sprinting
            if (sprinting) {
                //maxRunSpeed += 2f;
            }
        }

        // sprint
        if ((shift_down || shift) && sprintReady && moving  && !sprinting && !sprintCooldown) {
            StopCoroutine(sprint_recharge_routine());
            StartCoroutine(sprint_routine());
        }
        // stop sprinting
        if (shift_up && sprinting) {
            //StopCoroutine(sprint_routine());
            sprinting = false;
            //StartCoroutine(sprint_recharge_routine());
        }


        // fall faster than the trip up
        float vel_y = rb.velocity.y;
        if ((vel_y < 0 ||vel_y > 8) && vel_y > -maxSpeed) {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        if (sprinting) {
            if (runSpeed < maxRunSpeed) runSpeed += accell * Time.deltaTime;
            else runSpeed = maxRunSpeed;
        }
        else {
            if (runSpeed > baseRunSpeed) runSpeed += decell * Time.deltaTime;
            else runSpeed = baseRunSpeed;
        }
    }// FixedUpdate
}
