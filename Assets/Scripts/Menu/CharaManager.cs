using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharaManager : MonoBehaviour
{
    public static CharaManager instance;
    /// <summary>
    ///  玩家所持有chara，AB=0~1,依據圖鑑裡能力值來乘法
    /// </summary>
    [System.Serializable]
    public class Chara
    {
        public int rare;
        public int ID;
        public CharaSkill[] skills;
        public float hpAB;
        public float healAB;
        public float defAB;
        public float hp { get { return GetValue(instance.charaDatas[ID].hpRange, hpAB); } }
        public float heal { get { return GetValue(instance.charaDatas[ID].healRange, healAB); } }
        public float def { get { return GetValue(instance.charaDatas[ID].defRange, defAB); } }

        float GetValue(Vector2 range, float AB)
        {
            float min = range.x, max = range.y;
            return min + (max - min) * AB;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class CharaSkill
    {
        public int skillId;
        public float[] paras;
    }

    /// <summary>
    /// (內部)基值
    /// </summary>
    [System.Serializable]
    public class CharaManifest
    {
        public List<CharaSkill> skill;
        public Vector2 hpRange;
        public Vector2 healRange;
        public Vector2 defRange;
    }

    /// <summary>
    /// (內部)圖鑑
    /// </summary>
    [SerializeField] List<CharaManifest> charaDatas;


    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 執行skill
    /// </summary>
    /// <param name="skill">skill wanna execute</param>
    /// <param name="doExecute">真的要執行嗎? false:只獲取string</param>
    /// <returns></returns>
    public string Execute(CharaSkill skill, bool doExecute = true)
    {
        string result = "";
        int i1, i2;
        float f1, f2;

        switch(skill.skillId)
        {
            case 0:
                result = $"Every {skill.paras[0]}s, heal LP by {skill.paras[1]}";
                break;
        }
        return result;
    }
}
