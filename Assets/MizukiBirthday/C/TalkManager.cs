using System;
using System.Collections.Generic;

[Serializable]
public class DialogueFile
{
    public List<DialogueLine> dialogues;
}

[Serializable]
public class DialogueLine
{
    public string background;

    public CharacterSlot ch01;
    public CharacterSlot ch02;
    public CharacterSlot ch03;

    public string name;
    public string talk;

    // 對話音效 ID
    // 沒有音效時可以不寫
    public string sound;
	
	public string bgm;
}

[Serializable]
public class CharacterSlot
{
    public string id;
    public bool focus;
}