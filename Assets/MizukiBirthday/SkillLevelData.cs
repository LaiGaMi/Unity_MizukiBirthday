using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillLevelData
{
    public string skillName;
    [TextArea]
    public string description;
}

[System.Serializable]
public class SkillData
{
    public string skillID;
    public Sprite icon;

    // 0、1、2、3 級的資料
    public List<SkillLevelData> levels = new List<SkillLevelData>();
}