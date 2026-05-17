using DQHieu.Framework;
using UnityEngine;

public struct LevelComplete : IGameEvent
{
    public int overallScore;

    public LevelComplete(int overallScore)
    {
        this.overallScore = overallScore;
    }
}
