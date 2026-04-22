using UnityEngine;
using System.Collections;

public class DiceVisual : MonoBehaviour
{
    private Rigidbody rb;
    private bool isRolling = false;
    private int result;

    private static readonly Vector3[] faceRotations = new Vector3[]
    {
        Vector3.zero,
        new Vector3(-90, 0, 0),
        new Vector3(0, 0, 0),
        new Vector3(0, 0, -90),
        new Vector3(0, 0, 90),
        new Vector3(-180, 0, 0),
        new Vector3(90, 0, 0)
    };

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Roll(int result)
    {
        this.result = result;
        StartCoroutine(RollAnimation());
    }

    private IEnumerator RollAnimation()
    {
        isRolling = true;

        rb.isKinematic = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(new Vector3(
            Random.Range(-2f, 2f),
            Random.Range(3f, 5f),
            Random.Range(-2f, 2f)
        ), ForceMode.Impulse);
        rb.AddTorque(new Vector3(
            Random.Range(-10f, 10f),
            Random.Range(-10f, 10f),
            Random.Range(-10f, 10f)
        ), ForceMode.Impulse);

        yield return new WaitForSeconds(2f);


        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        Quaternion targetRot = Quaternion.Euler(faceRotations[result]);
        float elapsed = 0f;
        float duration = 0.5f;
        Quaternion startRot = transform.rotation;

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRot;

        isRolling = false;
    }
    public bool IsRolling() => isRolling;
    public int GetResult() => result;


    public void ResetDice(Vector3 spawnPosition)
    {
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = spawnPosition;

        transform.rotation = UnityEngine.Random.rotation;
    }
}
