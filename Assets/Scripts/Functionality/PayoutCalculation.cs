using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class PayoutCalculation : MonoBehaviour
{
    [SerializeField]
    private int x_Distance;
    [SerializeField]
    private float y_Distance;
    [SerializeField]
    private Material[] LineMats;

    [SerializeField]
    private Transform LineContainerEnable;
    [SerializeField]
    private Transform LineContainerDisable;
    [SerializeField]
    private GameObject Line_Prefab;
    [SerializeField]
    private GameObject Line2_Prefab;
    internal List<int> DontDestroyLines = new List<int>();

    [SerializeField]
    private List<float> yStartpoints;

    [SerializeField]
    private Vector2 InitialLinePosition = new Vector2(-315, 100);

    [SerializeField]
    private List<GameObject> WinLinesEnabled;
    [SerializeField]
    private List<GameObject> WinLinesDisabled;

    GameObject TempObj = null;

    //generate lines at runtime accordingly
    internal void GeneratePayoutLinesBackend(List<int> y_index, int Count, int matCount = 0)
    {
        GameObject MyLineObjEnable = null;
        GameObject MyLineObjDisable = null;
        if (y_index[0] == 1)
        {
            MyLineObjEnable = Instantiate(Line2_Prefab, LineContainerEnable);
            MyLineObjDisable = Instantiate(Line2_Prefab, LineContainerDisable);
        }
        else
        {
            MyLineObjEnable = Instantiate(Line_Prefab, LineContainerEnable);
            MyLineObjDisable = Instantiate(Line_Prefab, LineContainerDisable);
        }
        MyLineObjDisable.SetActive(false);
        MyLineObjEnable.SetActive(false);
        MyLineObjEnable.transform.localPosition = new Vector2(InitialLinePosition.x, InitialLinePosition.y);
        UILineRenderer MyLine = MyLineObjEnable.GetComponent<UILineRenderer>();
        MyLine.material = LineMats[1];
        MyLineObjDisable.transform.localPosition = new Vector2(InitialLinePosition.x, InitialLinePosition.y);
        UILineRenderer MyLine2 = MyLineObjDisable.GetComponent<UILineRenderer>();
        MyLine2.material = LineMats[0];
        for (int i = 0; i < Count; i++)
        {
            var points = new Vector2() { x = i * x_Distance, y = yStartpoints[i] - (y_index[i] * 137) };
            var pointlist = new List<Vector2>(MyLine.Points);
            pointlist.Add(points);
            MyLine.Points = pointlist.ToArray();
        }
        var temppoints = new Vector2() { x = (5 * x_Distance) + 120, y = yStartpoints[5] - (y_index[5] * 137) };
        var temppointlist = new List<Vector2>(MyLine.Points);
        temppointlist.Add(temppoints);
        MyLine.Points = temppointlist.ToArray();
        var newpointlist = new List<Vector2>(MyLine.Points);
        MyLine.Points = newpointlist.ToArray();
        MyLine2.Points = MyLine.Points;
        WinLinesEnabled.Add(MyLineObjEnable);
        WinLinesDisabled.Add(MyLineObjDisable);
    }

    internal void StartLinesRoutine()
    {
        StartCoroutine(LineRoutine());
    }

    private IEnumerator LineRoutine()
    {
        for (int i = 0; i < WinLinesEnabled.Count; i++)
        {
            WinLinesDisabled[i].SetActive(true);
            yield return new WaitForSecondsRealtime(0.2f);
            WinLinesEnabled[i].SetActive(true);
        }
        foreach (Transform child in LineContainerEnable)
        {
            child.gameObject.SetActive(false);
        }
        for (int i = 0; i < WinLinesEnabled.Count; i++)
        {
            WinLinesEnabled[i].SetActive(true);
            yield return new WaitForSecondsRealtime(0.5f);
            WinLinesEnabled[i].SetActive(false);
        }
        foreach (Transform child in LineContainerEnable)
        {
            child.gameObject.SetActive(false);
        }
        foreach (Transform child in LineContainerDisable)
        {
            child.gameObject.SetActive(false);
        }
        while (true)
        {
            for (int i = 0; i < WinLinesEnabled.Count; i++)
            {
                WinLinesEnabled[i].SetActive(true);
                yield return new WaitForSecondsRealtime(1f);
                WinLinesEnabled[i].SetActive(false);
            }
        }
    }

    //delete all lines
    internal void ResetLines()
    {
        StopCoroutine(LineRoutine());
        foreach (Transform child in LineContainerDisable)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in LineContainerEnable)
        {
            Destroy(child.gameObject);
        }
        WinLinesEnabled.Clear();
        WinLinesEnabled.TrimExcess();
        WinLinesDisabled.Clear();
        WinLinesDisabled.TrimExcess();
    }
}
