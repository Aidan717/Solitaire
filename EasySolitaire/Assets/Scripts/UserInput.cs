using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    public GameObject slot1;
    private Solitaire solitaire;
    // Start is called before the first frame update
    void Start()
    {
        solitaire = FindObjectOfType<Solitaire>();
        slot1 = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        GetMouseClick();
    }

    void GetMouseClick() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit) {
                //what has been hit?
                if(hit.collider.CompareTag("Deck")) {
                    Deck();
                }
                else if (hit.collider.CompareTag("Card")) {
                    Card(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Top")){
                    Top();
                }
                else if (hit.collider.CompareTag("Bottom")) {
                    Bottom();
                }
            }
        }
    }

    void Deck() {
        print("Clicked on Deck");
        solitaire.DealFromDeck();
    }

    void Card(GameObject selected) {
        print("Clicked on Card");

        //if the card clicked on is facedown
        if (!selected.GetComponent<Selectable>().faceUp) {
             // if card clicked on is not blocked
                //flip it over
                selected.GetComponent<Selectable>().faceUp = true;
                slot1 = this.gameObject;
        }
           

        //if the card clicked on is in the deck pile with the trips
            //if the card is not blocked
                //select it

        //if the card is face up
            //if there's no card currently selected
                //select the card

        //not null because we pass in this gameObject instead
        if (slot1 == this.gameObject) {
            slot1 = selected;
        }
            
        else if ( slot1 != selected) {        
            //if there is already card selected (and not the same card)
            if (Stackable(selected)) { 
                Stack(selected);
            }
            else {
                //select the new card
                slot1 = selected;
            }
        }   


            //else if there is already a card selected and it is the same card
                //if the time is short enough between clicks, it's a double click
                    //if the card is eligable to fly up to top, then do it
    }

    void Top() {
        print("Clicked on Top");
    }

    void Bottom() {
        print("Clicked on Bottom");
    }

    bool Stackable(GameObject selected) {
        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();
        //compare if they stack

        if (!s2.inDeckPile) {
            
            // if in the top pile must stack suited Ace to King
            if (s2.top) {
                if (s1.suit == s2.suit || (s1.value == 1 && s2.suit == null)) {
                    if (s1.value == s2.value + 1) {
                        return true;
                    }
                }
                else {
                        return false;
                    }
            }

            //if in the bottom pile must stack alternate colours King to Ace
            else {
                if (s1.value == s2.value -1) {
                    bool card1Red = true;
                    bool card2Red = true;

                    if (s1.suit == "C" || s1.suit == "S") {
                        card1Red = false;
                    }

                    if (s2.suit == "C" || s2.suit == "S") {
                        card2Red = false;
                    }

                    if (card1Red == card2Red) {
                        print ("Not Stackable");
                        return false;
                    }
                    else {
                        print("Stackable");
                        return true;
                    }
                }
            }
        }
        return false;
    }


    void Stack(GameObject selected) {
        //if on top of king or empty bottom stack the cards in place
        //else stack the cards with a negativ y offset

        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();
        float yOffset = 0.3f;

        if (s2.top || !s2.top && s1.value == 13) {
            yOffset = 0;
        }
        
        slot1.transform.position = new Vector3(selected.transform.position.x, selected.transform.position.y - yOffset, selected.transform.position.z - 0.01f);
        slot1.transform.parent = selected.transform; //this makes the children move with the parents

        //removes the cards from the top pile to prevent duplicate cards
        if (s1.inDeckPile) { 
            solitaire.tripsOnDisplay.Remove(slot1.name);
        }
        //allows movement of cards between top spots 
        else if (s1.top && s2.top && s1.value == 1) {
            solitaire.topPos[s1.row].GetComponent<Selectable>().value = 0;
            solitaire.topPos[s1.row].GetComponent<Selectable>().suit = null;
        }
        //keeps track of the current value of the top decks as a card has been removed.
        else if (s1.top) {
            solitaire.topPos[s1.row].GetComponent<Selectable>().value = s1.value -1;
        }
        //removes the card string from the appropriate bottom list
        else {
            solitaire.bottoms[s1.row].Remove(slot1.name);
        }
        
        //cannot add cards to the trips pile so this is always fine
        s1.inDeckPile = false;
        s1.row = s2.row;

        if (s2.top) {
            solitaire.topPos[s1.row].GetComponent<Selectable>().value = s1.value;
            solitaire.topPos[s1.row].GetComponent<Selectable>().suit = s1.suit;
            s1.top = true;
        }
        else {
            s1.top = false;
        }

        //after completing move reset slot1 to be essentially null as being null will break the logic
        slot1 = this.gameObject;
    }

}
