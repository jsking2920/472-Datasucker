using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Response
{
    public string Line;
    public int Next;
}

[System.Serializable]
public class DialogueLine
{
    public string Line;
    public List<Response> Responses;
    public int[] NewStart; // Use array just so we don't need to update start dialogue all the time (annoying to set up), and you can randomize it.
    public bool Final;
}

[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/Dialogue", order = 1)]
public class DialogueScript : ScriptableObject
{
    public int StartLine;
    public List<DialogueLine> Lines;

    private int _currentStartLine;
    private int _currentLine;

    private void Awake()
    {
        _currentStartLine = StartLine;
    }

    public void Activate()
    {
        _currentLine = _currentStartLine;
    }

    public string Read()
    {
        int startOptionCount = Lines[_currentLine].NewStart.Length;
        if (startOptionCount > 0)
        {
            int newStartIndex = Random.Range(0, startOptionCount);
            _currentStartLine = Lines[_currentLine].NewStart[newStartIndex];
        }
        // Oof these names might need some work
        DialogueLine currentDialogueLine = Lines[_currentLine];
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
                responses = string.Concat(responses, "\n<link=", index, "><i>\t", response.Line, "</i></link>");
                index++;
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
