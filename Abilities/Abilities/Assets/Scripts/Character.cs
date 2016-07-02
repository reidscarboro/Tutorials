using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour {

    public string name;
    public int maxHealth = 100;
    public List<Ability> abilities;

    private int health;

	void Start () {
        //for now just give each character a random amount of health so we can see that its working
        health = Random.Range(0, maxHealth);
    }
	
	void Update () {
	    if (Input.GetMouseButtonDown(0)) {
            if (Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.position) < 1) {
                UIController.SelectCharacter(this);
            }
        }
	}

    public int GetHealth() {
        return health;
    }
}
