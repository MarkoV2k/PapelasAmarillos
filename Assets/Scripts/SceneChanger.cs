using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public int numerSceny;
    public void ZmienScene()
    {
        SceneManager.LoadScene(1);
    }

    public void wyjscie()
    {
        Application.Quit();
    }
}
