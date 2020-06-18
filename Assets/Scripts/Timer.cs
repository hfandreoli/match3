using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private float roundDuration;

    private Slider _slider;
    private RoundController _roundController;

    private float _elapsedTime;
    private bool _stopped;

    private void Start()
    {
        _slider = GetComponent<Slider>();
        _roundController = FindObjectOfType<RoundController>();
    }

    private void Update()
    {
        if ( !_stopped ) {
            _elapsedTime += Time.deltaTime;
            _slider.value = (roundDuration - _elapsedTime) / roundDuration;

            if ( _elapsedTime > roundDuration ) {
                _roundController.TimesUp();
            }
        }
    }

    public void Stop()
    {
        _stopped = true;
    }
}