using UnityEngine;

public class RoundCounter : MonoBehaviour
{
    private int _currentRound;
    
    private void Awake()
    {
        var numObjects = FindObjectsOfType<RoundController>().Length;
        if ( numObjects > 1 ) {
            Destroy(gameObject);
        }
        else {
            _currentRound = 1;
            DontDestroyOnLoad(gameObject);
        }
    }

    public int GetRound()
    {
        return _currentRound;
    }

    public void NextRound()
    {
        _currentRound++;
    }
}
