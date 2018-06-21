using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int mapSeed;
    public int rows;
    public int columns;
    public GameObject[] gridPrefabs;
    public bool isMapOfTheDay;
    public bool isRandomMap;

    private float roomWidth = 50.0f;
    private float roomHeight = 50.0f;
    private Room[,] grid;
    private string mapTypeKey = "MapType";

    // Use this for initialization
    void Start ()
    {
        if (PlayerPrefs.HasKey(mapTypeKey))
        {
            if (PlayerPrefs.GetString(mapTypeKey) == "Random Map")
            {
                isMapOfTheDay = false;
                isRandomMap = true;
            }
            else if (PlayerPrefs.GetString(mapTypeKey) == "Map Of The Day")
            {
                isMapOfTheDay = true;
                isRandomMap = false;
            }
        }

        if (isMapOfTheDay)
        {
            mapSeed = DateToInt(DateTime.Now.Date);
        }
        else if (isRandomMap)
        {
            mapSeed = DateToInt(DateTime.Now);
        }
        else if (mapSeed == 0)
        {
            mapSeed = DateToInt(DateTime.Now);
        }

        //Generate Grid
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        //Set our seed

        UnityEngine.Random.InitState(mapSeed);

        //Clear out the grid - "which column" is our X, "which row" is our Y
        grid = new Room[columns, rows];

        //For each grid row...
        for (int row = 0; row < rows; row++)
        {
            //for each column in that row
            for (int column = 0; column < columns; column++)
            {
                //Figure out the location
                float xPosition = roomWidth * column;
                float zPosition = roomHeight * row;
                Vector3 newPosition = new Vector3(xPosition, 0.0f, zPosition);

                //Create a new grid at the appropriate location
                GameObject tempRoomObj = Instantiate(RandomRoomPrefab(), newPosition, Quaternion.identity) as GameObject;

                //Set its parent
                tempRoomObj.transform.parent = this.transform;

                //Give it a meaningful name
                tempRoomObj.name = "Room_" + column + "," + row;

                //Get the room object
                Room tempRoom = tempRoomObj.GetComponent<Room>();

                //Open the doors
                //If we are on the bottom row, open the north door
                if(row == 0)
                {
                    tempRoom.doorNorth.SetActive(false);
                }
                else if (row == rows - 1)
                {
                    //Otherwise, if we are on the top row, open the south door
                    tempRoom.doorSouth.SetActive(false);
                }
                else
                {
                    //otherwise, we are in the middle, so open both doors
                    tempRoom.doorNorth.SetActive(false);
                    tempRoom.doorSouth.SetActive(false);
                }

                //If we are on the first column, open the east door
                if (column == 0)
                {
                    tempRoom.doorEast.SetActive(false);
                }
                else if (column == columns - 1)
                {
                    //Otherwise, if we are on the last column row, open the west door
                    tempRoom.doorWest.SetActive(false);
                }
                else
                {
                    //Otherwise, we are in the middle, so open both doors
                    tempRoom.doorEast.SetActive(false);
                    tempRoom.doorWest.SetActive(false);
                }

                //Save it the grid array
                grid[column, row] = tempRoom;
            }
        }
    }

    //Returns a random room
    public GameObject RandomRoomPrefab()
    {
        return gridPrefabs[UnityEngine.Random.Range(0, gridPrefabs.Length)];
    }

    public int DateToInt(DateTime dateToUse)
    {
        //Add our date up and return it
        return dateToUse.Year + dateToUse.Month + dateToUse.Day + dateToUse.Hour + dateToUse.Minute + dateToUse.Second + dateToUse.Millisecond;
    }
}
