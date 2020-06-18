using System.Collections.Generic;
using UnityEngine;

public class MatchFinder : MonoBehaviour
{
    private GemGrid _grid;

    private void Start()
    {
        _grid = GetComponent<GemGrid>();
    }

    public bool WouldCreateMatch(MatrixIndex index, Gem gem)
    {
        return GetColumnMatch(index, gem).Count() > 0 || GetRowMatch(index, gem).Count() > 0;
    }

    public Match GetColumnMatch(MatrixIndex index, Gem gem)
    {
        var matchIndexes = new List<MatrixIndex> {index};

        matchIndexes.AddRange(GetColumnDownGems(index, gem));
        matchIndexes.AddRange(GetColumnUpGems(index, gem));

        if ( matchIndexes.Count < 3 ) {
            matchIndexes.Clear();
        }

        return new Match(matchIndexes);
    }

    public Match GetRowMatch(MatrixIndex index, Gem gem)
    {
        var matchIndexes = new List<MatrixIndex> {index};

        matchIndexes.AddRange(GetRowRightGems(index, gem));
        matchIndexes.AddRange(GetRowLeftGems(index, gem));

        if ( matchIndexes.Count < 3 ) {
            matchIndexes.Clear();
        }

        return new Match(matchIndexes);
    }

    public bool HasPossibleMatch()
    {
        for (int i = 0; i < _grid.GetHeight(); i++) {
            for (int j = 0; j < _grid.GetWidth(); j++) {
                if ( CheckLShapedPossibilities(i, j) ||
                     CheckVShapedPossibilities(i, j) ||
                     CheckLineShapedPossibilities(i, j) ) {
                    return true;
                }
            }
        }

        return false;
    }

    private bool CheckLineShapedPossibilities(int i, int j)
    {
        var gem = _grid.GetValue(i, j);

        if ( i >= 3 ) {
            if ( _grid.GetValue(i - 1, j).CompareTag(gem.tag) && _grid.GetValue(i - 3, j).CompareTag(gem.tag) ) {
                return true;
            }
        }

        if ( i < _grid.GetHeight() - 3 ) {
            if ( _grid.GetValue(i + 1, j).CompareTag(gem.tag) && _grid.GetValue(i + 3, j).CompareTag(gem.tag) ) {
                return true;
            }
        }

        if ( j >= 3 ) {
            if ( _grid.GetValue(i, j - 1).CompareTag(gem.tag) && _grid.GetValue(i, j - 3).CompareTag(gem.tag) ) {
                return true;
            }
        }

        if ( j < _grid.GetWidth() - 3 ) {
            if ( _grid.GetValue(i, j + 1).CompareTag(gem.tag) && _grid.GetValue(i, j + 3).CompareTag(gem.tag) ) {
                return true;
            }
        }

        return false;
    }

