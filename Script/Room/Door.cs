using System.Collections;
using UnityEngine;

public class Door : ProgressObstacle
{
    [SerializeField]
    private float timer = 2.0f;

    [SerializeField]
    private float openedAngle = 90.0f;

    [SerializeField]
    private AudioClip openSound;

    [SerializeField]
    private AudioClip closeSound;

    [SerializeField]
    //private bool testOpen = false;

    public bool isOpen = false;
    private Vector3 closedRotation;
    private Vector3 openRotation;
    private Coroutine currentCoroutine;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component missing from the door object.");
        }
    }

    private void Start()
    {
        // Save the initial local rotation as the closed rotation
        closedRotation = transform.localEulerAngles;
        // Calculate the open rotation by rotating around the Y axis
        openRotation = closedRotation + new Vector3(0, openedAngle, 0);
    }

   /* private void Update()
    {
        // This is for testing from the Inspector.
        if (testOpen && !isOpen)
        {
            Open();
        }
        else if (!testOpen && isOpen)
        {
            Close();
        }
    }*/

    public override void Open() 
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(RotateDoor(openRotation));
        PlaySound(openSound);
        isOpen = true;
    }

    public override void Close()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(RotateDoor(closedRotation));
        PlaySound(closeSound);
        isOpen = false;
    }

    private IEnumerator RotateDoor(Vector3 targetRotation)
    {
        float elapsedTime = 0.0f;
        Quaternion startingRotation = transform.localRotation;
        Quaternion targetQuaternion = Quaternion.Euler(targetRotation);

        while (elapsedTime < timer)
        {
            transform.localRotation = Quaternion.Slerp(startingRotation, targetQuaternion, elapsedTime / timer);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = targetQuaternion;
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
