using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class TutorialPageEntry
{
    public GameObject pageObject;
    public Image timelineDotImage;
}

public class TutorialPageController : MonoBehaviour
{
    [SerializeField] private List<TutorialPageEntry> pageEntries = new();

    [Header("Timeline Dot Color")]
    [SerializeField] private Color activeDotColor = Color.white;
    [SerializeField] private Color inactiveDotColor = Color.gray;

    private int _currentPageIndex;

    public int CurrentPageIndex => _currentPageIndex;
    public int PageCount => pageEntries.Count;

    public bool IsFirstPage => _currentPageIndex <= 0;
    public bool IsLastPage => _currentPageIndex >= pageEntries.Count - 1;

    private void Awake()
    {
        ShowPage(0);
    }

    public void ResetPage()
    {
        ShowPage(0);
    }

    public void NextPage()
    {
        if (pageEntries == null || pageEntries.Count == 0)
            return;

        if (IsLastPage)
            return;

        ShowPage(_currentPageIndex + 1);
    }

    public void BackPage()
    {
        if (pageEntries == null || pageEntries.Count == 0)
            return;

        if (IsFirstPage)
            return;

        ShowPage(_currentPageIndex - 1);
    }

    public void ShowPage(int index)
    {
        if (pageEntries == null || pageEntries.Count == 0)
        {
            Debug.LogWarning("[TutorialPageController] pageEntries Ç™ãÛÇ≈Ç∑ÅB", this);
            return;
        }

        index = Mathf.Clamp(index, 0, pageEntries.Count - 1);
        _currentPageIndex = index;

        for (int i = 0; i < pageEntries.Count; i++)
        {
            var entry = pageEntries[i];
            if (entry == null) continue;

            bool isCurrent = i == _currentPageIndex;

            if (entry.pageObject != null)
                entry.pageObject.SetActive(isCurrent);

            if (entry.timelineDotImage != null)
                entry.timelineDotImage.color = isCurrent
                    ? activeDotColor
                    : inactiveDotColor;
        }
    }
}