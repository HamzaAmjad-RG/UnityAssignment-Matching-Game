using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class FileToGridManager : MonoBehaviour
{
    [SerializeField] private string fileName = "layout";

    private readonly List<string> _blockAssets = new() {"P1", "P2", "P3", "P4", "P4", "P5", "P6", "P7"};
    private List<List<string>> _grid  = new();

    public int GetCols()
    {
        return _grid.Max(x => x.Count);
    }

    public int GetRows()
    {
        return _grid.Count;
    }

    //Reloads needs it to check if the csv file changed.
    //Keep this separate from instantiation to provide flexibility
    // of changing instantiation logic.
    public List<List<string>> CreateNewGridFromFile()
    {
        TextAsset file = Resources.Load(fileName) as TextAsset;
        if (file == null)
        {
            Debug.Log($"File {fileName} not found!!");
            return new List<List<string>>();
        }

        //Assumes that the fileContent only has 1s and 0s.
        string fileContent = file.ToString();
        string[] lines = fileContent.Split("\n"[0]);

        _grid = new List<List<string>>();

        int index = 0;
        foreach (var line in lines)
        {
            _grid.Add(new List<string>());
            var row = line.Split(",");
            foreach (var val in row)
            {
                _grid[index].Add(val.Equals("1") ? GetRandomSpriteName() : string.Empty);
            }
            index++;
        }

        //Clone here (defying OOP principles)
        return _grid;
    }

    private string GetRandomSpriteName()
    {
        return _blockAssets[Random.Range(0, _blockAssets.Count)];
    }

}
