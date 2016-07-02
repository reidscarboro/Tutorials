using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIController : MonoBehaviour {

    //using the "singleton" pattern
    private static UIController instance;

    public Text healthText;
    public List<Button> abilityButtons;

    private Character selectedCharacter;
    private List<Ability> selectedCharacterAbilities;

    void Start () {
        instance = this;
    }
	
	public static void SelectCharacter(Character character) {
        instance.selectedCharacter = character;
        instance.healthText.text = character.name + " health: " + character.GetHealth().ToString();
        instance.selectedCharacterAbilities = new List<Ability>();
        for (int i = 0; i < character.abilities.Count; i++) {
            instance.selectedCharacterAbilities.Add(character.abilities[i]);
            if (i < instance.abilityButtons.Count) {
                instance.abilityButtons[i].image.sprite = character.abilities[i].abilityIcon;
            }
        }
    }

    public void CallAbility(int abilityIndex) {
        if (selectedCharacter != null) {
            selectedCharacterAbilities[abilityIndex].Trigger(selectedCharacter);
        } else {
            Debug.Log("Please select a character first");
        }
        
    }
}
