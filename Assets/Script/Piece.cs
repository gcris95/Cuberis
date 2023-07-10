using System.Collections;
using UnityEngine;

public class Piece : MonoBehaviour
{
    private float timer;
    private float timerMoving;
    private bool isFrozen = false;
    private IEnumerator freezing;

    private Color originalColor;
    private Color startColor;
    private Color endColor;
    private float lastColorChangeTime;

    private Material material;
    private bool changing = true;
    private float fallInterval;

    private void Awake()
    {
        if (!checkPosition())
        {
            transform.SetParent(GameObject.FindGameObjectWithTag("FallenPieces").transform, true);
            FindObjectOfType<ControlManager>().gameOver();
        }

        startColor = GetComponentInChildren<Renderer>().material.color;
        endColor = Color.black;
        originalColor = new Color(startColor.r, startColor.g, startColor.b);
        freezing = tooLate();
        timer = 0;
    }
    
    private void Update()
    {
        timer += Time.deltaTime;
        timerMoving += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            rotate();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || (Input.GetKey(KeyCode.LeftArrow) && timer >= 0.1))
        {
            translateLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || (Input.GetKey(KeyCode.RightArrow) && timer >= 0.1))
        {
            translateRight();
        }

        fallInterval = FindObjectOfType<ControlManager>().fallInterval;
        if ((Input.GetKeyDown(KeyCode.DownArrow) || timer >= fallInterval || (Input.GetKey(KeyCode.DownArrow) && timer >= 0.1) || timerMoving >= fallInterval) && !isFrozen)
        {
            translateDown();
        }

        if (changing)
        {
            float ratio = (Time.time - lastColorChangeTime) / 0.5f;
            ratio = Mathf.Clamp01(ratio);

            foreach (Transform child in transform)
                child.GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, ratio);

