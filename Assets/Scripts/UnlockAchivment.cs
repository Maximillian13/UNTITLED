using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;


public class UnlockAchivment : MonoBehaviour
{
	// 0 = Green, 1 = Blue, 2 = Red, 3 = Orange, 4 = Pink, 5 = Yellow, 6 = Light Blue,
	// 7 = Disconnected, 8 = Hidden Room, 9 = Black, 10 = White, 11 = Disconnected2
	public int achIndex;
    private Achievement_t[] m_Achievements = new Achievement_t[]
    {
        new Achievement_t(Achievement.ACH_GREEN_BOX, "#3ED80D", "..."),
        new Achievement_t(Achievement.ACH_BLUE_BOX, "#594AC9", "..."),
        new Achievement_t(Achievement.ACH_RED_BOX, "#DA1616", "..."),
        new Achievement_t(Achievement.ACH_ORANGE_BOX, "#E58B03", "..."),
        new Achievement_t(Achievement.ACH_PINK_BOX, "#E837C2", "..."),
        new Achievement_t(Achievement.ACH_YELLOW_BOX, "#F1FF00", "..."),
        new Achievement_t(Achievement.ACH_LIGHTBLUE_BOX, "#2AD7F0", "..."),
        new Achievement_t(Achievement.ACH_DISCONNECTED, "ID-41", "..."),
        new Achievement_t(Achievement.ACH_HIDDEN_ROOM, "22 : [404]", "..."),
		new Achievement_t(Achievement.ACH_BLACK_BOX, "#000000", "..."),
		new Achievement_t(Achievement.ACH_WHITE_BOX, "#FFFFFF", "..."),
		new Achievement_t(Achievement.ACH_DISCONNECTED2, "ID-41-2", "...")
	};

    // Use this for initialization
    void Start ()
    {
        if (SteamManager.Initialized == true)
            this.Unlock(m_Achievements[achIndex]);
	}
	
	private void Unlock(Achievement_t achievement)
    {
        achievement.m_bAchieved = true;

        // mark it down
        SteamUserStats.SetAchievement(achievement.m_eAchievementID.ToString());
        SteamUserStats.StoreStats();
    }

}

enum Achievement : int
{
    ACH_GREEN_BOX,
    ACH_BLUE_BOX,
    ACH_RED_BOX,
    ACH_ORANGE_BOX,
    ACH_PINK_BOX,
    ACH_YELLOW_BOX,
    ACH_LIGHTBLUE_BOX,
    ACH_DISCONNECTED,
    ACH_HIDDEN_ROOM,
	ACH_BLACK_BOX,
	ACH_WHITE_BOX,
	ACH_DISCONNECTED2
}

class Achievement_t
{
    public Achievement m_eAchievementID;
    public string m_strName;
    public string m_strDescription;
    public bool m_bAchieved;

    /// <summary>
    /// Creates an Achievement. You must also mirror the data provided here in https://partner.steamgames.com/apps/achievements/yourappid
    /// </summary>
    /// <param name="achievement">The "API Name Progress Stat" used to uniquely identify the achievement.</param>
    /// <param name="name">The "Display Name" that will be shown to players in game and on the Steam Community.</param>
    /// <param name="desc">The "Description" that will be shown to players in game and on the Steam Community.</param>
    public Achievement_t(Achievement achievementID, string name, string desc)
    {
        m_eAchievementID = achievementID;
        m_strName = name;
        m_strDescription = desc;
        m_bAchieved = false;
    }
}


