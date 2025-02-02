//
//  game.cs
//  Game
//  Created by Ingenuity i/o on 2025/02/02
//
// no description
//  Copyright © 2023 Ingenuity i/o. All rights reserved.
//
using Ingescape;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Game
{
    // Singleton
	private static Game _instance;
    private Game () { }
	public static Game GetInstance () {
		if (_instance == null) {
			_instance = new Game ();
		}
		return _instance;
	}


	//inputs
	private int _DirectionI;
    public int DirectionI
    {

        get { return _DirectionI; }

        set
        {
            if (_DirectionI != value)
                _DirectionI = value;

            Debug.Log(string.Format("'direction' int value changed. New value : {0}", value.ToString()));
            //add code here if needed
        }
    }


    //outputs
    public void SetCoin_CollectedO()
    {
        //add code here if needed
        Igs.OutputSetImpulsion("coin_collected");
    }
    public void SetGhost_KilledO()
    {
        //add code here if needed
        Igs.OutputSetImpulsion("ghost_killed");
    }
    public void SetGame_WonO()
    {
        //add code here if needed
        Igs.OutputSetImpulsion("game_won");
    }
    public void SetGame_LostO()
    {
        //add code here if needed
        Igs.OutputSetImpulsion("game_lost");
    }

}
