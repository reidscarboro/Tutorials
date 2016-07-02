using UnityEngine;
using System.Collections;

public class Ability : MonoBehaviour {

    public Sprite abilityIcon;
    public string abilityName;

	public void Trigger(Character character) {
        Debug.Log("Ability \"" + abilityName + "\" called for character \"" + character.name + "\"");
    }
}
