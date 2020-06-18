using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI roundGoalText;

    private RoundController _roundControllerController;

    private void Start()
    {
        _roundControllerController = FindObjectOfType<RoundController>();
    }

    private void Update()
    {
        scoreText.text = _roundControllerController.GetScore().ToString();
        roundGoalText.text = _roundControllerController.GetRoundGoal().ToString();
    }
}
