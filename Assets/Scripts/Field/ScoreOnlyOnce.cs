using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using System.Linq;

public class ScoreOnlyOnce : FieldScorer
{
    private HashSet<GameObject> scoredPieces = new HashSet<GameObject>();
    private HashSet<GameObject> previousOccupyObjects = new HashSet<GameObject>();
    private int totalScore = 0;

    //kevin: store original score for enabling/disabling
    private bool isEnabled = true;

    public void EnableScoring()
    {
        if (isEnabled) return;
        SetScoreToAdd(1);
        isEnabled = true;

        Debug.Log("ENABLE: " + string.Join("/",
              transform.GetComponentsInParent<Transform>(true)
              .Select(t => t.name)
              .Reverse()));
    }

    public void DisableScoring()
    {
        if (!isEnabled) return;
        SetScoreToAdd(0);
        isEnabled = false;

        Debug.Log("DISABLE: " + string.Join("/", 
              transform.GetComponentsInParent<Transform>(true)
              .Select(t => t.name)
              .Reverse()));
    }

    void FixedUpdate()
    {
        occupyObjects = occupyPieces();
        
        // Create a set of current objects for comparison
        HashSet<GameObject> currentObjects = new HashSet<GameObject>();
        foreach (var piece in occupyObjects)
        {
            currentObjects.Add(piece.gameObject);
            
            // Only score if this piece hasn't been scored yet
            if (!scoredPieces.Contains(piece.gameObject))
            {
                scoredPieces.Add(piece.gameObject);
                totalScore++; // Increment total score
            }
        }
        
        // Remove pieces that left the zone from scoredPieces
        scoredPieces.RemoveWhere(obj => !currentObjects.Contains(obj));
        
        ScorePoints(totalScore); // Pass the total accumulated score
    }
}