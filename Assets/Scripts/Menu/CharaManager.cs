using UnityEngine;
using System.Collections;

public class CharaManager : MonoBehaviour
{
    /// <summary>
    ///  玩家所持有chara
    /// </summary>
    public class Chara
    {
        public int rare;
        public int typeID;
        public float hpAB;
        public float healAB;
        public float hpdrainAB;
    }

    /// <summary>
    /// (內部)基值
    /// </summary>
    [System.Serializable]
    public class CharaManifest
    {
        public int typeID;
        public string spriteName;
        public float hpBase, hpRange;
        public float healBase, healRange;
        public float hpdrainBase, hpdrainRange;
    }
    [SerializeField] CharaManifest charaData;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
