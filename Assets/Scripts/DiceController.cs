using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(CameraController))]
public class DiceController : MonoBehaviour
{
    [Header("Dice Settings")]
    public float xAngle;
    public float yAngle;
    public float zAngle;
    public float damping;
    public float rotationTimerSeconds = 1.0f;
    public float checkVelocityTimerSeconds = 2.0f;
    public float forceMultiplier = 3f;
    public bool useRotation = true; //public for test purposes.

    [Header("Game Objects")]
    public GameObject RaycastOriginObject;
    public CameraController cameraController;
    public Image pointerHelper;
    public Text gotNumberTxt;

    private float changeValuesTimer;
    private float checkVelocityTimer;
    private float pointerSpawnTimer;
    private bool shooted = false;
    private bool finished = false;
    private Vector3 mouseInitialPos;
    private Vector3 mouseReleasePos;
    private RaycastHit raycastHit;
    private Rigidbody rigidBody;
    private AudioSource audioSource;
   

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody>();
        audioSource = gameObject.GetComponent<AudioSource>();

        //Randomize the start rotation.
        ChangeValues();

        //Initiates the timer value.
        checkVelocityTimer = checkVelocityTimerSeconds;
    }

    // Update is called once per frame
    void Update()
    {
        if (useRotation)
        {
            //Set the initial values for the rotation of the dice.
            transform.Rotate(xAngle, yAngle, zAngle, Space.Self);

            //Clamp a value base on delta time for the rotation time.
            float time = Mathf.Clamp(Time.deltaTime * damping, 0f, 0.99f);

            //Set the rotation.
            transform.rotation = Quaternion.Lerp(transform.rotation, new Quaternion(), time);
        }
    }

    void FixedUpdate()
    {
        //Control the rotation changing of the dice.
        //If is not shooted yet, continue to change the rotation.
        //If it was shooted, check the velocity until it stops.
        if (shooted)
        {
            //If reachs zero, check the dice result value.
            //If the result is already checked, then there is not need to keep checking.
            if (checkVelocityTimer <= 0f && !finished)
            {
                if (Mathf.Approximately(rigidBody.velocity.x, 0) &&
                    Mathf.Approximately(rigidBody.velocity.y, 0) &&
                    Mathf.Approximately(rigidBody.velocity.z, 0))
                {
                    //Debug.Log("Stoped!");
                    CheckDiceValue();
                }
            }

            //Timer countdown.
            //Continue the countdown if it's major than zero.
            if (checkVelocityTimer > 0f)
            {
                checkVelocityTimer -= Time.deltaTime;
            }

        }
        else
        {
            //If the timer is Up, generate new random rotation value.
            if (changeValuesTimer <= 0f)
            {
                ChangeValues();

                changeValuesTimer = rotationTimerSeconds;
            }

            //If the user not point and drag the mouse in 10 seconds, show the point helper.
            if(pointerSpawnTimer > 10f && !pointerHelper.gameObject.activeInHierarchy)
            {
                ShowPointerHelper();
            }

            //Timer countdown.
            changeValuesTimer -= Time.deltaTime;
            pointerSpawnTimer += Time.deltaTime;
        }
    }

    #region Private Methods

    /// <summary>
    /// This method change the values of a random angle.
    /// </summary>
    void ChangeValues()
    {
        int angleToChange = Random.Range(0, 2);

        switch (angleToChange)
        {
            case 0: xAngle = Random.Range(2f, 4f) * (Random.Range(0, 2) * 2 - 1); break;
            case 1: xAngle = Random.Range(2f, 4f) * (Random.Range(0, 2) * 2 - 1); break;
            case 2: xAngle = Random.Range(2f, 4f) * (Random.Range(0, 2) * 2 - 1); break;
            default: Debug.Log($"Numero: {angleToChange} sorteado, nenhum angulo a mudar."); break;
        }
    }

    private void Shoot(Vector3 force)
    {
        //rigidBody.AddForce(force * forceMultiplier);
        rigidBody.AddForce(new Vector3(force.x, force.y, force.y) * forceMultiplier);

        //It's wierd to change the rotation of the dice after shooting it.
        //For a more natural behaviour, we stop this rotation changing after shooting it.
        shooted = true;

        //Choose the camera that will follow the dice.
        cameraController.ChooseCamera();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Collided with: " + collision.gameObject.name);
        audioSource.pitch = Random.Range(0.5f, 1.5f);
        audioSource.Play();

        if (collision.gameObject.name.Equals("ResetObject"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        //Once the dice collide with somthing, stop all rotations.
        if(useRotation)
            useRotation = false;

        //Back to main camera.
        cameraController.SetFollowCamera();
    }

    private void CheckDiceValue()
    {
        //Repositionate the RaycastOriginObject to be above the dice, then raycast to see what number is facing up.
        RaycastOriginObject.transform.position = new Vector3(transform.position.x, transform.position.y + 10, transform.position.z);

        if (Physics.Raycast(RaycastOriginObject.transform.position, RaycastOriginObject.transform.TransformDirection(Vector3.down), out raycastHit))
        {
            //Debug.DrawRay(RaycastOriginObject.transform.position, RaycastOriginObject.transform.TransformDirection(Vector3.down) * raycastHit.distance, Color.yellow);
            //Debug.Log("Did Hit: "+ raycastHit.collider.gameObject.name);

            switch (raycastHit.collider.gameObject.tag)
            {
                case "One": Debug.Log("One"); gotNumberTxt.text = "Got Number: 1"; break;
                case "Two": Debug.Log("Two"); gotNumberTxt.text = "Got Number: 2"; break;
                case "Three": Debug.Log("Three"); gotNumberTxt.text = "Got Number: 3"; break;
                case "Four": Debug.Log("Four"); gotNumberTxt.text = "Got Number: 4"; break;
                case "Five": Debug.Log("Five"); gotNumberTxt.text = "Got Number: 5"; break;
                case "Six": Debug.Log("Six"); gotNumberTxt.text = "Got Number: 6"; break;
                default: Debug.Log("Did not found the collider."); break;
            }

            finished = true;
        }
    }

    private void ShowPointerHelper()
    {
        pointerHelper.gameObject.SetActive(true);
        LeanTween.moveLocalY(pointerHelper.gameObject, -280f, .7f).setLoopClamp();
        LeanTween.alpha(pointerHelper.rectTransform, 0f, .7f).setEaseInExpo().setLoopClamp();
    }

    #endregion

    #region Public Methods

    public void OnMouseDown()
    {
        mouseInitialPos = Input.mousePosition;

        //Reset pointer helper variables
        pointerHelper.gameObject.SetActive(false);
        pointerSpawnTimer = 0f;
    }

    public   void OnMouseUp()
    {
        mouseReleasePos = Input.mousePosition;
        rigidBody.useGravity = true;
        Shoot(mouseInitialPos - mouseReleasePos);
    }

    #endregion
}
