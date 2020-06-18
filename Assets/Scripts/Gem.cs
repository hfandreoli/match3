using UnityEngine;

public class Gem : MonoBehaviour
{
    public delegate void ReachedDestination(Gem gem);
    public event ReachedDestination OnReachedDestination;
    
    private GridController _gridController;

    private Vector2 _targetPosition;
    private float _speed;
    private bool _moving;
    
    private void Start()
    {
        _gridController = FindObjectOfType<GridController>();
    }

    private void Update()
    {
        Move();
    }

    private void OnMouseDown()
    {
        if ( !_moving ) {
            _gridController.SelectGemWithPosition(transform.position);
        }
    }

    private void OnMouseUp()
    {
        _gridController.ClearGemSelection();
    }

    private void OnMouseOver()
    {
        _gridController.MouseOverGemWithPosition(transform.position);
    }

    public void Clear()
    {
        Destroy(gameObject);
    }

    public void SetPosition(Vector2 position, float speed)
    {
        _targetPosition = position;
        _speed = speed;
        _moving = true;
    }

    private void Move()
    {
        if ( _moving ) {
            transform.Translate((_targetPosition - (Vector2)transform.position).normalized * (Time.deltaTime * _speed));
            if ( Vector2.Distance(_targetPosition, transform.position) < 0.1) {
                transform.position = _targetPosition;
                _moving = false;
                OnReachedDestination?.Invoke(this);
            }
        }
    }
}
