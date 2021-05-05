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

    public void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }
}
