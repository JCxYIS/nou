using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SettingsHandler : MonoBehaviour 
{
    public Dropdown skin, calcScore;
    public Slider bgm, sfx;
    public InputField osuPath;

    void Start() 
    {
        Userpref.Load();
        skin.value = Userpref.data.skinType;
        bgm.value = Userpref.data.volumeBgm;
        sfx.value = Userpref.data.volumeSfx;
        osuPath.text = Userpref.data.customOsuPath;
        calcScore.value = Userpref.data.calcScoreMod;
    }
    void Update() 
    {
        Userpref.data.skinType = skin.value;
        Userpref.data.calcScoreMod = calcScore.value;
        Userpref.data.volumeBgm = bgm.value;
        Userpref.data.volumeSfx = sfx.value;
        Userpref.data.customOsuPath = osuPath.text;
    }


    public void CheckOsuPath(Text result)
    {
        int getSheet = 0;
        foreach (var item in Directory.GetDirectories(Userpref.data.customOsuPath))
        {
            Debug.Log("從osu獲取曲包 路徑: "+item);
            foreach(var f in Directory.GetFiles( item ) )
            {
                if(Path.GetExtension(f) == ".osu")
                {
                    getSheet++;
                    break;
                }
            }
        }
        if(getSheet == 0)
            result.text = $"未找到任何譜面";
        else
            result.text = $"已獲取{getSheet}個譜面！\n重新啟動以套用！";
    }

    
}