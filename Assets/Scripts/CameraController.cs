using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [Header("Cameras")]
    public Camera mainCamera;
    public Camera leftCamera;
    public Camera rightCamera;
    public Camera followCamera;

    [Header("Dice")]
    public GameObject dice;

    Vector3 followCameraOffSet;

    void Awake()
    {
        // At the start enable the main camera.
        EnableMainCamera();

        //Set some random values for the follow camera offset.
        followCameraOffSet = new Vector3(Random.Range(-5, 5), Random.Range(3, 7), Random.Range(3, 7));
        //followCameraOffSet = new Vector3(0f, 5f, 5f);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (mainCamera.enabled)
            mainCamera.transform.LookAt(dice.transform);

        if (rightCamera.enabled)
            rightCamera.transform.LookAt(dice.transform);

        if (leftCamera.enabled)
            leftCamera.transform.LookAt(dice.transform);

        if(followCamera.enabled)
        {
            //Make the follow look at the dice (rotation and position)
            followCamera.transform.LookAt(dice.transform);
            followCamera.transform.position = dice.transform.position + followCameraOffSet;
        }
    }

    #region Private Methods

    /// <summary>
    /// Enables the main camera to follow the dice.
    /// </summary>
    private void EnableMainCamera()
    {
        mainCamera.enabled = true;
        leftCamera.enabled = false;
        rightCamera.enabled = false;
        followCamera.enabled = false;
    }

    /// <summary>
    /// Enables Left Camera to follow the dice after launch.
    /// </summary>
    private void EnableLeftCamera()
    {
        leftCamera.enabled = true;
        mainCamera.enabled = false;
        rightCamera.enabled = false;
        followCamera.enabled = false;
    }

    /// <summary>
    /// Enables Right Camera to follow the dice after launch.
    /// </summary>
    private void EnableRightCamera()
    {
        rightCamera.enabled = true;
        mainCamera.enabled = false;
        leftCamera.enabled = false;
        followCamera.enabled = false;
    }

    /// <summary>
    /// Enables Follow Camera to follow the dice after launch.
    /// </summary>
    private void EnableFollowCamera()
    {
        followCamera.enabled = true;
        rightCamera.enabled = false;
        mainCamera.enabled = false;
        leftCamera.enabled = false;
    }

    IEnumerator ChooseCameraCoroutine()
    {
        //Debug.Log("Entered on the ChooseCameraCoroutine");

        float distanceRightCamera = Vector3.Distance(dice.transform.position, rightCamera.transform.position);
        float distanceLeftCamera = Vector3.Distance(dice.transform.position, leftCamera.transform.position);

        yield return new WaitForSeconds(1f);

        if (distanceRightCamera < distanceLeftCamera)
            EnableRightCamera();
        else
            EnableLeftCamera();
    }

    #endregion

    #region Public Methods

    public void ChooseCamera()
    {
        //Debug.Log("Entered on the ChooseCamera");

        StartCoroutine(ChooseCameraCoroutine());
    }

    public void SetFollowCamera()
    {
        EnableFollowCamera();
    }

    #endregion
}
