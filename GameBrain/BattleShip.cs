﻿using System;
using System.Linq;
using System.Text.Json;
using Domain;
using Domain.Enums;
using Domain.GameBrain;

namespace GameBrain
{
    public class BattleShip
    {
        private CellState[,] _boardA;
        private CellState[,] _boardB;
        private int _boardWidth;
        private int _boardHeight;
        private bool _nextMoveByA = true;
        private bool _placeBoatsByA = true;
        private GameOption _gameOption;
        private GameBoat[] _playerABoats;
        private GameBoat[] _playerBBoats;

        public BattleShip(GameOption option)
        {
            _boardA = new CellState[option.BoardHeight, option.BoardWidth];
            _boardB = new CellState[option.BoardHeight, option.BoardWidth];

            _boardWidth = option.BoardWidth;
            _boardHeight = option.BoardHeight;

            _gameOption = option;
            
            _playerABoats = BuildBoats();
            _playerBBoats = BuildBoats();

        }

        public void PlaceBoat(int boatIndex, int x, int y)
        {
            var boats = _placeBoatsByA ? _playerABoats : _playerBBoats;
            boats[boatIndex].CoordX = x;
            boats[boatIndex].CoordY = y;
        }

        public void RotateBoat(int boatIndex)
        {
            var boats = _placeBoatsByA ? _playerABoats : _playerBBoats;

            boats[boatIndex].Horizontal = !boats[boatIndex].Horizontal;
        }

        public void UpdateBoatsOnBoard()
        {
            var board = new CellState[_gameOption.BoardHeight, _gameOption.BoardWidth];
            var boats = _placeBoatsByA ? _playerABoats : _playerBBoats;

            foreach (var boat in boats)
            {
                if (boat.CoordX >= 0)
                {
                    UpdateBoat(boat, board);
                }

            }
        }

        private void UpdateBoat(GameBoat boat, CellState[,] board)
        {
            for (var i = 0; i < boat.Size; i++)
            {
                if (boat.Horizontal)
                {
                    board[boat.CoordX + i, boat.CoordY] = CellState.Ship;
                }
                else
                {
                    board[boat.CoordX, boat.CoordY + i] = CellState.Ship;
                }
            }
        }

        private GameBoat[] BuildBoats()
        {
            var boatArray = new GameBoat[CountBoatsFromOptions()];

            var i = 0;
            foreach (var boat in _gameOption.Boats)
            {
                for (var j = 0; j < boat.Amount; j++)
                {
                    boatArray[i] = new GameBoat
                    {
                        Size = boat.Size
                    };
                    i++;
                }
            }

            return boatArray;
        }

        private int CountBoatsFromOptions()
        {
            return _gameOption.Boats.Sum(boat => boat.Amount);
        }

        public (CellState[,], CellState[,]) GetBoards()
        {
            var resA = new CellState[_boardHeight,_boardWidth];
            Array.Copy(_boardA, resA, _boardA.Length );
            
            var resB = new CellState[_boardHeight,_boardWidth];
            Array.Copy(_boardB, resB, _boardA.Length );

            return (resA, resB);
        }

        public (GameBoat[], GameBoat[]) GetBoatArrays()
        {
            var resA = new GameBoat[CountBoatsFromOptions()];
            Array.Copy(_playerABoats, resA, _playerABoats.Length );
            
            var resB = new GameBoat[CountBoatsFromOptions()];
            Array.Copy(_playerBBoats, resB, _playerBBoats.Length );

            return (resA, resB);
        }

        public bool MakeAMove(int x, int y)
        {
            var board = _nextMoveByA ? _boardA : _boardB;
            
            switch (board[y, x])
            {
                case CellState.Empty:
                    board[y, x] = CellState.Miss;
                    _nextMoveByA = !_nextMoveByA;
                    return true;
                case CellState.Ship:
                    board[y, x] = CellState.HitShip;
                    switch (_gameOption.MoveOnHit)
                    {
                        case MoveOnHit.OtherPlayer:
                            _nextMoveByA = !_nextMoveByA;
                            return true;
                        case MoveOnHit.SamePlayer:
                            return true;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case CellState.Miss:
                    break;
                case CellState.HitShip:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }

        public string GetSerializedGameState()
        {
            var state = new GameState
            {
                NextMoveByX = _nextMoveByA, 
                Width = _boardA.GetLength(1), 
                Height = _boardA.GetLength(0),
                GameOption = _gameOption
            };
            
            state.BoardA = new CellState[state.Width ][];
            state.BoardB = new CellState[state.Width ][];
            
            for (var i = 0; i < state.BoardA.Length; i++)
            {
                state.BoardA[i] = new CellState[state.Height];
                state.BoardB[i] = new CellState[state.Height];
            }

            for (var x = 0; x < state.Width; x++)
            {
                for (var y = 0; y < state.Height; y++)
                {
                    state.BoardA[x][y] = _boardA[x, y];
                    state.BoardB[x][y] = _boardB[x, y];
                }
            }

            var jsonOptions = new JsonSerializerOptions()
            {
                 WriteIndented = true
            };
            return JsonSerializer.Serialize(state, jsonOptions);
            
        }

        public void SetGameStateFromJsonString(string jsonString)
        {
            var state = JsonSerializer.Deserialize<GameState>(jsonString);
            
            // restore actual state from deserialized state
            _nextMoveByA = state.NextMoveByX;
            _boardA =  new CellState[state.Width, state.Height];
            _boardB =  new CellState[state.Width, state.Height];
            _boardHeight = state.Height;
            _boardWidth = state.Width;
            _gameOption = state.GameOption;
            
            
            for (var x = 0; x < state.Width; x++)
            {
                for (var y = 0; y < state.Height; y++)
                {
                    _boardA[x, y] = state.BoardA[x][y];
                    _boardB[x, y] = state.BoardB[x][y];
                }
            }
            
        }

        public int GetBoardWidth()
        {
            return _boardWidth;
        }

        public int GetBoardHeight()
        {
            return _boardHeight;
        }

        public bool GetTurn()
        {
            return _nextMoveByA;
        }

        public GameOption GetGameOptions()
        {
            return _gameOption;
        }
    }

}