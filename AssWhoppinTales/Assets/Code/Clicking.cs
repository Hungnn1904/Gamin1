using UnityEngine;
using UnityEngine.SceneManagement;
public class Clicking : MonoBehaviour
{
    public  void Skipping1(){
       SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
    public  void Skipping2(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+2);
    }

    public  void Backing1(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
    }
    public  void Backing2(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-2);
    }
}
