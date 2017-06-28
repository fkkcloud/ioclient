using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

// this is sepcifially for in-game elements!
public class IOGameBehaviour : MonoBehaviour 
{
	private GameState _gameState;
	public GameState GlobalGameState
	{
		get
		{
			if (_gameState == null)
				_gameState = FindObjectOfType<GameState> ();

			return _gameState;
		}
	}

	private SocketIOComponent _socketIO;
	public SocketIOComponent SocketIOComp
	{
		get
		{
			if (_socketIO == null)
				_socketIO = FindObjectOfType<SocketIOComponent> ();

			return _socketIO;
		}
	}

	private MapManager _mapManager;
	public MapManager GlobalMapManager
	{
		get
		{
			if (_mapManager == null)
				_mapManager = FindObjectOfType<MapManager> ();

			return _mapManager;
		}
	}
}