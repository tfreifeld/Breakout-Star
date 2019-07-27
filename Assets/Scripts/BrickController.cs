using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class BrickController : MonoBehaviour
{
    private Tilemap _tilemap;
    private GameManager _gameManager;
    private AudioSource _audioSource;

    private AudioClip _regularBrickClip;
    private AudioClip _strongBrickClip;
    private AudioClip _transparentBrickClip;
    private AudioClip _unbreakableBrickClip;

    private int NumOfUnbreakable { get; set; }

    private void Start()
    {
        _tilemap = GetComponent<Tilemap>();
        _audioSource = GetComponent<AudioSource>();
        _gameManager = FindObjectOfType<GameManager>();

        _regularBrickClip = Resources.Load<AudioClip>("Audio/regularBrick");
        _strongBrickClip = Resources.Load<AudioClip>("Audio/strongBrick");
        _transparentBrickClip = Resources.Load<AudioClip>("Audio/transparentBrick");
        _unbreakableBrickClip = Resources.Load<AudioClip>("Audio/unbreakableBrick");

        NumOfUnbreakable = 0;

        BoundsInt bounds = _tilemap.cellBounds;
        TileBase[] allTiles = _tilemap.GetTilesBlock(bounds);

        foreach (TileBase tile in allTiles)
        {
            if (tile != null)
            {
                if (tile.name == "unbreakable")
                {
                    NumOfUnbreakable++;
                }
            }
        }
    }

    public void HandleBrickCollision(Collision2D collision, float speed)
    {
        HandleCollision(collision, speed);

        Tilemap tilemap = collision.gameObject.GetComponent<Tilemap>();
        int tilesCount = tilemap.GetUsedTilesCount();
        if (tilesCount <= 1)
        {
            if (tilesCount == 1)
            {
                if (NumOfUnbreakable > 0)
                {
                    _gameManager.LevelWon();
                }
            }
            else
            {
                _gameManager.LevelWon();
            }
        }
    }

    private void HandleCollision(Collision2D collision, float speed)
    {
        Vector3 hitPosition = Vector3.zero;

        ContactPoint2D hit = collision.GetContact(0);

        hitPosition.x = hit.point.x - 0.05f * hit.normal.x;
        hitPosition.y = hit.point.y - 0.05f * hit.normal.y;

        Vector3Int tilePosition = _tilemap.WorldToCell(hitPosition);
        TileBase oTile = _tilemap.GetTile(tilePosition);

        if (oTile != null)
        {
            if ((!_gameManager.IsWeakBricks && !_gameManager.IsFireBall) || collision.otherCollider.gameObject.name == "Shot(Clone)")
            {
                HandleRegularCollision(speed, oTile, tilePosition);
            }
            else if (_gameManager.IsWeakBricks)
            {
                HandleWeakBrickCollision(speed, oTile, tilePosition);
            }
            else
            {
                HandleFireBallCollision(speed, oTile, tilePosition);
            }

            PlayAudioClip(oTile.name);
        }


    }

    private void PlayAudioClip(string tileName)
    {
        if (tileName.Contains("_s_"))
        {
            _audioSource.PlayOneShot(_strongBrickClip);
        }
        else if (tileName.Equals("transparent"))
        {
            _audioSource.PlayOneShot(_transparentBrickClip);
        }
        else if (tileName.Equals("unbreakable"))
        {
            _audioSource.PlayOneShot(_unbreakableBrickClip);
        }
        else
        {
            _audioSource.PlayOneShot(_regularBrickClip);
        }
    }

    private void HandleFireBallCollision(float speed, TileBase oTile, Vector3Int tilePosition)
    {
        HandleWeakBrickCollision(speed, oTile, tilePosition);

        HelperFunction(0, 1);
        HelperFunction(1, 0);
        HelperFunction(0, -1);
        HelperFunction(-1, 0);


        void HelperFunction(int i, int j)
        {
            Vector3Int newTilePosition = new Vector3Int(tilePosition.x + i, tilePosition.y + j, tilePosition.z);
            if (_tilemap.HasTile(newTilePosition))
            {
                TileBase tile = _tilemap.GetTile(newTilePosition);
                HandleWeakBrickCollision(speed, tile, newTilePosition);
            }
        }
    }

    private void HandleWeakBrickCollision(float speed, TileBase oTile, Vector3Int tilePosition)
    {
        if (oTile.name == "unbreakable")
        {
            NumOfUnbreakable--;
        }

        int points = 150;
        points += Mathf.RoundToInt(speed) * 2;
        _gameManager.Score += points;
        _tilemap.SetTile(tilePosition, null);

        float rnd = Random.Range(0f, 1f);
        if (rnd > 0.925f)
        {
            _gameManager.MakePowerUp(_tilemap.CellToWorld(tilePosition));
        }
    }

    private void HandleRegularCollision(float speed, TileBase oTile, Vector3Int tilePosition)
    {
        if (oTile.name != "unbreakable")
        {
            TileBase ret = null;
            int points = 150;

            // Strong brick - first hit
            if (oTile.name.Contains("_s_0"))
            {
                points = 100;
                int rndNum = Random.Range(0, 3);
                String newTileName = oTile.name.Replace("_s_0", "_s_b_" + rndNum);
                Tile newTile1 = ScriptableObject.CreateInstance<Tile>();
                newTile1.sprite = Resources.Load<Sprite>("Sprites/" + newTileName);


                ret = newTile1;
            }
            // Transparent brick - first hit
            else if (oTile.name.Contains("transparent"))
            {
                points = 200;
                Tile newTile2 = ScriptableObject.CreateInstance<Tile>();
                newTile2.sprite = Resources.Load<Sprite>("Sprites/" + "transparent");
                ret = newTile2;
            }
            // Strong brick - second hit
            else if (oTile.name.Contains("_b_"))
            {
                points = 200;
            }

            points += Mathf.RoundToInt(speed) * 2;
            _gameManager.Score += points;
            TileBase newTile = ret;
            _tilemap.SetTile(tilePosition, newTile);
            if (ret == null)
            {
                float rnd = Random.Range(0f, 1f);

                if (rnd > 0.925f)
                {
                    _gameManager.MakePowerUp(_tilemap.CellToWorld(tilePosition));
                }
            }
        }
    }
}