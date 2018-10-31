using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour {

    public Transform tile;
    public Transform goalTile;
    public Transform tree;

    private readonly int TILE_LENGTH = 12, MAX_TILES = 10, TILE_DEPTH = 10;
    private readonly float TREE_MAX = 2.5f, TREE_MIN = 1.5f; 
    private readonly int MIN_TREES = 1, MAX_TREES = 3;
    private static System.Random rand = new System.Random();

	// Use this for initialization
	void Start () {

        // Starting from (0,0) on the x,z plane
        // Instantiate a tile
        Transform currTile = Instantiate(tile, new Vector3(0,0,0), Quaternion.identity);
        currTile.transform.localScale = new Vector3(TILE_LENGTH, TILE_DEPTH, TILE_LENGTH);
        currTile.Rotate(0, 30, 0);
        Vector3 currPosition = currTile.transform.position;

        // Expand a random path out from the starting tile
        int tileCount = 1;
        while (tileCount <= MAX_TILES) {

            // Choose a direction to make the next tile in
            int hexEdge = rand.Next(1, 6);

            // Add 60 degrees for each position
            Vector3 nextDirection = new Vector3(Mathf.Cos(Mathf.PI / 3 * hexEdge),
                    0, Mathf.Sin(Mathf.PI / 3 * hexEdge));

            // Elongate the vector to match the length
            nextDirection = 2 * nextDirection.normalized;

            // Calculate position to move to
            Vector3 nextPosition = currPosition + nextDirection;
            currPosition = nextPosition;

            // If tile already exists at position, just mark new position
            // Otherwise, instantiate tile and count it
            if (!isObjectHere(nextPosition))
            {
                if (tileCount == MAX_TILES)
                {
                    // Instantiate goal tile at last location
                    currTile = Instantiate(goalTile, nextPosition, Quaternion.identity);
                } else {
                    // Instantiate normal tiles at intermediate locations
                    currTile = Instantiate(tile, nextPosition, Quaternion.identity);

                    // Drop 1-3 trees on the current tile
                    // Given a central location, 
                    // place trees s.t. all edges are reachable by character
                    // Push towards top and edges of current tile
                    int numTrees = rand.Next(MIN_TREES, MAX_TREES);
                    for (int i = 0; i < numTrees; i++) {

                        // random angle to push tree outwards by
                        float angle = Mathf.Deg2Rad * rand.Next(0, 359);

                        Vector3 pushTree = nextPosition + new Vector3(
                            Mathf.Cos(angle) * .5f, 0, Mathf.Sin(angle) * .5f);

                        pushTree = new Vector3(pushTree.x, .25f, pushTree.z);

                        Transform currTree = Instantiate(tree, pushTree, Quaternion.identity);
                        // Scale slightly
                        float treeRad = Random.Range(TREE_MIN, TREE_MAX);
                        currTree.transform.localScale = new Vector3(
                            treeRad, treeRad, treeRad);
                    }

                }
                currTile.transform.localScale = new Vector3(TILE_LENGTH, TILE_DEPTH, TILE_LENGTH);
                currTile.Rotate(0, 30, 0);
                currPosition = currTile.transform.position;
                tileCount++;
            }

        }

    }

    // Return true if a gameobject exists at position
    private bool isObjectHere(Vector3 position)
    {
        Collider[] intersecting = Physics.OverlapSphere(position, 0.1f);
        return intersecting.Length != 0;
    }

}
