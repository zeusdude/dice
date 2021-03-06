﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainView : MonoBehaviour {
	private struct ObjectInMotion {
		public GameObject gameObject;
		public Vector3 destination;
		public float speed;

		public delegate void OnMotionCompleteHandler();
		public OnMotionCompleteHandler OnMotionComplete;
	}

	public GameObject startTile;
	public GameObject pipPrefab;
	public List<GameObject> tilePrefabs;
	public Text scoreText;
	private GameObject _blankPrefabTile;

	private DiceGame.GameController _gameController = new DiceGame.GameController(new DiceGame.GameState(4, 6));
	private GameObject[,] _gameBoardTiles;
	private List<ObjectInMotion> _objectsInMotion = new List<ObjectInMotion>();
	private const float _dieDropSpeed = 1.2f; // Larger is faster
	private Vector3 _tileSize;
	private int _totalScore;
	private int _pendingScore;

	// Use this for initialization
	void Start () {
		_totalScore = 0;
		_pendingScore = 0;

		//
		// Initialize
		SpriteRenderer sr = startTile.GetComponent<SpriteRenderer>();
		_tileSize = sr.bounds.size;

		_blankPrefabTile = tilePrefabs.Find(item => item.name.Equals("b"));
		Debug.Assert(_blankPrefabTile != null);

		//
		// Listen to events from GameController		
		_gameController.ActionEventHandler += _EventCallback;

		_gameBoardTiles = new GameObject[_gameController.state.width, _gameController.state.height];
		//_gameController.Execute(new DiceGame.FillBoardAction(_gameController.testTiles));
		_gameController.Execute(new DiceGame.FillBoardAction(_gameController.pickableTiles));
		_FillBoard();
	}
	
	void Update () {
		if (_objectsInMotion.Count != 0) {
			_MoveObjects();
		} else {
			//
			// Did I hit a tile with a mouse click?
			if (Input.GetMouseButtonDown(0)) {
				RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
				if (hit.collider != null) {
					//Debug.Log("Target Position: " + hit.collider.gameObject.transform.position);
					IntPoint? positionOnGameBoard = _TilePositionOnGameBoard(hit.collider.gameObject);
					if (positionOnGameBoard.HasValue == false) {
						Debug.Log("positionOnGameBoard.HasValue is false - something went wrong.");
					} else {
						//Debug.LogFormat("positionOnGameBoard.Value is {0}", positionOnGameBoard.Value));
						_gameController.Execute(new DiceGame.ActivateTileAction(positionOnGameBoard.Value.x, positionOnGameBoard.Value.y));
					}
				}
			}	
		}
	}
	private void _EmptyBoard() {
		Debug.Log("[_EmptyBoard]");

		for (int col = 0; col < _gameController.state.width; ++col) {
			for (int row = 0; row < _gameController.state.height; ++row) {
				Destroy(_gameBoardTiles[col, row]);
				_gameBoardTiles[col, row] = null;
			}
		}
	}

	private void _FillBoard() {
		Debug.Log("[_FillBoard]");
		Debug.Log(_gameController.state.BoardAsString());

		Vector3 startPosition = startTile.transform.position;

		//
		// Subtract a tileSize.y to the start position since die is positioned above the playing field
		startPosition.y -= _tileSize.y;

		for (int col = 0; col < _gameController.state.width; ++col) {
			for (int row = 0; row < _gameController.state.height; ++row) {
				GameObject prefab = _PrefabForGameBoardPosition(col, row);

				//
				// Fill with prefab or remove a tile that is no longer available...
				if (prefab) {
					Vector3 tilePos = new Vector3(startPosition.x + (col * _tileSize.x), startPosition.y - (row * _tileSize.y), 0);
					_gameBoardTiles[col, row] = Instantiate(prefab, tilePos, Quaternion.identity);					
				} else if (_gameBoardTiles[col, row]) {
					Debug.Assert(_gameController.state[col, row].Equals("e"));
					Destroy(_gameBoardTiles[col, row]);
					_gameBoardTiles[col, row] = null;
				}
			}
		}			
	}

	private void _EventCallback(object sender, EventArgs e)
    {
		Type eventType = e.GetType();

		if (eventType == typeof(DiceGame.ActivateTileEvent))
		{
			_HandleActivateTileEvent((DiceGame.ActivateTileEvent)e);
		}
		else if (eventType == typeof(DiceGame.RefillEvent))
		{
			_HandleRefillEvent((DiceGame.RefillEvent)e);
		} else {
			//Debug.Log("MainView._EventCallback Unknown Event: " + eventType);
			Debug.LogErrorFormat("MainView._EventCallback Unknown Event: {0}", eventType);
		}
    }

	private void _HandleActivateTileEvent(DiceGame.ActivateTileEvent activateTileEvent)
	{
		Debug.LogFormat("_HandleActivateTileEvent");
		activateTileEvent.shootVictimList.ForEach(shootVictimInfo => _ShootVictim(shootVictimInfo));

		if (activateTileEvent.removePipsInfo != null)
			_RemovePips(activateTileEvent.removePipsInfo);

		_pendingScore = activateTileEvent.score;
	}

	private void _HandleRefillEvent(DiceGame.RefillEvent refillEvent)
	{
		Debug.LogFormat("_HandleRefillEvent");

		refillEvent.moveTileList.ForEach(moveTileInfo => _MoveTile(moveTileInfo));
		_ProcessNewTileList(refillEvent.newTileList);
	}

	private IntPoint? _TilePositionOnGameBoard(GameObject tile)
	{
		for (int col = 0; col < _gameController.state.width; ++col) {
			for (int row = 0; row < _gameController.state.height; ++row) {
				if (_gameBoardTiles[col, row] == tile) {
					return new IntPoint(col, row);
				}
			}
		}

		return null;
	}

	private void _MoveObjects() {
		//
		// Move and then remove from list if at destination.  For this to work in a loop
		// you have to iterate in reverse...
		for (int cnt = _objectsInMotion.Count-1; cnt >= 0; --cnt) {
			ObjectInMotion objInMotion = _objectsInMotion[cnt];

			float step = objInMotion.speed * Time.deltaTime;
			objInMotion.gameObject.transform.position = Vector3.MoveTowards(objInMotion.gameObject.transform.position, objInMotion.destination, step);

			if (objInMotion.gameObject.transform.position == objInMotion.destination)
			{
				if (objInMotion.OnMotionComplete != null)
					objInMotion.OnMotionComplete();

				_objectsInMotion.RemoveAt(cnt);
			}
		}

		//
		// We may need a refill now.  I am not sure where else to test for this?  If we do need a refill then
		// new objects will be added to _objectsInMotion.
		if (_objectsInMotion.Count == 0)
		{
			if (_pendingScore != 0)
			{
				_totalScore += _pendingScore;
				_pendingScore = 0;
				scoreText.text = "Score: " + _totalScore.ToString();
			}

			_gameController.Execute(new DiceGame.RefillColumnsAction(_gameController.refillTiles));
		}
	}

	private void _ShootVictim(DiceGame.ActivateTileEvent.ShootVictimInfo shootVictimInfo)
    {
		Debug.LogFormat("_ShootVictim {0},{1} -> {2},{3}",
			shootVictimInfo.shooterCol, shootVictimInfo.shooterRow,
			shootVictimInfo.victimCol, shootVictimInfo.victimRow);

		//
		// Pip animation
		//if (shootVictimEvent.shooterCol != shootVictimEvent.victimCol || shootVictimEvent.shooterRow != shootVictimEvent.victimRow)
		{
			int shooterCol = shootVictimInfo.shooterCol;
			int shooterRow = shootVictimInfo.shooterRow;
			int victimCol = shootVictimInfo.victimCol;
			int victimRow = shootVictimInfo.victimRow;

			GameObject shooterObject = _gameBoardTiles[shooterCol, shooterRow];
			GameObject victimObject = _gameBoardTiles[victimCol, victimRow];
			GameObject pip = Instantiate(this.pipPrefab, shooterObject.transform.position, Quaternion.identity);

			SpriteRenderer victimSprite = pip.GetComponent<SpriteRenderer>();
			SpriteRenderer pipSprite = pip.GetComponent<SpriteRenderer>();
			pipSprite.sortingOrder = victimSprite.sortingOrder + 1;

			ObjectInMotion objInMotion = new ObjectInMotion();

			objInMotion.gameObject = pip;

			objInMotion.destination = new Vector3(
				victimObject.transform.position.x,
				victimObject.transform.position.y,
				victimObject.transform.position.z);
			objInMotion.speed = _dieDropSpeed;

			//
			// The lambda function below does variable capturing to do it's job properly...
			objInMotion.OnMotionComplete = () => {
				Destroy(victimObject);

				GameObject prefab = _PrefabForGameBoardPosition(victimCol, victimRow);
				if (prefab) {
					_gameBoardTiles[victimCol, victimRow] = Instantiate(prefab, _gameBoardTiles[victimCol, victimRow].transform.position, Quaternion.identity);
				} else {
					_gameBoardTiles[victimCol, victimRow] = null;
				}
				
				Destroy(pip);
			};

			_objectsInMotion.Add(objInMotion);
		}
    }

	private void _RemovePips(DiceGame.ActivateTileEvent.RemovePipsInfo removePipsInfo)
    {
		Debug.LogFormat("_RemovePips {0},{1}",
			removePipsInfo.tileCol, removePipsInfo.tileRow);

		//
		// This needs to animate eventually
		GameObject oldObject = _gameBoardTiles[removePipsInfo.tileCol, removePipsInfo.tileRow];
		_gameBoardTiles[removePipsInfo.tileCol, removePipsInfo.tileRow] = 
			Instantiate(_blankPrefabTile, oldObject.transform.position, Quaternion.identity);
		Destroy(oldObject);
    }

	private void _ProcessNewTileList(List<DiceGame.RefillEvent.NewTileInfo> newTileList)
	{
		//
		// Order of operation is important here.  Final position (closest to the top of
		// gameboard) should fall from highest postion.  Currently the RefillEvent
		// stacks things in the correct order...

		//
		// Make an easier to read algorithm by doing a single column at a time
		for (int column = 0; column < _gameController.state.width; ++column)
		{
			int stackPosition = 0;
			newTileList.ForEach( newTileInfo => {
				if (newTileInfo.col == column)
				{
					_NewTile(newTileInfo, stackPosition);
					++stackPosition;
				}
			});
		}
	}

	// stackPosition == 0 is nearest the gameboard (i.e. Zero is bottom of stack)
	private void _NewTile(DiceGame.RefillEvent.NewTileInfo newTileInfo, int stackPosition)
	{
		Debug.LogFormat("_NewTile {0},{1} add {2}",
			newTileInfo.col, newTileInfo.row, newTileInfo.tileValue);

		//
		// Drop new tile from top of screen to new location
		ObjectInMotion objInMotion = new ObjectInMotion();
		GameObject prefab = _PrefabFromDataModelTileName(newTileInfo.tileValue);
		Vector3 initialTilePos = new Vector3(
			startTile.transform.position.x + _tileSize.x * newTileInfo.col,
			startTile.transform.position.y + _tileSize.y * stackPosition,
			startTile.transform.position.z);

		_gameBoardTiles[newTileInfo.col, newTileInfo.row] = Instantiate(prefab, initialTilePos, Quaternion.identity);
		objInMotion.gameObject = _gameBoardTiles[newTileInfo.col, newTileInfo.row];

		objInMotion.destination = new Vector3(
			startTile.transform.position.x + _tileSize.x * newTileInfo.col,
			startTile.transform.position.y - _tileSize.y * (newTileInfo.row + 1),
			startTile.transform.position.z);
		objInMotion.speed = _dieDropSpeed;

		_objectsInMotion.Add(objInMotion);
	}

	private void _MoveTile(DiceGame.RefillEvent.MoveTileInfo moveTileInfo)
	{
		Debug.LogFormat("_MoveTile {0},{1} to {2},{3}",
			moveTileInfo.col, moveTileInfo.fromRow, moveTileInfo.col, moveTileInfo.toRow);

		//
		// Drop tile into new position
		ObjectInMotion objInMotion = new ObjectInMotion();

		_gameBoardTiles[moveTileInfo.col, moveTileInfo.toRow] = _gameBoardTiles[moveTileInfo.col, moveTileInfo.fromRow];
		Debug.Assert(_gameBoardTiles[moveTileInfo.col, moveTileInfo.toRow]);
		_gameBoardTiles[moveTileInfo.col, moveTileInfo.fromRow] = null;

		objInMotion.gameObject = _gameBoardTiles[moveTileInfo.col, moveTileInfo.toRow];

		objInMotion.destination = new Vector3(
			objInMotion.gameObject.transform.position.x,
			objInMotion.gameObject.transform.position.y - _tileSize.y * (moveTileInfo.toRow - moveTileInfo.fromRow),  // Negative moves down
			objInMotion.gameObject.transform.position.z);
		objInMotion.speed = _dieDropSpeed;

		_objectsInMotion.Add(objInMotion);
	}

	private GameObject _PrefabForGameBoardPosition(int col, int row) {
		return _PrefabFromDataModelTileName(_gameController.state[col, row]);
	}

	private GameObject _PrefabFromDataModelTileName(string tileName) {
		for (int cnt = 0; cnt < tilePrefabs.Count; ++cnt) {
			if (tilePrefabs[cnt].name.Equals(tileName)) {
				return tilePrefabs[cnt];
			}
		}

		return null;
	}
}
