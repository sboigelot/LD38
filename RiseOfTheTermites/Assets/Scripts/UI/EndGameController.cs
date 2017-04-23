using Assets.Scripts.Managers.DialogBoxes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Assets.Scripts.Controllers;
using Assets.Scripts.Managers;

public class EndGameController :  DialogBoxBase<EndGameController>
{
    public Button RetryButton;
    public Button NextLevelButton;
    public Text TitleText;
    public Text ScoreText;

    public bool GameIsSuccessful = true;

    protected override void OnDialogOpen()
    {
        if (GameIsSuccessful)
            TitleText.text = "Congratulations";
        else
            TitleText.text = "Game over";
            
        NextLevelButton.gameObject.SetActive( GameIsSuccessful );

        RetryButton.onClick.AddListener( () => {
            //TODO
            //GameController.Instance.NewGame(GameManager.Instance.CurrentLevel.Index);
        });
        NextLevelButton.onClick.AddListener(() => {
            //TODO
        });
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
