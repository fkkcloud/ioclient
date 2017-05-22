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

	private PlayerController _playerControllerComp;
	public PlayerController PlayerControllerComp
	{
		get
		{
			if (_playerControllerComp == null)
				_playerControllerComp = FindObjectOfType<PlayerController> ();

			return _playerControllerComp;
		}
	}
}