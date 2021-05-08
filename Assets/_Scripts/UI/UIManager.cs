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
    [SerializeField] private GameObject PlayButton;
    [SerializeField] private GameObject LayoutContainer;
    [SerializeField] private GameObject Pipe;
    #endregion

    #region PRIVATE PARAMETERS
    private List<Image> buttonImages;
    #endregion

    #region INITIALIZATION
    private void Awake()
    {
        if (Instance != null)
            Destroy(this);
        Instance = this;
        buttonImages = new List<Image>();
    }
    #endregion

    #region BOMB LAYOUT
    public void LayoutAddBomb(int index, Sprite sprite)
    {
        Button newButton = Instantiate(BombButtonPrefab, BombLayout);
        newButton.onClick.AddListener(delegate { SelectBomb(index); });
        newButton.image.sprite = sprite;
        buttonImages.Add(newButton.image);
    }

    public void LayoutGreyBomb(int index)
    {
        buttonImages[index].color = Color.gray;
    }

    public void LayoutUnGreyBomb(int index)
    {
        buttonImages[index].color = Color.white;
    }
    #endregion

    #region BUTTONS METHODS
    public void SelectBomb(int which)
    {
        PlacementManager.Instance.SelectBomb(which);
    }

    public void InitiateSequence()
    {
        PlayButton.SetActive(false);
        LayoutContainer.SetActive(false);
        Pipe.SetActive(false);
        PlacementManager.Instance.InitiateSequence();
    }

    public void RetryLevel()
    {
        PlayButton.SetActive(true);
        LayoutContainer.SetActive(true);
        Pipe.SetActive(true);
        PlacementManager.Instance.LoadPreviousPlacement();
    }
    public void ClearLevel()
    {
        PlacementManager.Instance.Clear();
    }

    public void MainMenu()
    {
        PlacementManager.Instance.Stop();
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
