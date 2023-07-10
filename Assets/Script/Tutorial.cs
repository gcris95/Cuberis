using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour {

    public GameObject page1;
    public GameObject page2;
    public GameObject page3;
    public GameObject page4;

    public void Page2()
    {
        page1.SetActive(false);
        page2.SetActive(true);
    }

    public void Page3()
    {
        page2.SetActive(false);
        page3.SetActive(true);
    }

    public void Page4()
    {
        page3.SetActive(false);
        page4.SetActive(true);
    }	

    public void endTutorial()
    {
        SceneManager.LoadScene("CUBEris");
    }
}
