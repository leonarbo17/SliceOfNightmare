using UnityEngine;

public class FPSLimit : MonoBehaviour
{
    public int limiteFPS = 100;

    private void Start()
    {
        Application.targetFrameRate = limiteFPS;
    }
}
