﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Character myCharacter;

    private const float m_stepDuration = 0.1f;

    // Start is called before the first frame update
    private void Awake()
    {
        myCharacter = GetComponent<Character>();
    }

    void Update() {
        if (!GameManager.actionInProcess) {
            if (Input.GetKeyDown(KeyCode.W)) {
                StartCoroutine(MoveUnitInDirection("up"));
            }
            else if (Input.GetKeyDown(KeyCode.A)) {
                StartCoroutine(MoveUnitInDirection("left"));
            }
            else if (Input.GetKeyDown(KeyCode.S)) {
                StartCoroutine(MoveUnitInDirection("down"));
            }
            else if (Input.GetKeyDown(KeyCode.D)) {
                StartCoroutine(MoveUnitInDirection("right"));
            }
        }
    }

    #region Movement
    IEnumerator MoveUnitInDirection(string direction) {
        // Action in process!
        GameManager.actionInProcess = true;

        float tileSize = GetComponent<SpriteRenderer>().sprite.bounds.size.x;

        // Calculate the steps you need to take

        //Take that step!
        if (direction.Equals("up")) {
            myCharacter.transform.position += new Vector3(0, tileSize);
            myCharacter.occupiedTile = myCharacter.occupiedTile.Up;
        }
        else if (direction.Equals("right")) {
            myCharacter.transform.position += new Vector3(tileSize, 0);
            myCharacter.occupiedTile = myCharacter.occupiedTile.Right;
        }
        else if (direction.Equals("down")) {
            myCharacter.transform.position -= new Vector3(0, tileSize);
            myCharacter.occupiedTile = myCharacter.occupiedTile.Down;
        }
        else if (direction.Equals("left")) {
            myCharacter.transform.position -= new Vector3(tileSize, 0);
            myCharacter.occupiedTile = myCharacter.occupiedTile.Left;
        }
        myCharacter.RecalculateDepth();
        myCharacter.StartBounceAnimation();
        yield return new WaitForSeconds(m_stepDuration);
        myCharacter.occupiedTile.PlaceUnit(myCharacter);
        myCharacter.SetCanMove(false);

        // Action over!
        GameManager.actionInProcess = false;
    }
    #endregion
}
