using UnityEngine;

namespace ScriptableObjects
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class BallSpawner : MonoBehaviour
    {
        [SerializeField] private GameSettings _settings;

        private CircleCollider2D _circle;
        
        private void Start()
        {
            _circle = GetComponent<CircleCollider2D>();
            
            for (var i = 0; i < _settings.Balls; i++)
            {
                Instantiate(_settings.Ball.BallPrefab, Random.insideUnitCircle * _circle.radius, Quaternion.identity);
            }
        }
    }
}