using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UserInput : MonoBehaviour
{
    public GameObject slot1;
    private Solitaire solitaire;

    private float timer;
    private float doubleClickTime = 0.3f;
    private int clickCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        solitaire = FindObjectOfType<Solitaire>();
        slot1 = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (clickCount == 1) {
            timer += Time.deltaTime;
        }
        if (clickCount == 3) {
            timer = 0;
            clickCount = 1;
        }
        if (timer > doubleClickTime) {
            timer = 0;
            clickCount = 0;
        }

        GetMouseClick();
    }

    void GetMouseClick() {
        if (Input.GetMouseButtonDown(0)) {
            clickCount++;
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
                    Top(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Bottom")) {
                    Bottom(hit.collider.gameObject);
                }
            }
        }
    }

    // Used to avoid null reference when resetting while a card is highlighted
    public void SetGameObject() {
        slot1 = this.gameObject;
    }

    void Deck() {
        print("Clicked on Deck");
        solitaire.DealFromDeck();
        slot1 = this.gameObject;
    }

    void Card(GameObject selected) {
        print("Clicked on Card");

        //if the card clicked on is facedown
        if (!selected.GetComponent<Selectable>().faceUp) {
             // if card clicked on is not blocked
                //flip it over
            if (!Blocked(selected)) {
                selected.GetComponent<Selectable>().faceUp = true;
                slot1 = this.gameObject;
            }
        }
        //if the card clicked on is in the deck pile with the trips 
        else if (selected.GetComponent<Selectable>().inDeckPile) {
            //if it is not blocked
            if (!Blocked(selected)) {
                if (slot1 == selected) {
                    if (DoubleClick()) {
                        // attempt auto stack
                        AutoStack(selected);
                    }
                }
                else {
                    slot1 = selected;
                }

               
            }
        }
        else {
        

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


            // if the same card is clicked twice, at bottomPos
            else if (slot1 == selected) {
                if (DoubleClick()) {
                    // attempt to auto stack
                    AutoStack(selected);
                }
            }
        }
    }

    void Top(GameObject selected) {
        print("Clicked on Top");
        if (slot1.CompareTag("Card")) {
            //if the card is an ace and the empty slot is top, then stack
            if (slot1.GetComponent<Selectable>().value == 1) {
                Stack(selected);
            }
        }
    }

    void Bottom(GameObject selected) {
        print("Clicked on Bottom");
        if (slot1.CompareTag("Card")) {
            //if the card is a king and the empty slot is bottom, then stack
            if (slot1.GetComponent<Selectable>().value == 13) {
                Stack(selected);
            }
        }
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

    bool Blocked(GameObject selected) {
        Selectable s2 = selected.GetComponent<Selectable>();
        if (s2.inDeckPile == true) {            
            if (s2.name == solitaire.tripsOnDisplay.Last()) {
                return false;
            }
            else {
                print(s2.name + " is blocked by " + solitaire.tripsOnDisplay.Last());
                return true;
            }
        }
        else {
            //check if it is the bottom card
            if (s2.name == solitaire.bottoms[s2.row].Last()) {
                return false;
            }
            else {
                return true;
            }
        }
    }
    bool DoubleClick() {
        if (timer < doubleClickTime && clickCount == 2) {
            print("Double Click");
            return true;
        }
        else {
            return false;
        }
    }

    void AutoStack(GameObject selected) {
        for (int i = 0; i < solitaire.topPos.Length; i++) {
            Selectable stack = solitaire.topPos[i].GetComponent<Selectable>();
            //if it is an ace
            if (selected.GetComponent<Selectable>().value == 1) {
                //and the top position is empty
                if (solitaire.topPos[i].GetComponent<Selectable>().value == 0) {
                    slot1 = selected;
                    //stack the ace up top
                    Stack(stack.gameObject);
                    //in the first empty position found
                    break;
                }
            }
            else {
                if ((solitaire.topPos[i].GetComponent<Selectable>().suit == slot1.GetComponent<Selectable>().suit) && (solitaire.topPos[i].GetComponent<Selectable>().value == slot1.GetComponent<Selectable>().value - 1)) {
                    //if it is the last card (if it has no children) 
                    if (HasNoChildren(slot1)) {
                        
                        slot1 = selected;
                        //find a top spot that matches the conditions for auto stacking if it exists
                        string lastCardname = stack.suit + stack.value.ToString();
                        if (stack.value == 1) {
                            lastCardname = stack.suit + "A";
                        }
                        if (stack.value == 11) {
                            lastCardname = stack.suit + "J";
                        }
                        if (stack.value == 12) {
                            lastCardname = stack.suit + "Q";
                        }
                        if (stack.value == 13) {
                            lastCardname = stack.suit + "K";
                        }
                        GameObject lastCard = GameObject.Find(lastCardname);
                        Stack(lastCard);
                        break;
                    
                    }
                }
            }
        }
    }

    bool HasNoChildren(GameObject card) {
        int i = 0;
        foreach (Transform child in card.transform) {
            i++;
        }
        if (i == 0) {
            return true;
        }
        else {
            return false;
        }
    }
}
