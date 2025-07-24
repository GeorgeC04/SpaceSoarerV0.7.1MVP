using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] tutorialPages; // assign pages in inspector
    public Button nextButton;
    public Button backButton;

    private int currentPage = 0;

    void Start()
    {
        ShowPage(0);
    }

    public void ShowPage(int pageIndex)
    {
        // Bounds check
        if (pageIndex < 0 || pageIndex >= tutorialPages.Length)
            return;

        // Disable all pages
        for (int i = 0; i < tutorialPages.Length; i++)
            tutorialPages[i].SetActive(i == pageIndex);

        currentPage = pageIndex;

         backButton.gameObject.SetActive(currentPage > 0);

        // Hide the Next button on the last page
        nextButton.gameObject.SetActive(currentPage < tutorialPages.Length - 1);
    }

    public void NextPage()
    {
        ShowPage(currentPage + 1);
    }

    public void PreviousPage()
    {
        ShowPage(currentPage - 1);
    }

    public void OpenTutorial()
    {
        gameObject.SetActive(true); // shows the panel
        ShowPage(0);                // starts on the first page
    }

    public void CloseTutorial()
    {
        ShowPage(0);                // reset to first page
       
    }



}
