using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Player's inventory. Associated with a player object. Holds our currently equipped item
public class Inventory : MonoBehaviour
{
    Item current_item;
    List<Item> all_items = new List<Item>();

	void Start ()
    {
	
	}
	

	void Update ()
    {
	
	}


    public void AcquiredItem()
    {

    }


    public void SwitchItem()
    {
        if (all_items.Count == 1)
        {
            // Set our only item as selected
            current_item = all_items[0];
        }
        else if (all_items.Count <= 0)
        {
            // No items
            return;
        }
        else
        {
            // We have multiple items, find the next one
            int cur_index = all_items.IndexOf(current_item);
            cur_index++;

            if (cur_index >= all_items.Count)
            {
                // Need to loop back to beginning of list
                cur_index = 0;
            }

            current_item = all_items[cur_index];
        }

        // Select our newly navigated to item
        SelectedItem(current_item);
    }
    // Updates the UI to reflect our current item
    public void SelectedItem(Item item)
    {

    }


    // Uses the currently selected item
    public void UseItem()
    {

    }
}


public class Item
{
    string display_name;
    Sprite UI_image;    // Image used to show the equipped item
}