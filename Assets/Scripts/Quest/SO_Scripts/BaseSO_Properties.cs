using UnityEngine;

public abstract class BaseSO_Properties : ScriptableObject
{
    public string questName;
    public string questDescription;
    public int id;

    public bool isCompleted;

    public abstract void Init();

    public abstract void CheckProgress();
}