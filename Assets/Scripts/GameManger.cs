using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    [SerializeField] private GameObject gridGenerator;
    private FileToGridManager _fileToGridManager;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject hintPrefab;
    [SerializeField] private SpriteRenderer boardPrefab;

    private GameObject _gridBoard;
    private float _width, _height;
    private List<List<string>> _gridValues;
    private readonly HashSet<(int, int)> _path = new();
    private readonly HashSet<(int, int)> _tempPath = new();
    private readonly Dictionary<(int,int), bool> _dfsCache = new();
    private bool _hintGiven = false;

    void Start()
    {
        _fileToGridManager = gridGenerator.GetComponent<FileToGridManager>();
        PopulateGrid();
    }

    private void PopulateGrid()
    {
        _gridValues = _fileToGridManager.CreateNewGridFromFile();
        _height = _fileToGridManager.GetRows();
        _width = _fileToGridManager.GetCols();

        int i = 0;
        int j = 0;
        _gridBoard = new GameObject("GridBoard");

        foreach (var row in _gridValues)
        {
            foreach (var val in row)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(j,i,0), Quaternion.identity, _gridBoard.transform);

                var sprite = tile.transform.GetChild(0).GetComponent<SpriteRenderer>();

                sprite.sprite = Resources.Load<Sprite>(val);
                j++;
            }

            i++;
            j = 0;
        }

        var centerOfGrid = new Vector2(_width / 2 - 0.5f, _height / 2 - 0.5f);

        SpriteRenderer board = Instantiate(boardPrefab, centerOfGrid, Quaternion.identity, _gridBoard.transform);
        board.size = new Vector2(_width, _height);

        Camera.main!.transform.position = new Vector3(centerOfGrid.x, centerOfGrid.y, -10);
    }

    public void Reload()
    {
        Destroy(_gridBoard);
        PopulateGrid();
        _path.Clear();
        _dfsCache.Clear();
        _hintGiven = false;
    }

    public void GetHint()
    {
        if(_hintGiven)
            return;

        PopulateHintPath();
        _hintGiven = true;

        int i = 0;
        int j = 0;

        foreach (var row in _gridValues)
        {
            foreach (var val in row)
            {
                if (_path.Contains((i, j)))
                {
                    Instantiate(hintPrefab, new Vector3(j,i,0), Quaternion.identity, _gridBoard.transform);
                }

                j++;
            }
            i++;
            j = 0;
        }
    }

    private void PopulateHintPath()
    {
        for (int r = 0; r < _height; r++)
        {
            for (int c = 0; c < _width; c++)
            {
                Dfs(_gridValues, r, c, _gridValues[r][c], (int)_height, (int)_width);
                if (_tempPath.Count > _path.Count)
                {
                    _path.Clear();
                    _path.UnionWith(_tempPath);
                }
                _tempPath.Clear();
            }
        }
    }

    private void Dfs(List<List<string>> grid, int r, int c, string prevValue, int rows, int cols)
    {
        if (r < 0 || r == rows || c < 0 || c == cols || !grid[r][c].Equals(prevValue) || grid[r][c].Equals(string.Empty))
        {
            return;
        }

        if (_dfsCache.TryGetValue((r,c), out _))
        {
            return;
        }

        _tempPath.Add((r,c));
        _dfsCache.Add((r,c), true);

        Dfs(grid, r + 1, c, grid[r][c], rows, cols);
        Dfs(grid, r - 1, c, grid[r][c], rows, cols);
        Dfs(grid, r, c + 1, grid[r][c], rows, cols);
        Dfs(grid, r, c - 1, grid[r][c], rows, cols);
    }
}
