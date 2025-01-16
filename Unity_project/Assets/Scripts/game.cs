//
//  game.cs
//  Game
//  Created by Ingenuity i/o on 2025/01/08
//
// no description
//  Copyright © 2023 Ingenuity i/o. All rights reserved.
//
using Ingescape;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Game {
    //inputs
    int i = 0;
    public void SetBarI () {
        Debug.Log ("'bar'  changed (impulsion)");
        //add code here if needed
        GameObject.Find ("Debug").GetComponent<TMP_Text> ().text = "" + i;
        i++;
    }


    //outputs
    public void SetFooO () {
        //add code here if needed
        Igs.OutputSetImpulsion ("foo");
    }


    public Game () {
    }

}
