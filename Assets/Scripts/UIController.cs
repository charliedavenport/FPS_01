using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public Text sprintText;
    public Text speedText;
    public RectTransform sprintBar;
    public Image minSprintReadyBar;

    public PlayerController player;

    public float maxBarWidth;
    private Vector2 offset;

	// Use this for initialization
	void Start () {
        sprintText.text = "Sprint :";
        maxBarWidth = sprintBar.GetWidth();
        offset = sprintBar.offsetMin;
    }
	
	void OnGUI () {
        speedText.text = "Speed: " + player.runSpeed; // for now, walkSpeed and strafeSpeed are the same.

        float x = sprintBar.rect.width;
        sprintBar.SetWidth(maxBarWidth * player.sprint);
        float x_new = sprintBar.rect.width;
        sprintBar.offsetMin = offset;



	}
}
