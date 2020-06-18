using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridController : MonoBehaviour
{
    [SerializeField] private List<Gem> availableGems;
    [SerializeField] private float swapSpeed;
    [SerializeField] private float dropSpeed;
    [SerializeField] private float stepTime;
    [SerializeField] private AudioClip swapClip;
    [SerializeField] private AudioClip clearClip;
    [SerializeField] private float shuffleSoundDuration;

    public delegate void NotBusy();

    public event NotBusy OnNotBusy;

    private GemGrid _grid;
    private MatchFinder _finder;
    private Camera _camera;
    private RoundController _roundController;

    private MatrixIndex _selectedIndex;
    private bool _busy;
    private AudioSource _shuffleSound;

    private void Start()
    {
        _grid = GetComponent<GemGrid>();
        _finder = GetComponent<MatchFinder>();
        _camera = Camera.main;
        _shuffleSound = GetComponent<AudioSource>();
        _roundController = FindObjectOfType<RoundController>();

        InitialSpawn();
        ClearGemSelection();
        SetBusy(false);
    }

    public void SelectGemWithPosition(Vector2 position)
    {
        if ( !_busy ) {
            _selectedIndex = _grid.GetIndexFromWorldPosition(position);
        }
    }

    public void MouseOverGemWithPosition(Vector2 position)
    {
        if ( HasSelectedGem() ) {
            var index = _grid.GetIndexFromWorldPosition(position);

            if ( index.i != _selectedIndex.i && index.j != _selectedIndex.j ) {
                //invalid move
                ClearGemSelection();
            }

            if ( index.i != _selectedIndex.i || index.j != _selectedIndex.j ) {
                if ( HasSelectedGem() ) {
                    StartCoroutine(Swap(_selectedIndex, index));
                }

                ClearGemSelection();
            }
        }
    }

    private IEnumerator Swap(MatrixIndex indexA, MatrixIndex indexB, bool returning = false)
    {
        SetBusy(true);

        AudioSource.PlayClipAtPoint(swapClip, _camera.transform.position);

        var speed = swapSpeed * transform.lossyScale.x;
        _grid.GetValue(indexA).SetPosition(_grid.GetWorldPositionFromIndex(indexB), speed);
        _grid.GetValue(indexB).SetPosition(_grid.GetWorldPositionFromIndex(indexA), speed);

        var tempGem = _grid.GetValue(indexA);
        _grid.SetValue(indexA, _grid.GetValue(indexB));
        _grid.SetValue(indexB, tempGem);

        yield return new WaitForSeconds(stepTime);

        if ( !returning ) {
            ProcessSwap(indexA, indexB);
        }
        else {
            SetBusy(false);
        }
    }

    private void ProcessSwap(MatrixIndex indexA, MatrixIndex indexB)
    {
        var gemA = _grid.GetValue(indexA);
        var rowMachFromA = _finder.GetRowMatch(indexA, gemA);
        var columnMatchFromA = _finder.GetColumnMatch(indexA, gemA);

        var gemB = _grid.GetValue(indexB);
        var rowMachFromB = _finder.GetRowMatch(indexB, gemB);
        var columnMatchFromB = _finder.GetColumnMatch(indexB, gemB);

        if ( rowMachFromA.Count() == 0 && columnMatchFromA.Count() == 0 &&
             rowMachFromB.Count() == 0 && columnMatchFromB.Count() == 0 ) {
            StartCoroutine(Swap(indexA, indexB, true));
        }
        else {
            var matches = new List<Match>();

            if ( rowMachFromA.Count() > 0 ) {
                matches.Add(rowMachFromA);
            }

            if ( columnMatchFromA.Count() > 0 ) {
                matches.Add(columnMatchFromA);
            }

            if ( rowMachFromB.Count() > 0 ) {
                matches.Add(rowMachFromB);
            }

            if ( columnMatchFromB.Count() > 0 ) {
                matches.Add(columnMatchFromB);
            }

            ClearMatches(matches);
        }
    }

    private void ScoreMatches(List<Match> matches)
    {
        foreach (var match in matches) {
            switch (match.indexes.Count) {
                case 3:
                    _roundController.ScoreMatch3();
                    break;
                case 4:
                    _roundController.ScoreMatch4();
                    break;
                case 5:
                    _roundController.ScoreMatch5();
                    break;
            }
        }
    }

    private void ClearMatches(List<Match> matches)
    {
        AudioSource.PlayClipAtPoint(clearClip, _camera.transform.position);

        var gemsToDestroy = new List<Gem>();

        foreach (var match in matches) {
            foreach (var index in match.indexes) {
                var gem = _grid.GetValue(index);
                if ( !gemsToDestroy.Contains(gem) ) {
                    _grid.SetValue(index, null);
                    gemsToDestroy.Add(gem);
                }
            }
        }

        foreach (var gem in gemsToDestroy) {
            if ( gem ) {
                gem.Clear();
            }
        }

        ScoreMatches(matches);
        StartCoroutine(ProcessSpawn());
    }

    private IEnumerator ProcessSpawn()
    {
        yield return new WaitForSeconds(stepTime);

        DropGems();
        SpawnGems();
    }

    private void DropGems()
    {
        var speed = dropSpeed * transform.lossyScale.x;

        for (int j = 0; j < _grid.GetWidth(); j++) {
            var emptyBelow = 0;
            for (int i = _grid.GetHeight() - 1; i >= 0; i--) {
                var gem = _grid.GetValue(i, j);
                if ( !gem ) {
                    emptyBelow++;
                }
                else {
                    if ( emptyBelow > 0 ) {
                        gem.SetPosition(_grid.GetWorldPositionFromIndex(new MatrixIndex(i + emptyBelow, j)),
                            speed);
                        _grid.SetValue(i + emptyBelow, j, gem);
                        _grid.SetValue(i, j, null);
                    }
                }
            }
        }
    }

    private void SpawnGems()
    {
        var speed = dropSpeed * transform.lossyScale.x;

        var lastGemI = _grid.GetHeight();
        var lastGemJ = 0;

        for (int j = 0; j < _grid.GetWidth(); j++) {
            var emptyBelow = 0;
            for (int i = _grid.GetHeight() - 1; i >= 0; i--) {
                var gem = _grid.GetValue(i, j);
                if ( !gem ) {
                    emptyBelow++;
                }
            }

            for (int k = 0; k < emptyBelow; k++) {
                var gemPrefab = availableGems[Random.Range(0, availableGems.Count)];
                // spawn above the grid
                var gemLine = k;
                var spawnLine = -2 - (emptyBelow - (k + 1));

                var newGem = SpawnPrefab(gemPrefab, _grid.IndexToPosition(spawnLine, j));
                _grid.SetValue(gemLine, j, newGem);
                newGem.SetPosition(_grid.GetWorldPositionFromIndex(new MatrixIndex(gemLine, j)), speed);

                // track most distant gem
                if ( k < lastGemI ) {
                    lastGemI = k;
                    lastGemJ = j;
                }
            }
        }

        _grid.GetValue(lastGemI, lastGemJ).OnReachedDestination += OnGemReachedDestination;
    }

    private void OnGemReachedDestination(Gem gem)
    {
        gem.OnReachedDestination -= OnGemReachedDestination;
        CheckForMatches();
    }

    private void CheckForMatches()
    {
        var matches = new List<Match>();

        for (int i = 0; i < _grid.GetHeight(); i++) {
            for (int j = 0; j < _grid.GetWidth(); j++) {
                var index = new MatrixIndex(i, j);

                var gem = _grid.GetValue(index);
                var rowMach = _finder.GetRowMatch(index, gem);
                var columnMatch = _finder.GetColumnMatch(index, gem);

                if ( rowMach.Count() > 0 ) {
                    if ( !matches.Exists(match => match == rowMach) ) {
                        matches.Add(rowMach);
                    }
                }

                if ( columnMatch.Count() > 0 ) {
                    if ( !matches.Exists(match => match == columnMatch) ) {
                        matches.Add(columnMatch);
                    }
                }
            }
        }

        if ( matches.Count > 0 ) {
            ClearMatches(matches);
        }
        else {
            if ( !_finder.HasPossibleMatch() ) {
                ShuffleGrid();
            }

            SetBusy(false);
        }
    }

    private void ShuffleGrid()
    {
        var speed = dropSpeed * transform.lossyScale.x;

        DefineNewPositions();

        for (int i = 0; i < _grid.GetHeight(); i++) {
            for (int j = 0; j < _grid.GetWidth(); j++) {
                _grid.GetValue(i, j).SetPosition(_grid.GetWorldPositionFromIndex(new MatrixIndex(i, j)), speed);
            }
        }

        StartCoroutine(PlayShuffleSound());
    }

    private IEnumerator PlayShuffleSound()
    {
        _shuffleSound.Play();
        yield return new WaitForSeconds(shuffleSoundDuration);
        _shuffleSound.Stop();
    }

    private void DefineNewPositions()
    {
        var watchdog = 0;

        do {
            var gemsOnGrid = new List<Gem>();

            for (int i = 0; i < _grid.GetHeight(); i++) {
                for (int j = 0; j < _grid.GetWidth(); j++) {
                    gemsOnGrid.Add(_grid.GetValue(i, j));
                    _grid.SetValue(i, j, null);
                }
            }


            for (int i = 0; i < _grid.GetHeight(); i++) {
                for (int j = 0; j < _grid.GetWidth(); j++) {
                    Gem gem;
                    do {
                        gem = gemsOnGrid[Random.Range(0, gemsOnGrid.Count)];
                    } while (_finder.WouldCreateMatch(new MatrixIndex(i, j), gem));

                    _grid.SetValue(i, j, gem);
                    gemsOnGrid.Remove(gem);
                }
            }

            watchdog++;
        } while (!_finder.HasPossibleMatch() && watchdog < 50);
    }

    private void InitialSpawn()
    {
        DefineInitialPositions();

        for (int i = 0; i < _grid.GetHeight(); i++) {
            for (int j = 0; j < _grid.GetWidth(); j++) {
                var newGem = SpawnPrefab(_grid.GetValue(i, j), _grid.IndexToPosition(i, j));
                _grid.SetValue(i, j, newGem);
            }
        }
    }

    private void DefineInitialPositions()
    {
        do {
            for (int i = 0; i < _grid.GetHeight(); i++) {
                for (int j = 0; j < _grid.GetWidth(); j++) {
                    Gem gemPrefab;

                    do {
                        gemPrefab = availableGems[Random.Range(0, availableGems.Count)];
                    } while (_finder.WouldCreateMatch(new MatrixIndex(i, j), gemPrefab));

                    _grid.SetValue(i, j, gemPrefab);
                }
            }
        } while (!_finder.HasPossibleMatch());
    }

    private Gem SpawnPrefab(Gem prefab, Vector2 localPosition)
    {
        var newGem = Instantiate(prefab, transform);
        var gemTransform = newGem.transform;
        gemTransform.SetParent(transform);
        gemTransform.localPosition = localPosition;
        return newGem;
    }

    public void SetBusy(bool busy)
    {
        _busy = busy;

        if ( !_busy ) {
            OnNotBusy?.Invoke();
        }
    }

    public bool GetBusy()
    {
        return _busy;
    }

    public void ClearGemSelection()
    {
        _selectedIndex = new MatrixIndex(-1, -1);
    }

    private bool HasSelectedGem()
    {
        return _selectedIndex.i >= 0 && _selectedIndex.j >= 0;
    }
}