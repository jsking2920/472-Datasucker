using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class Response
{
    public string Line;
    public int Next;
    public List<string> Requires; // The progress conditions required for this option to appear
}

[System.Serializable]
public class DialogueLine
{
    public string Line;
    public List<Response> Responses;
    public List<String> Unlocks; // Progress conditions unlocked when read
}

[System.Serializable]
public class StartOption
{
    public int Index;
    public List<string> Conditions;
}

[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/Dialogue", order = 1)]
public class DialogueScript : ScriptableObject
{
    public List<StartOption> Starts;
    public List<DialogueLine> Lines;

    public bool HasVoice;

    private int _currentLineIndex;

    public void Initialize()
    {
        _currentLineIndex = FindStartLine();
        Debug.Log(_currentLineIndex);
    }

    private int FindStartLine() 
    {
        if (Starts.Count == 0) return 0;
        foreach (var line in Starts) 
        {
            bool met = true;
            foreach (var condition in line.Conditions)
            {
                met &= PlayerManager.Instance.CheckProgress(condition); // Checks each condition for possible starting line
                if (!met) break;
            }
            if (met) 
            {
                return line.Index;
            }
        }
        return Starts[Starts.Count - 1].Index; // Last item should have no condition anyway but yk. Possibly return -1 here instead and prevent talking if no options.
    }

    public List<string> Read()
    {
        Debug.Log(_currentLineIndex);
        DialogueLine currentDialogueLine = Lines[_currentLineIndex];
        // Now update progression
        foreach (var i in currentDialogueLine.Unlocks)
        {
            PlayerManager.Instance.UpdateProgress(i, true);
            Debug.Log("Updated progress");
        }

        List<string> outLines = new List<string>();
        outLines.Add(currentDialogueLine.Line);

        if (currentDialogueLine.Responses.Count > 0)
        {
            foreach (var response in currentDialogueLine.Responses)
            {
                bool conditionsMet = true;
                foreach (var condition in response.Requires)
                {
                    conditionsMet &= PlayerManager.Instance.CheckProgress(condition);
                    if (!conditionsMet) break;
                }
                if (conditionsMet)
                {
                    outLines.Add(response.Line);
                }
            }
        }

        return outLines;
    }

    public bool Respond(int responseIndex = 0) //bool so that the dialogue panel knows when to close
    {
        if (Lines[_currentLineIndex].Responses.Count == 0)
        {
            _currentLineIndex += 1;
        }
        else
        {
            int next = Lines[_currentLineIndex].Responses[responseIndex].Next;
            if (_currentLineIndex == next)
            {
                _currentLineIndex += 1;
            }
            else
            {
                _currentLineIndex = next;
            }
        }

        return _currentLineIndex >= 0 && _currentLineIndex < Lines.Count;
    }
}
