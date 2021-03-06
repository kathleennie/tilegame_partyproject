﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Variables
    private static GameManager m_Singleton;
    public static GameManager GetSingleton() {
        return m_Singleton;
    }

    public static int currentPlayer = 1;
    public static bool actionInProcess;

    [SerializeField]
    private Button m_attackButton;

    [SerializeField]
    private GameObject[] tilePrefabs;

    [SerializeField]
    GameObject testCharacter;

    List<Character> player1Units;
    List<Character> player2Units;

    TileBehavior[,] mapArray;
        float tileSize;
    #endregion

    #region Initialization
    public void Awake() {
        // Singleton makes sure there is only one of this object
        if (m_Singleton != null) {
            DestroyImmediate(gameObject);
            return;
        }
        m_Singleton = this;

        player1Units = new List<Character>();
        player2Units = new List<Character>();

        m_attackButton.onClick.AddListener(PressAttackButton);

        tileSize = tilePrefabs[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        CreateTiles();
    }

    public void Start() {
        // FOR TESTING PURPOSES
        PlaceCharacterOnTile(testCharacter, 0, 1, 1);
    }
    #endregion

    #region Set Up
    public void CreateTiles() {

        string[] mapData = ReadLevelText();

        int mapXSize = mapData[0].ToCharArray().Length;
        int mapYSize = mapData.Length;

        // Fill mapArray, which should be empty at first.
        mapArray = new TileBehavior[mapXSize, mapYSize];

        // Calculate the size of the map.
        float mapWidth = mapXSize * tileSize;
        float mapHeight = mapYSize * tileSize;

        // Finds the top left corner.
        Vector3 worldStart = new Vector3(-mapWidth / 2.0f + (0.5f * tileSize), mapHeight / 2.0f - (0.5f * tileSize));

        // Nested for loop that creates mapYSize * mapXSize tiles.
        for (int y = 0; y < mapYSize; y++) {
            char[] newTiles = mapData[y].ToCharArray();
            for (int x = 0; x < mapXSize; x++) {
                PlaceTile(newTiles[x].ToString(), x, y, worldStart);
            }
        }

        for (int y = 0; y < mapYSize; y++) {
            for (int x = 0; x < mapXSize; x++) {
                if (x - 1 >= 0) {
                    mapArray[x, y].Left = mapArray[x - 1, y];
                }
                if (x + 1 < mapXSize) {
                    mapArray[x, y].Right = mapArray[x + 1, y];
                }
                if (y + 1 < mapYSize) {
                    mapArray[x, y].Down = mapArray[x, y + 1];
                }
                if (y - 1 >= 0) {
                    mapArray[x, y].Up = mapArray[x, y - 1];
                }
            }
        }
    }

    // Places a tile at position (x, y).
    private void PlaceTile(string tileType, int x, int y, Vector3 worldStart) {
        int tileIndex = int.Parse(tileType);

        // Creates a new tile instance.
        GameObject newTile = Instantiate(tilePrefabs[tileIndex]);

        // Calculates where it should go.
        float newX = worldStart.x + (tileSize * x);
        float newY = worldStart.y - (tileSize * y);

        // Puts it there.
        newTile.transform.position = new Vector3(newX, newY, 0);
        newTile.GetComponent<TileBehavior>().xPosition = x;
        newTile.GetComponent<TileBehavior>().yPosition = y;

        // Adds it to mapArray so we can keep track of it later.
        mapArray[x, y] = newTile.GetComponent<TileBehavior>();
    }

    private string[] ReadLevelText() {
        TextAsset bindData = Resources.Load("Test") as TextAsset;
        string data = bindData.text.Replace("\r\n", string.Empty);
        return data.Split('-');
    }

    void PlaceCharacterOnTile(GameObject unit, int x, int y, int player) {
        // Instantiate an instance of the unit and place it on the given tile.
        Character newUnit = Instantiate(unit).GetComponent<Character>();
        newUnit.SetPlayer(player);
        newUnit.SetHPFull();
        mapArray[x, y].transform.GetComponent<TileBehavior>().PlaceUnit(newUnit);

        // Put the unit in the right player's array.
        if (player == 1) {
            player1Units.Add(newUnit);

        }
        else if (player == 2) {
            player2Units.Add(newUnit);
        }
    }
    #endregion

    #region UI
    public void ShowCharacterUI(Character selectedUnit) {
    }

    public void ClearUI() {

    }

    public void PressAttackButton() {
        if (TileBehavior.selectedUnit != null && TileBehavior.selectedUnit.GetComponent<Character>().GetCanAttack()) {
            TileBehavior.AttackSelection();
        }
    }

    public void PressEndTurnButton() {
        //For every character in Player 1, set can move and can attack.
        foreach (Character unit in player1Units) {
            unit.SetCanMove(true);
            unit.SetCanAttack(true);
        }
        //For every character in Player 2/Enemy, set can move and can attack.
        foreach (Character unit in player2Units) {
            unit.SetCanMove(true);
            unit.SetCanAttack(true);
        }
        //Reset selection state.
        if (TileBehavior.GetSelectionState() != null) {
            TileBehavior.selectedTile.GetComponent<TileBehavior>().SelectionStateToNull();
        }
    }
    #endregion
}
