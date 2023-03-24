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
    public int[] Requires; // The indices in the progression manager needed for this option to appear
}

[System.Serializable]
public class DialogueLine
{
    public string Line;
    public List<Response> Responses;
    public int[] Unlocks; // Indices in progress manager unlocked when this dialogue is read
    public int[] NewStart; // Use array here just so we don't need to update start dialogue all the time (annoying to set up), and you can randomize it.
    public bool Final;
}

[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/Dialogue", order = 1)]
public class DialogueScript : ScriptableObject
{
    public int StartLine;
    public List<DialogueLine> Lines;

    private ProgressManager _progressManager;

    // Keep these public for dev purposes (annoying to reset otherwise, since data persists between runs)
    public int _currentStartLine;
    public int _currentLine;

    private void Awake()
    {
        _currentStartLine = StartLine;
        _currentLine = _currentStartLine;
    }

    public void Initialize()
    {
        _progressManager = ProgressManager.Instance;
        _currentLine = _currentStartLine;
    }

    public string Read()
    {
        // Update start dialogue line if needed
        int startOptionCount = Lines[_currentLine].NewStart.Length;
        if (startOptionCount > 0)
        {
            int newStartIndex = Random.Range(0, startOptionCount);
            _currentStartLine = Lines[_currentLine].NewStart[newStartIndex];
        }

        // Oof these names might need some work
        DialogueLine currentDialogueLine = Lines[_currentLine];
        // Now update progression
        foreach (var i in currentDialogueLine.Unlocks)
        {
            _progressManager.ProgList[i] = true;
            Debug.Log("Updated progress");
        }

        if (currentDialogueLine.Responses.Count < 0)
        {
            return currentDialogueLine.Line;
        }
        else
        {
            string responses = string.Concat(currentDialogueLine.Line, '\n');
            int index = 0;
            foreach (var response in currentDialogueLine.Responses)
            {
                bool conditionsMet = true;
                foreach (var i in response.Requires)
                {
                    conditionsMet &= _progressManager.ProgList[i];
                    if (!conditionsMet) break;
                }

                if (conditionsMet)
                {
                    responses = string.Concat(responses, "\n<link=", index, "><i>\t", response.Line, "</i></link>");
                    index++;
                }
            }

            return responses;
        }
    }

    public bool Respond(int index = 0)
    {
        if (Lines[_currentLine].Final)
        {
            return false;
        }
        if (Lines[_currentLine].Responses.Count == 0)
        {
            _currentLine += 1;
        }
        else
        {
            _currentLine = Lines[_currentLine].Responses[index].Next;
        }

        return _currentLine < Lines.Count;
    }
}
