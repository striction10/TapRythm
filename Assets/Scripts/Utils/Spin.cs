using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField] private float _speed = 30f;
    
    void Update()
    {
        transform.Rotate(Vector3.forward, _speed * Time.deltaTime);
    }
}