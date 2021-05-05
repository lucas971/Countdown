using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private Transform bombLayout;
    [SerializeField] private Button bombButtonPrefab;

    [SerializeField] private GameObject clearButton;
    [SerializeField] private GameObject retryButton;

    
    private void Awake()
    {
        if (Instance != null)
            Destroy(this);
        Instance = this;
    }
    public void LayoutAddBomb()
    {
        Button newButton = Instantiate(bombButtonPrefab, bombLayout);
        newButton.onClick.AddListener(SelectBomb);
    }

    public void LayoutRemoveBomb()
    {
        Destroy(bombLayout.GetChild(0).gameObject);
    }

    public void SelectBomb()
    {
        PlacementManager.Instance.SelectBomb();
    }

    public void InitiateSequence()
    {
        PlacementManager.Instance.InitiateSequence();
    }

    public void RetryLevel()
    {
        PlacementManager.Instance.LoadPreviousPlacement();
    }
    public void ClearLevel()
    {
        SceneManager.LoadScene(0);
    }
    public void ShowClearButton()
    {
        clearButton.SetActive(true);
        retryButton.SetActive(false);
    }
    public void ShowRetryButton()
    {
        clearButton.SetActive(false);
        retryButton.SetActive(true);
    }
}
