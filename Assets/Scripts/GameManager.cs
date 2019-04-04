using UnityEngine;

public class GameManager : MonoBehaviour {
    static public GameManager instance;
	static public string version = "V.2.0";

    private void Awake() 
    {
        if(instance == null)
		{
			DontDestroyOnLoad(gameObject);
			instance = this;
            var dio = Instantiate( Resources.Load<GameObject>("Discord") );
            DontDestroyOnLoad(dio);
			Debug.Log($"<color=green>nou! {version}</color>");
		}
		else
		{
			Destroy(gameObject);
		}
    }
}