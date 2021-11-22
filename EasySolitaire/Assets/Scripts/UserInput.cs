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
            // if card clicked on is not blocked
                //flip it over

        //if the card clicked on is in the deck pile with the trips
            //if the card is not blocked
                //select it

        //if the card is face up
            //if there's no card currently selected
                //select the card

        //not null because we pass in this gameObject instead
        if (slot1 = this.gameObject) {
            slot1 = selected;
        }
            
            //if there is already card selected (and not the same card)
                //if the new card is eligable to stack on the old card
                    //stack it
                //else
                    //select the new card

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
}
