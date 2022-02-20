using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootTableMember : ScriptableObject
{
    // LootTableMember = Weighted Item
    // Basic member of a LootTable representing a weighted element
    // that can be rolled for, whether it's an 
    // attribute, an actual item, etc

    [HideInInspector]
    public int index;
    public float probabilityWeight;  
    [HideInInspector]
    public float probabilityRangeFrom;
    [HideInInspector]
    public float probabilityRangeTo;


    public LootTableMember(int _index, float _weight)
    {
        index = _index;
        probabilityWeight = _weight;

    }


}


