using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BallHandler : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float delayBeforeDetachBall = 1f;
    [SerializeField] private float respawnDelay;
    [SerializeField] private int numBalls = 0;
    [SerializeField] private float sceneReloadDelay = 5f;

    private Camera mainCamera;
    private bool isDragging;

    private Rigidbody2D currentBallRigidbody;
    private SpringJoint2D currentBallSpringJoint;

    void Start()
    {
        mainCamera = Camera.main;

        SpawnNewBall();
        
    }

    void Update()
    {
        if (currentBallRigidbody == null)
        {
            return;
        }

        if (!Touchscreen.current.primaryTouch.press.isPressed)
        {
            if (isDragging)
            {
                LaunchBall();
            }

            isDragging = false;

            return;
        }

        isDragging = true;

        currentBallRigidbody.isKinematic = true;
        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);
        currentBallRigidbody.position = worldPosition;       
    }

    private void LaunchBall()
    {
        currentBallRigidbody.isKinematic = false;
        currentBallRigidbody = null;

        Invoke(nameof(DetachBall), delayBeforeDetachBall);  
    }

    private void DetachBall()
    {
        currentBallSpringJoint.enabled = false;
        currentBallSpringJoint = null;

        Invoke(nameof(SpawnNewBall), respawnDelay);

        numBalls++;

        if (numBalls == 3)
        {
            Invoke(nameof(reloadScene), sceneReloadDelay);

        }
    }

    private void SpawnNewBall()
    {
        
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);

        currentBallRigidbody = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSpringJoint = ballInstance.GetComponent<SpringJoint2D>();

        currentBallSpringJoint.connectedBody = pivot;

        
    }

    private void reloadScene()
    {
        SceneManager.LoadScene(0);
    }
}
