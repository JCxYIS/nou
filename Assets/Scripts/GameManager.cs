using UnityEngine;

public class GameManager : MonoBehaviour {
    static public GameManager instance;

    private void Awake() 
    {
        if(instance == null)
		{
			DontDestroyOnLoad(gameObject);
			instance = this;
            var dio = Instantiate( Resources.Load<GameObject>("Discord") );
            DontDestroyOnLoad(dio);
		}
		else
		{
			Destroy(gameObject);
		}
    }
}