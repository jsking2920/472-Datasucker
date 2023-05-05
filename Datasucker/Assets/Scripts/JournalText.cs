using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JournalLine
{
    public string Line;
    public List<string> Requires;
}


[CreateAssetMenu(fileName = "JournalText", menuName = "ScriptableObjects/JournalText", order = 2)]
public class JournalText : ScriptableObject
{
    public List<JournalLine> Lines;

    public string Read()
    {
        string outString = "";

        foreach (JournalLine line in Lines)
        {
            if(Check(line))
            {
                outString += "-" + line.Line + "\n";
            }
        }

        return outString;
    }

    private bool Check(JournalLine line)
    {
        foreach (string req in line.Requires)
        {
            if ((req[0] == '!') == PlayerManager.Instance.CheckProgress(req.Substring(1)))
            {
                return false;
            }
        }
        return true;
    }
}
