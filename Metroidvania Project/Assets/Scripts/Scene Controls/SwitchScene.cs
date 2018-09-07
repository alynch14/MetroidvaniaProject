using UnityEngine;
using UnityEngine.SceneManagement;
 
public class Control : MonoBehaviour
{
    public void NextScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}