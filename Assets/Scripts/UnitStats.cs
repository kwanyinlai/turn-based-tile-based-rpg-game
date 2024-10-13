using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    public string charName;
    public int mvt;
    public bool hasActed;
    public int maxHP;
    public int currentHP;
    public int atk;
    public int spd;
    public int def;
    public int res;
    public bool attacksFirst;
    public string type;

    public void Kill()
    {

        
        GameObject.Find(charName).SetActive(false);
        int index = -1;
        bool enemy = false;
        for (int i = 0; i < CharacterManager.enemyNames.Count; i++){
            if (charName == CharacterManager.enemyNames[i]) 
            {
                index = i;
                enemy = true;
            }
        }

        if (index == -1)
        {
            for (int i = 0; i < CharacterManager.playerNames.Count; i++)
            {
                if (charName == CharacterManager.playerNames[i])
                {
                    index = i;
                }
            }

        }

        if (enemy == true)
        {
            CharacterManager.enemyNames.RemoveAt(index);
            CharacterManager.enemyPositions.RemoveAt(index);
        }
        else
        {
            CharacterManager.playerNames.RemoveAt(index);
            CharacterManager.playerPositions.RemoveAt(index);
            CharacterManager.playerNames.TrimExcess();

        }

    }

    public int showBST()
    {
        return maxHP + atk + spd + def + res;
    }


}
