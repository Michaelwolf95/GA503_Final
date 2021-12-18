using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	[SerializeField] private CanvasGroup victoryScreen;
	[SerializeField] private Button mainMenuButton;

	private void Awake()
	{
		Instance = this;

		ConfigureMenus();
	}

	private void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	private void Start()
	{
		this.InvokeAction((() =>
		{
			StartLevel();
		}), 0.8f);
	}

	private void StartLevel()
	{
		PlayerController.Instance?.OnLevelStart();
	}

	public void CompleteLevel()
	{
		PlayerController.Instance?.OnLevelCompleted();
		
		this.InvokeAction((() =>
		{
			victoryScreen.gameObject.SetActive(true);
		}), 0.3f);
	}
	
	public void ResetLevel()
	{
		this.InvokeAction((() =>
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}), 1f);
	}


	private void ConfigureMenus()
	{
		mainMenuButton.onClick.AddListener((() =>
		{
			SceneManager.LoadScene(0);
		}));
		victoryScreen.gameObject.SetActive(false);
	}
}