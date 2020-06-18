using System.Collections.Generic;

public struct Match
{
    public List<MatrixIndex> indexes;

    public Match(List<MatrixIndex> match)
    {
        indexes = match;
        indexes.Sort();
    }

    public int Count()
    {
        return indexes.Count;
    }

    public static bool operator ==(Match lhs, Match rhs)
    {
        if ( lhs.indexes.Count != rhs.indexes.Count ) {
            return false;
        }

        for (int i = 0; i < lhs.indexes.Count; i++) {
            if ( lhs.indexes[i].i != rhs.indexes[i].i || lhs.indexes[i].j != rhs.indexes[i].j) {
                return false;
            }
        }

        return true;
    }

    public static bool operator !=(Match lhs, Match rhs)
    {
        return !(lhs == rhs);
    }
}