            if (ratio == 1f)
            {
                lastColorChangeTime = Time.time;

                // Switch colors
                Color temp = startColor;
                startColor = endColor;
                endColor = temp;

            }
        }
    }
    
    //public void setTransparency(float value)
    //{
    //    startColor.a = value;
    //    endColor.a = value;
    //    originalColor.a = value;
    //    //foreach (Transform child in transform)
    //    //{
    //    //    Material r = child.GetComponent<Renderer>().material;
    //    //    Color color = r.GetColor("_Color");
    //    //    color.a = value;
    //    //    r.SetColor("_Color", color);
    //    //}
    //}

    public void translateLeft()
    {
        transform.position += new Vector3(-1, 0, 0);
        if (!isFrozen)
        {
            if (checkPosition())
                updateGrid();
            else
                transform.position += new Vector3(1, 0, 0);
        }
        else
        {
            if (!checkBorders())
                transform.position += new Vector3(1, 0, 0);

            if (checkPosition())
            {
                isFrozen = false;
                transform.position += new Vector3(0, 0, 1);
                //setTransparency(1);
                updateGrid();
            }
        }

        timer = 0;
    }

    public void translateRight()
    {
        transform.position += new Vector3(1, 0, 0);
        if (!isFrozen)
        {
            if (checkPosition())
                updateGrid();
            else
                transform.position += new Vector3(-1, 0, 0);
        }
        else
        {
            if (!checkBorders())
                transform.position += new Vector3(-1, 0, 0);

            if (checkPosition())
            {
                transform.position += new Vector3(0, 0, 1);
                isFrozen = false;
                //setTransparency(1);
                updateGrid();
            }
        }

        timer = 0;
    }

    public void translateDown()
    {
        transform.position += new Vector3(0, -1, 0);
        if (checkPosition())
        {
            updateGrid();
        }

        else
        {
            changing = false;
            foreach (Transform child in transform)
            {
                child.GetComponent<Renderer>().material.color = originalColor;
            }
            transform.position += new Vector3(0, 1, 0);
            updateGrid();
            Transform t = transform;
            Grid.deleteFullRows(ref t);
            Spawner s = FindObjectOfType<Spawner>();
            if (s != null)
                s.SpawnPiece();
            transform.SetParent(GameObject.FindGameObjectWithTag("FallenPieces").transform, true);
            enabled = false;
        }
        timer = 0;
        timerMoving = 0;
    }

    public void rotate()
    {
        transform.rotation *= MyQuaternion.Euler(0, 0, -90);

        if (!isFrozen)
        {
            if (checkPosition())
            {
                updateGrid();
            }
            else
                transform.rotation *= MyQuaternion.Euler(0, 0, 90);
        }
        else
        {
            if (!checkBorders())
                transform.rotation *= MyQuaternion.Euler(0, 0, 90);

            if (checkPosition())
            {
                isFrozen = false;
                transform.position += new Vector3(0, 0, 1);
                //setTransparency(1);
                updateGrid();
            }
        }
        timer = 0;
    }

    public void updateGrid()
    {
        int activeGrid = Grid.active_grid;

        //Possible optimization, save the previous position instead of check all the grids?
        switch (activeGrid)
        {
            case 1:
                for (int y = 0; y < Grid.h; ++y)
                {
                    for (int x = 0; x < Grid.w; ++x)
                        if (Grid.grid1[x, y] != null)
                            if (Grid.grid1[x, y].parent == transform)
                                Grid.grid1[x, y] = null;

                    if (Grid.grid4[6, y] != null)
                        if (Grid.grid4[6, y].parent == transform)
                            Grid.grid4[6, y] = null;

                    if (Grid.grid2[0, y] != null)
                        if (Grid.grid2[0, y].parent == transform)
                            Grid.grid2[0, y] = null;
                }

                foreach (Transform child in transform)
                {
                    Vector3 v = Grid.roundVec3(child.position);
                    Grid.grid1[(int)v.x, (int)v.y] = child;
                    if (v.x == 0)
                    {
                        Grid.grid4[6, (int)v.y] = child;
                    }
                    else if (v.x == 6)
                    {
                        Grid.grid2[0, (int)v.y] = child;
                    }
                }
                break;

            case 2:
                for (int y = 0; y < Grid.h; ++y)
                {
                    for (int x = 0; x < Grid.w; ++x)
                        if (Grid.grid2[x, y] != null)
                            if (Grid.grid2[x, y].parent == transform)
                                Grid.grid2[x, y] = null;

                    if (Grid.grid1[6, y] != null)
                        if (Grid.grid1[6, y].parent == transform)
                            Grid.grid1[6, y] = null;

                    if (Grid.grid3[0, y] != null)
                        if (Grid.grid3[0, y].parent == transform)
                            Grid.grid3[0, y] = null;
                }

                foreach (Transform child in transform)
                {
                    Vector3 v = Grid.roundVec3(child.position);
                    Grid.grid2[(int)v.x, (int)v.y] = child;
                    if (v.x == 0)
                    {
                        Grid.grid1[6, (int)v.y] = child;
                    }
                    else if (v.x == 6)
                    {
                        Grid.grid3[0, (int)v.y] = child;
                    }
                }
                break;

            case 3:
                for (int y = 0; y < Grid.h; ++y)
                {
                    for (int x = 0; x < Grid.w; ++x)
                        if (Grid.grid3[x, y] != null)
                            if (Grid.grid3[x, y].parent == transform)
                                Grid.grid3[x, y] = null;

                    if (Grid.grid2[6, y] != null)
                        if (Grid.grid2[6, y].parent == transform)
                            Grid.grid2[6, y] = null;

                    if (Grid.grid4[0, y] != null)
                        if (Grid.grid4[0, y].parent == transform)
                            Grid.grid4[0, y] = null;
                }

                foreach (Transform child in transform)
                {
                    Vector3 v = Grid.roundVec3(child.position);
                    Grid.grid3[(int)v.x, (int)v.y] = child;
                    if (v.x == 0)
                    {
                        Grid.grid2[6, (int)v.y] = child;
                    }
                    else if (v.x == 6)
                    {
                        Grid.grid4[0, (int)v.y] = child;
                    }
                }
                break;

            case 4:
                for (int y = 0; y < Grid.h; ++y)
                {
                    for (int x = 0; x < Grid.w; ++x)
                        if (Grid.grid4[x, y] != null)
                            if (Grid.grid4[x, y].parent == transform)
                                Grid.grid4[x, y] = null;

                    if (Grid.grid3[6, y] != null)
                        if (Grid.grid3[6, y].parent == transform)
                            Grid.grid3[6, y] = null;

                    if (Grid.grid1[0, y] != null)
                        if (Grid.grid1[0, y].parent == transform)
                            Grid.grid1[0, y] = null;
                }

                foreach (Transform child in transform)
                {
                    Vector3 v = Grid.roundVec3(child.position);
                    Grid.grid4[(int)v.x, (int)v.y] = child;
                    if (v.x == 0)
                    {
                        Grid.grid3[6, (int)v.y] = child;
                    }
                    else if (v.x == 6)
                    {
                        Grid.grid1[0, (int)v.y] = child;
                    }
                }
                break;
        }
    }

    public bool checkBorders()
    {
        foreach (Transform child in transform)
        {
            Vector3 v = Grid.roundVec3(child.position);

            if (!Grid.verifyPosition(v))
                return false;
        }
        return true;
    }

    public bool checkPosition()
    {
        foreach (Transform child in transform)
        {
            Vector3 v = Grid.roundVec3(child.position);
            int activeGrid = Grid.active_grid;

            // Not inside Border?
            if (!Grid.verifyPosition(v))
                return false;

            switch (activeGrid)
            {
                case 1:
                    if (Grid.grid1[(int)v.x, (int)v.y] != null && Grid.grid1[(int)v.x, (int)v.y].parent != transform)
                        return false;
                    break;

                case 2:
                    if (Grid.grid2[(int)v.x, (int)v.y] != null && Grid.grid2[(int)v.x, (int)v.y].parent != transform)
                        return false;
                    break;

                case 3:
                    if (Grid.grid3[(int)v.x, (int)v.y] != null && Grid.grid3[(int)v.x, (int)v.y].parent != transform)
                        return false;
                    break;

                case 4:
                    if (Grid.grid4[(int)v.x, (int)v.y] != null && Grid.grid4[(int)v.x, (int)v.y].parent != transform)
                        return false;
                    break;
            }
        }

        return true;
    }

    public void setFreeze(bool state)
    {
        isFrozen = state;
        //if (state)
        //    StartCoroutine(freezing);
        //else
        //    StopCoroutine(freezing);
    }

    public bool getFreeze()
    {
        return isFrozen;
    }

    public void removeForNextGrid(int grid)
    {
        switch (grid)
        {
            case 1:
                foreach (Transform child in transform)
                {
                    Vector3 v = Grid.roundVec3(child.position);

                    Grid.grid1[(int)v.x, (int)v.y] = null;
                    if (v.x == 0)
                        Grid.grid4[6, (int)v.y] = null;
                    else if (v.x == 6)
                        Grid.grid2[0, (int)v.y] = null;
                }
                break;

            case 2:
                foreach (Transform child in transform)
                {
                    Vector3 v = Grid.roundVec3(child.position);

                    Grid.grid2[(int)v.x, (int)v.y] = null;
                    if (v.x == 0)
                        Grid.grid1[6, (int)v.y] = null;
                    else if (v.x == 6)
                        Grid.grid3[0, (int)v.y] = null;
                }
                break;

            case 3:
                foreach (Transform child in transform)
                {
                    Vector3 v = Grid.roundVec3(child.position);

                    Grid.grid3[(int)v.x, (int)v.y] = null;
                    if (v.x == 0)
                        Grid.grid2[6, (int)v.y] = null;
                    else if (v.x == 6)
                        Grid.grid4[0, (int)v.y] = null;
                }
                break;

            case 4:
                foreach (Transform child in transform)
                {
                    Vector3 v = Grid.roundVec3(child.position);

                    Grid.grid4[(int)v.x, (int)v.y] = null;
                    if (v.x == 0)
                        Grid.grid3[6, (int)v.y] = null;
                    else if (v.x == 6)
                        Grid.grid1[0, (int)v.y] = null;
                }
                break;
        }
    }

    private IEnumerator tooLate()
    {
        int i = 5;
        ControlManager cm = FindObjectOfType<ControlManager>();
        while (i > 0)
        {
            cm.CountDown(i);
            i--;
            Debug.Log("i: " + i);
            yield return new WaitForSeconds(1);
        }
        cm.gameOver();
    }   
}