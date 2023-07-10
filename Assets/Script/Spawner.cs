using UnityEngine;
using UnityEngine.UI;
using System.Security.Cryptography;

public class Spawner : MonoBehaviour {
    public Image next;
    public GameObject[] pieces;
    public Sprite[] sprites;
    public int cube_Interval;

    private int cont = 0;
    private GameObject active_Piece;
    private GameObject nextPiece;
    private static readonly RNGCryptoServiceProvider _generator = new RNGCryptoServiceProvider();

    private void OnEnable()
    {
        int r = Random(0, 6);

        nextPiece = pieces[r];
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        active_Piece = Instantiate(nextPiece, transform.position, Quaternion.identity);
        int r;
        cont++;

        if (cont == cube_Interval)
        {
            r = 7;
            cont = 0;
        }
        else    
            r = Random(0, 6);
    
        nextPiece = pieces[r];
        next.GetComponent<Image>().sprite = sprites[r];

    }

    public GameObject GetPiece()
    {
        return active_Piece;
    }

    private int Random(int minimumValue, int maximumValue)
    {
        byte[] randomNumber = new byte[1];
        _generator.GetBytes(randomNumber);

        double asciiValueOfRandomCharacter = System.Convert.ToDouble(randomNumber[0]);
        double multiplier = System.Math.Max(0, (asciiValueOfRandomCharacter / 255d) - 0.00000000001d);
        int range = maximumValue - minimumValue + 1;
        double randomValueInRange = System.Math.Floor(multiplier * range);

        return (int)(minimumValue + randomValueInRange);
    }
}
