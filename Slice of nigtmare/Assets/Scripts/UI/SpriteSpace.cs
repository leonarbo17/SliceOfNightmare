using UnityEngine;
using UnityEngine.SceneManagement;

public class SpriteSpace : MonoBehaviour
{
    public GameObject spaceKeySprite;
    private bool checkSpaceAnimation = false;

    
    public void ShowKey()
    {
        spaceKeySprite.SetActive(true);
    }
    public void check()
    {
        checkSpaceAnimation = true;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && checkSpaceAnimation)
        {
            SceneManager.LoadScene(2);
        }
    }


}

