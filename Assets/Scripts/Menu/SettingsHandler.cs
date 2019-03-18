using UnityEngine;
using UnityEngine.UI;

public class SettingsHandler : MonoBehaviour 
{
    public Dropdown skin;

    void Start() 
    {
        Userpref.instance.Load();
        skin.value = Userpref.instance.data.skinType;
    }
    void Update() 
    {
        Userpref.instance.data.skinType = skin.value;
    }

    
}