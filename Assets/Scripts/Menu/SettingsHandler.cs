using UnityEngine;
using UnityEngine.UI;

public class SettingsHandler : MonoBehaviour 
{
    public Dropdown skin;

    void Start() 
    {
        Userpref.Load();
        skin.value = Userpref.data.skinType;
    }
    void Update() 
    {
        Userpref.data.skinType = skin.value;
    }

    
}