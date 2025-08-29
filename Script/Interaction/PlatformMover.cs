using System.Collections;
using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    [SerializeField]
    private Vector3 startingLocalPos;

    [SerializeField]
    private Vector3 targetLocalPos;

    [SerializeField]
    private float moveDuration = 2f;

    private Coroutine moveCoroutine = null;
    private AudioSource audioSource;

    public MeshRenderer[] meshRenderers;
    public Material startMat;
    public Material activeMat;

    public bool matchOnStart = true;

    void Start()
    {
        if (matchOnStart) {
            transform.localPosition = startingLocalPos;
        }
        
        audioSource = GetComponent<AudioSource>();
        foreach(MeshRenderer renderer in meshRenderers) { renderer.material = startMat; }
    }

    public void Activate()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(MovePlatform(targetLocalPos));
        audioSource.Play();
        ActiveMaterial();
    }

    public void Deactivate()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(MovePlatform(startingLocalPos));
        audioSource.Play();
        DeactivateMaterial();
    }

    private IEnumerator MovePlatform(Vector3 targetPosition)
    {
        
        Vector3 initialPosition = transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPosition;
      
        moveCoroutine = null;
    }

    public void ActiveMaterial() { foreach (MeshRenderer renderer in meshRenderers) { renderer.material = activeMat; } }
    public void DeactivateMaterial() { foreach (MeshRenderer renderer in meshRenderers) { renderer.material = startMat; } }
}