    private bool CheckLShapedPossibilities(int i, int j)
    {
        var gem = _grid.GetValue(i, j);

        if ( i >= 2 ) {
            if ( _grid.GetValue(i - 1, j).CompareTag(gem.tag) ) {
                if ( j >= 1 ) {
                    if ( _grid.GetValue(i - 2, j - 1).CompareTag(gem.tag) ) {
                        return true;
                    }
                }

                if ( j < _grid.GetWidth() - 1 ) {
                    if ( _grid.GetValue(i - 2, j + 1).CompareTag(gem.tag) ) {
                        return true;
                    }
                }
            }
        }

        if ( i < _grid.GetHeight() - 2 ) {
            if ( _grid.GetValue(i + 1, j).CompareTag(gem.tag) ) {
                if ( j >= 1 ) {
                    if ( _grid.GetValue(i + 2, j - 1).CompareTag(gem.tag) ) {
                        return true;
                    }
                }

                if ( j < _grid.GetWidth() - 1 ) {
                    if ( _grid.GetValue(i + 2, j + 1).CompareTag(gem.tag) ) {
                        return true;
                    }
                }
            }
        }

        if ( j >= 2 ) {
            if ( _grid.GetValue(i, j - 1).CompareTag(gem.tag) ) {
                if ( i >= 1 ) {
                    if ( _grid.GetValue(i - 1, j - 2).CompareTag(gem.tag) ) {
                        return true;
                    }
                }

                if ( i < _grid.GetHeight() - 1 ) {
                    if ( _grid.GetValue(i + 1, j - 2).CompareTag(gem.tag) ) {
                        return true;
                    }
                }
            }
        }

        if ( j < _grid.GetWidth() - 2 ) {
            if ( _grid.GetValue(i, j + 1).CompareTag(gem.tag) ) {
                if ( i >= 1 ) {
                    if ( _grid.GetValue(i - 1, j + 2).CompareTag(gem.tag) ) {
                        return true;
                    }
                }

                if ( i < _grid.GetHeight() - 1 ) {
                    if ( _grid.GetValue(i + 1, j + 2).CompareTag(gem.tag) ) {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool CheckVShapedPossibilities(int i, int j)
    {
        var gem = _grid.GetValue(i, j);

        if ( i >= 2 ) {
            if ( _grid.GetValue(i - 2, j).CompareTag(gem.tag) ) {
                if ( j >= 1 ) {
                    if ( _grid.GetValue(i - 1, j - 1).CompareTag(gem.tag) ) {
                        return true;
                    }
                }

                if ( j < _grid.GetWidth() - 1 ) {
                    if ( _grid.GetValue(i - 1, j + 1).CompareTag(gem.tag) ) {
                        return true;
                    }
                }
            }
        }

        if ( i < _grid.GetHeight() - 2 ) {
            if ( _grid.GetValue(i + 2, j).CompareTag(gem.tag) ) {
                if ( j >= 1 ) {
                    if ( _grid.GetValue(i + 1, j - 1).CompareTag(gem.tag) ) {
                        return true;
                    }
                }

                if ( j < _grid.GetWidth() - 1 ) {
                    if ( _grid.GetValue(i + 1, j + 1).CompareTag(gem.tag) ) {
                        return true;
                    }
                }
            }
        }

        if ( j >= 2 ) {
            if ( _grid.GetValue(i, j - 2).CompareTag(gem.tag) ) {
                if ( i >= 1 ) {
                    if ( _grid.GetValue(i - 1, j - 1).CompareTag(gem.tag) ) {
                        return true;
                    }
                }

                if ( i < _grid.GetHeight() - 1 ) {
                    if ( _grid.GetValue(i + 1, j - 1).CompareTag(gem.tag) ) {
                        return true;
                    }
                }
            }
        }

        if ( j < _grid.GetWidth() - 2 ) {
            if ( _grid.GetValue(i, j + 2).CompareTag(gem.tag) ) {
                if ( i >= 1 ) {
                    if ( _grid.GetValue(i - 1, j + 1).CompareTag(gem.tag) ) {
                        return true;
                    }
                }

                if ( i < _grid.GetHeight() - 1 ) {
                    if ( _grid.GetValue(i + 1, j + 1).CompareTag(gem.tag) ) {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public List<MatrixIndex> GetColumnDownGems(MatrixIndex index, Gem gem)
    {
        var gems = new List<MatrixIndex>();

        while (index.i < _grid.GetHeight() - 1) {
            index.i++;
            var testGem = _grid.GetValue(index);
            if ( !testGem || !testGem.CompareTag(gem.tag) ) {
                break;
            }

            gems.Add(index);
        }

        return gems;
    }

    public List<MatrixIndex> GetColumnUpGems(MatrixIndex index, Gem gem)
    {
        var gems = new List<MatrixIndex>();

        while (index.i > 0) {
            index.i--;
            var testGem = _grid.GetValue(index);
            if ( !testGem || !testGem.CompareTag(gem.tag) ) {
                break;
            }

            gems.Add(index);
        }

        return gems;
    }

    public List<MatrixIndex> GetRowRightGems(MatrixIndex index, Gem gem)
    {
        var gems = new List<MatrixIndex>();

        while (index.j < _grid.GetWidth() - 1) {
            index.j++;
            var testGem = _grid.GetValue(index);
            if ( !testGem || !testGem.CompareTag(gem.tag) ) {
                break;
            }

            gems.Add(index);
        }

        return gems;
    }

    public List<MatrixIndex> GetRowLeftGems(MatrixIndex index, Gem gem)
    {
        var gems = new List<MatrixIndex>();

        while (index.j > 0) {
            index.j--;
            var testGem = _grid.GetValue(index);
            if ( !testGem || !testGem.CompareTag(gem.tag) ) {
                break;
            }

            gems.Add(index);
        }

        return gems;
    }
}