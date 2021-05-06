using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    #region EDITOR FIELDS
    [Header("Bomb")]
    [SerializeField] private Transform BombLayout;
    [SerializeField] private Button BombButtonPrefab;

    [Space]
    [Header("Buttons")]
    [SerializeField] private GameObject ClearButton;
    [SerializeField] private GameObject RetryButton;
    #endregion

    #region INITIALIZATION
    private void Awake()
    {
        if (Instance != null)
            Destroy(this);
        Instance = this;
    }
    #endregion

    #region BOMB LAYOUT
    public void LayoutAddBomb()
    {
        Button newButton = Instantiate(BombButtonPrefab, BombLayout);
        newButton.onClick.AddListener(SelectBomb);
    }

    public void LayoutRemoveBomb()
    {
        Destroy(BombLayout.GetChild(0).gameObject);
    }
    #endregion

    #region BUTTONS METHODS
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
    #endregion

    #region BUTTONS CHANGE
    public void ShowClearButton()
    {
        ClearButton.SetActive(true);
        RetryButton.SetActive(false);
    }
    public void ShowRetryButton()
    {
        ClearButton.SetActive(false);
        RetryButton.SetActive(true);
    }
    #endregion
}
