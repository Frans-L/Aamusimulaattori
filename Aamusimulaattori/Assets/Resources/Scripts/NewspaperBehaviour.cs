using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(MeshRenderer))]
public class NewspaperBehaviour : MonoBehaviour
{
    public float movementSpeed = 0.1f;
    public float distanceThreshold = 0.35f;
    public float startingDistance = 3f;
    public float spinCount = 3;
    public float displayTime = 3.5f;
    public float appearDelay = 0f;

    private float _spinAmount;
    private AudioSource _audioSource;
    private MeshRenderer _meshRenderer;
    private MeshRenderer[] _childRenderers;

    public void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _childRenderers = GetComponentsInChildren<MeshRenderer>();
        _SetVisibility(false);
        transform.parent = Camera.main.transform;
        transform.localPosition = new Vector3(0, 0, startingDistance);
        _spinAmount = spinCount * 360;
        transform.localRotation = Quaternion.Euler(0, 0, _spinAmount);
        Invoke("_Appear", appearDelay);
    }

    private void _SetVisibility(bool value)
    {
        _meshRenderer.enabled = value;
        foreach(var renderer in _childRenderers)
        {
            renderer.enabled = value;
        }
    }

    private void _Appear()
    {
        _SetVisibility(true);
        _PlayAudio();
        //Destroy(gameObject, displayTime);
        Invoke("_EndGame", displayTime);
    }

    private void _EndGame()
    {
        GameLost.EndGame(false, "OLET SADISTI");
    }

    private void _PlayAudio()
    {
        var audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    public void Update()
    {
        if(!_meshRenderer.enabled) return;
        if(transform.localPosition.z > distanceThreshold)
        {
            if(transform.localPosition.z - movementSpeed > distanceThreshold)
            {
                transform.localPosition = transform.localPosition - new Vector3(0, 0, movementSpeed);
            }
            else
            {
                transform.localPosition = new Vector3(0, 0, distanceThreshold);
            }

            var normalizedDistance = (transform.localPosition.z - distanceThreshold) / (startingDistance - distanceThreshold);
            transform.localRotation = Quaternion.Euler(0, 0, _spinAmount * normalizedDistance);
        }
    }

    public void SetTitle(string title, int fontSize)
    {
        var titleMesh = transform.Find("Title").GetComponent<TextMesh>();
        titleMesh.fontSize = fontSize;
        titleMesh.text = title;
    }
}
