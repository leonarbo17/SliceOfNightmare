using UnityEngine;

public class FlashLight : MonoBehaviour
{
    public GameObject lightObject;
    public AudioClip lightSound;
    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            lightManager();
        }
    }

    void lightManager()
    {
        lightObject.SetActive(!lightObject.activeSelf);
        source.PlayOneShot(lightSound);
     }
}
