using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Util;

public class FMS : MonoBehaviour
{
    public int matchTime = 160;
    public int autoTime = 20;
    public float autoDisableTime = 0.5f;
    public int endgameTime = 30;
    public float matchDisabledTime = 3;
    public static float MatchTimer, ShownTimer;
    public static RobotState RobotState;
    public static MatchState MatchState;
    public MatchState state;
    
    private MatchState previousMatchState;

    private LoadMatch matchLoader;
    private TextMeshProUGUI timer;
    private TextMeshProUGUI textStatus;

    private ScoreOnlyOnce scorerBlue;
    private ScoreOnlyOnce scorerRed;

    public RobotState robotState;
    // Start is called before the first frame update
    void OnEnable()
    {
        Restart();

        //kevin: get scorers for enabling/disabling
        scorerBlue = GameObject.Find("BlueScoreAdder").GetComponent<ScoreOnlyOnce>();
        scorerRed  = GameObject.Find("RedScoreAdder").GetComponent<ScoreOnlyOnce>();
    }

    // Update is called once per frame
    void Update()
    {
        state = MatchState;
        robotState = RobotState;
        MatchTimer -= Time.deltaTime;

        if (MatchTimer >= matchTime - autoTime)
        {
            MatchState = MatchState.auto;
        }  else if (MatchTimer >= endgameTime)
        {
            MatchState = MatchState.teleop;
        }
        else if (MatchTimer < 0)
        {
            MatchState = MatchState.finished;
        } else if (MatchTimer <= endgameTime)
        {
            MatchState = MatchState.endgame;
        }
        
        if (MatchState != previousMatchState && MatchState != MatchState.endgame)
        {
            switch (MatchState)
            {
                case MatchState.teleop:
                    MatchState = MatchState.auto;
                    StartCoroutine(wait(autoDisableTime));
                    MatchState = MatchState.teleop;
                    break;
                case MatchState.finished:
                    MatchState = MatchState.endgame;
                    StartCoroutine(wait(matchDisabledTime));
                    MatchState = MatchState.finished;
                    break;
            }
        }
        
        previousMatchState = MatchState;

        if (MatchState == MatchState.auto)
        {
            ShownTimer = MatchTimer - (matchTime - autoTime);   //AUTO TIME
        }
        else {
            ShownTimer = MatchTimer;
        }

        float minutes = Mathf.FloorToInt(ShownTimer / 60); 
        float seconds = Mathf.FloorToInt(ShownTimer % 60);
        
        if (minutes < 0) minutes = 0;
        if (seconds < 0) seconds = 0;

        if (timer != null)
        {
            timer.text = $"{minutes:00}:{seconds:00}";
        }
    
    
        if( MatchTimer > 140)   //AUTO
        {
            //kevin: use FloorToInt to fix floating point display issue
            float ss = Mathf.FloorToInt(MatchTimer - 140);
            textStatus.text = $"AUTO {ss:00}";
        } 
        else if (MatchTimer > 130) //TRANS
        {
            float ss = Mathf.FloorToInt(MatchTimer - 130);
            textStatus.text = $"TRANS {ss:00}";
        }       
        else if (MatchTimer > 105) //SHIFT-1
        {
            float ss = Mathf.FloorToInt(MatchTimer - 105);
            textStatus.text = $"S1-RED {ss:00}";
            scorerRed.EnableScoring();
            scorerBlue.DisableScoring();
        }
        else if(MatchTimer > 80) //SHIFT-2
        {
            float ss = Mathf.FloorToInt(MatchTimer - 80);
            textStatus.text = $"S2-BLUE {ss:00}";
            scorerRed.DisableScoring();
            scorerBlue.EnableScoring();
        }
        else if(MatchTimer > 55) //SHIFT-3
        {
            float ss = Mathf.FloorToInt(MatchTimer - 55);
            textStatus.text = $"S3-RED {ss:00}";
            scorerRed.EnableScoring();
            scorerBlue.DisableScoring();
        }
        else if(MatchTimer > 30) //SHIFT-4
        {
            float ss = Mathf.FloorToInt(MatchTimer - 30);
            textStatus.text = $"S4-BLUE {ss:00}";
            scorerRed.DisableScoring();
            scorerBlue.EnableScoring();
        }
        else if(MatchTimer > 0) //END
        {
            float ss = MatchTimer;
            textStatus.text = $"END {ss:00}";
            scorerRed.EnableScoring();
            scorerBlue.EnableScoring();
        }
    }

    private IEnumerator wait(float time)
    {
        RobotState = RobotState.disabled;
        yield return new WaitForSeconds(time);
        RobotState = RobotState.enabled;
    }

    public void Restart()
    {
        var dispT = GameObject.Find("TimerDisplay");
        var dispS = GameObject.Find("StatusDisplay");
        if (dispT != null)
        {
            timer = dispT.GetComponent<TextMeshProUGUI>();
        }

        if (dispS != null)
        {
            textStatus = dispS.GetComponent<TextMeshProUGUI>();
        }


        matchLoader = Utils.FindParentObjectComponent<LoadMatch>(gameObject);
        matchLoader.setFMS(this);
        MatchTimer = matchTime;
        previousMatchState = MatchState.auto;
        MatchState = MatchState.auto;
        RobotState = RobotState.enabled;
    }
}

[Serializable]
public enum RobotState
{
    enabled,
    disabled,
}

[Serializable]
public enum MatchState
{
    auto,
    teleop,
    endgame,
    finished
}