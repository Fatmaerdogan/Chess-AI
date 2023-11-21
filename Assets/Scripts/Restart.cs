using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
	public GameObject gameFinishUI;

	private void Start()=>Events.GameFinish += GameFinishMenu;
    private void OnDestroy() => Events.GameFinish -= GameFinishMenu;

    public void GameFinishMenu()=>gameFinishUI.SetActive(true);
    public void LoadScene()=>SceneManager.LoadScene(0);
}
