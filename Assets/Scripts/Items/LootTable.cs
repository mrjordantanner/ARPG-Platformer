using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootTable : ScriptableObject
{
    // LootTable = WeightedItemTables

    // Class to contain list of LootTableMembers (weighted items)
    // within which to roll for
    // Returns the selected LootTableMember

    public List<LootTableMember> weightedItems;
    public float probabilityTotalWeight;

    void Start()
    {
        weightedItems = new List<LootTableMember>();
        ValidateTable();
    }

    public LootTable(List<LootTableMember> _lootTableMembers)
    {
        weightedItems = _lootTableMembers;

    }

    /// <summary>
    /// Calculates the percentage and assigns the probabilities how many times
    /// the items can be picked. Function used also to validate data when tweaking numbers in editor.
    /// </summary> 
    public void ValidateTable()
    {
        if (weightedItems != null && weightedItems.Count > 0)
        {
            float currentProbabilityWeightMaximum = 0f;

            // Sets the weight ranges of the selected items
            foreach (var weightedItem in weightedItems)
            {
                if (weightedItem.probabilityWeight < 0f)
                    weightedItem.probabilityWeight = 0f;
                else
                {
                    weightedItem.probabilityRangeFrom = currentProbabilityWeightMaximum;
                    currentProbabilityWeightMaximum += weightedItem.probabilityWeight;
                    weightedItem.probabilityRangeTo = currentProbabilityWeightMaximum;
                }

            }

            probabilityTotalWeight = currentProbabilityWeightMaximum;

           // Calculate percentage of item drop select rate.
           // foreach (var weightedItem in weightedItems)
           // {
           //     weightedItem.probabilityPercent = ((weightedItem.probabilityWeight) / probabilityTotalWeight) * 100;
           // }

        }

    }


    public LootTableMember PickLootTableMember()
    {
        float pickedNumber = Random.Range(0, probabilityTotalWeight);

        // Find an item whose range contains pickedNumber
        foreach (var weightedItem in weightedItems)
        {
            // If the picked number matches the item's range, return item
            if (pickedNumber > weightedItem.probabilityRangeFrom && pickedNumber < weightedItem.probabilityRangeTo)
            {
                return weightedItem;
            }
        }

        return weightedItems[0];
    }

    


}


