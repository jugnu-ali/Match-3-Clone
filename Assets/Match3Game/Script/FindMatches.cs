using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private List<GameObject> IsAdjacentBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();

        if (dot1.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot1.column,  dot1.row));
        }

        if (dot2.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot2.column, dot2.row));
        }

        if (dot3.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot3.column, dot3.row));
        }

        return currentDots;
    }

    private List<GameObject> IsRowBomb( Dot dot1, Dot dot2, Dot dot3 )
    {
        List<GameObject> currentDots = new List<GameObject>();

        if (dot1.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot1.row));
        }

        if (dot2.GetComponent<Dot>().isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot2.row));
        }

        if (dot3.GetComponent<Dot>().isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot3.row));
        }

        return currentDots;
    }

    private List<GameObject> IsColumnBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();

        if (dot1.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot1.column));
        }

        if (dot2.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot2.column));
        }

        if (dot3.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot3.column));
        }

        return currentDots;
    }

    private void AddToListAndMatch(GameObject dot)
    {
        if (!currentMatches.Contains(dot))
        {
            currentMatches.Add(dot);
        }
        dot.GetComponent<Dot>().isMatched = true;
    }

    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3)
    {
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(0.2f);

        for( int i = 0; i < board.width; ++i)
        {
            for( int j = 0; j < board.height; ++j)
            {
                GameObject currentDot = board.allDots[i, j];
                
                if(currentDot != null)
                {
                    Dot currentDotComponent = currentDot.GetComponent<Dot>();
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftDot = board.allDots[i - 1, j];
                        
                        GameObject rightDot = board.allDots[i + 1, j];
                        

                        if(leftDot != null && rightDot != null)
                        {
                            Dot leftDotComponent = leftDot.GetComponent<Dot>();
                            Dot rightDotComponent = rightDot.GetComponent<Dot>();
                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                            {
                                currentMatches.Union(IsRowBomb(leftDotComponent, currentDotComponent, rightDotComponent));

                                currentMatches.Union(IsColumnBomb(leftDotComponent, currentDotComponent, rightDotComponent));

                                currentMatches.Union(IsAdjacentBomb(leftDotComponent, currentDotComponent, rightDotComponent));

                                GetNearbyPieces(leftDot, currentDot, rightDot);
                            }
                        }
                    }

                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upDot = board.allDots[i, j + 1];
                        GameObject downDot = board.allDots[i, j - 1];

                        if (upDot != null && downDot != null)
                        {
                            Dot upDotComponent = upDot.GetComponent<Dot>();
                            Dot downDotComponent = downDot.GetComponent<Dot>();
                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                            {
                                currentMatches.Union(IsColumnBomb(upDotComponent, currentDotComponent, downDotComponent));

                                currentMatches.Union(IsRowBomb(upDotComponent, currentDotComponent, downDotComponent));

                                currentMatches.Union(IsAdjacentBomb(upDotComponent, currentDotComponent, downDotComponent));

                                GetNearbyPieces(upDot, currentDot, downDot);
                            }
                        }
                    }
                }
            }
        }
    }

    public void MatchPiecesOfColor(string col)
    {
        for(int i = 0; i < board.width; ++i)
        {
            for(int j = 0; j < board.height; ++j)
            {
                // Check if that piece exists
                if(board.allDots[i,j] != null)
                {
                    // Check tha tag on that dot
                    if(board.allDots[i,j].tag == col)
                    {
                        // Set that dot to be matched.
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
    }

    List<GameObject> GetAdjacentPieces(int column, int row)
    {
        List<GameObject> dots = new List<GameObject>();

        for(int  i = column -1; i <= column + 1; ++i)
        {
            for(int  j = row - 1; j <= row+1; ++j)
            {
                // Check if the piece is inside the board.
                if(i >= 0 && i < board.width && j >= 0 && j < board.height)
                {
                    dots.Add(board.allDots[i, j]);
                    board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                }
            }
        }

        return dots;
    }

    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();

        for(int  i = 0; i < board.height; ++i)
        {
            if(board.allDots[column, i] != null)
            {
                dots.Add(board.allDots[column, i]);
                board.allDots[column, i].GetComponent<Dot>().isMatched = true;
            }
        }

        return dots;
    }

    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();

        for (int i = 0; i < board.width; ++i)
        {
            if (board.allDots[i, row] != null)
            {
                dots.Add(board.allDots[i, row]);
                board.allDots[i, row].GetComponent<Dot>().isMatched = true;
            }
        }

        return dots;
    }

    public void CheckBomb()
    {
        // Did the player move something
        if(board.currentDot != null)
        {
            // is the piece moved is matched
            if(board.currentDot.isMatched)
            {
                board.currentDot.isMatched = false;
                // Decide what kind of bomb to make
                /*int typeOfBomb = Random.Range(0, 100);
                if(typeOfBomb < 50)
                {
                    // Make a Row Bomb
                    board.currentDot.MakeRowBomb();
                }
                else
                {
                    // Make a Column Bomb.
                    board.currentDot.MakeColumnBomb();
                }*/

                if((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45)
                    || (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
                {
                    board.currentDot.MakeRowBomb();
                }
                else
                {
                    board.currentDot.MakeColumnBomb();
                }
            }
            // Is the other piece matched ? 
            else if(board.currentDot.otherDot != null)
            {
                Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
                // is the other dot Matched?
                if(otherDot.isMatched)
                {
                    // make it unmatched
                    otherDot.isMatched = false;
                    // Decide what kind of bomb to make.
                    /*int typeOfBomb = Random.Range(0, 100);
                    if (typeOfBomb < 50)
                    {
                        // Make a Row Bomb
                        otherDot.MakeRowBomb();
                    }
                    else
                    {
                        // Make a Column Bomb.
                        otherDot.MakeColumnBomb();
                    }*/

                    if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45)
                    || (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
                    {
                        otherDot.MakeRowBomb();
                    }
                    else
                    {
                        otherDot.MakeColumnBomb();
                    }
                }
            }
        }
    }
}
