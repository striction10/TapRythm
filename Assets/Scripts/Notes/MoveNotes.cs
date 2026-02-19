using UnityEngine;
using UnityEngine.InputSystem;

public class MoveNotes : MonoBehaviour
{
    [Header("Settings")]
    public float speedZ = 2f;
    public float startZ = 10f;
    public float targetZ = -5f;

    private bool hasBeenHit = false;
    private Camera mainCamera;

    void Start()
    {
        Vector3 pos = transform.position;
        pos.z = startZ;
        transform.position = pos;
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (hasBeenHit) return;

        transform.Translate(Vector3.back * speedZ * Time.deltaTime);

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            CheckTouchHit();
        }

        if (transform.position.z <= targetZ)
        {
            MissNote();
        }
    }

    void CheckTouchHit()
    {
        Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(touchPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                HitNote();
            }
        }
    }

    void HitNote()
    {
        if (hasBeenHit) return;
        hasBeenHit = true;
        Debug.Log("✅ ПОПАДАНИЕ!");
        Destroy(gameObject);
    }

    void MissNote()
    {
        if (hasBeenHit) return;
        hasBeenHit = true;
        Debug.Log("❌ ПРОМАХ");
        Destroy(gameObject);
    }
}