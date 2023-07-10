using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
    public static int w = 7;
    public static int h = 17;
    public static Transform[,] grid1 = new Transform[w, h];
    public static Transform[,] grid2 = new Transform[w, h];
    public static Transform[,] grid3 = new Transform[w, h];
    public static Transform[,] grid4 = new Transform[w, h];
    public static int active_grid = 1;

    public static bool verifyPosition(Vector3 pos)
    {
        return ((int)pos.x >= 0 &&
                (int)pos.x < w &&
                (int)pos.y >= 0);

        //return ((int)pos.x >= 0 && (int)pos.x < w && (int)pos.y >= 0 && (int)pos.y<=h);
    }
    
    public static Vector3 roundVec3(Vector3 v)
    {
        return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
    }
    
    public static void deleteRow(int y)
    {
        for (int x = 1; x < w; x++)
        {
            Destroy(grid1[x, y].gameObject);
            Destroy(grid2[x, y].gameObject);
            Destroy(grid3[x, y].gameObject);
            Destroy(grid4[x, y].gameObject);
        }
        for (int x = 0; x < w; x++)
        {
            grid1[x, y] = null;
            grid2[x, y] = null;
            grid3[x, y] = null;
            grid4[x, y] = null;
        }
    }
    
    public static bool isRowFull(int y)
    {
        for (int x = 1; x < w; ++x)
            if (grid1[x, y] == null || grid2[x, y] == null || grid3[x, y] == null || grid4[x, y] == null)
                return false;
        return true;
    }
    
    public static void decreaseRowsAbove(int y)
    {
        for (int i = y; i < h; ++i)
            decreaseRow(i);
    }

    public static void deleteFullRows(ref Transform piece)
    {
        int r = 0;

        foreach(Transform child in piece)
        {
            int y = (int) roundVec3(child.position).y;
            if (isRowFull(y))
            {
                deleteRow(y);
                decreaseRowsAbove(y + 1);
                ++r;
            }
        }

        FindObjectOfType<ControlManager>().givePoints(r);
    }
    
    public static void decreaseRow(int y)
    {
        for (int x = 0; x < w; ++x)
        {
            if (grid1[x, y] != null)
            {
                grid1[x, y - 1] = grid1[x, y];
                grid1[x, y] = null;

                if (x != 0)
                {
                    grid1[x, y - 1].position += new Vector3(0, -1, 0);
                }
                
            }

            if (grid2[x, y] != null)
            {
                grid2[x, y - 1] = grid2[x, y];
                grid2[x, y] = null;

                if (x != 0)
                {
                    grid2[x, y - 1].position += new Vector3(0, -1, 0);
                }
            }

            if (grid3[x, y] != null)
            {
                grid3[x, y - 1] = grid3[x, y];
                grid3[x, y] = null;

                if (x != 0)
                {
                    grid3[x, y - 1].position += new Vector3(0, -1, 0);
                }
            }

            if (grid4[x, y] != null)
            {
                grid4[x, y - 1] = grid4[x, y];
                grid4[x, y] = null;

                if (x != 0)
                {
                    grid4[x, y - 1].position += new Vector3(0, -1, 0);
                }
            }
        }
    }
    
    public static void clean()
    {
        for(int x = 0; x < w; ++x)
            for(int y = 0; y < h; ++y)
            {
                grid1[x, y] = null;
                grid2[x, y] = null;
                grid3[x, y] = null;
                grid4[x, y] = null;
            }
    }
}
