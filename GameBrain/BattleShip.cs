using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Domain;
using Domain.Enums;

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
        private string _winnerString = "";
        private GameOption _gameOption;
        private GameBoat[] _playerABoats;
        private GameBoat[] _playerBBoats;
        private List<JournalEntry> _gameJournal;

        public BattleShip(GameOption option)
        {
            _boardA = new CellState[option.BoardHeight, option.BoardWidth];
            _boardB = new CellState[option.BoardHeight, option.BoardWidth];

            _boardWidth = option.BoardWidth;
            _boardHeight = option.BoardHeight;

            _gameOption = option;
            
            _playerABoats = BuildBoats();
            _playerBBoats = BuildBoats();
            
            _gameJournal = new List<JournalEntry>();
        }

        public void ReplayJournalEntry(int moveNumber)
        {
            if (moveNumber < -1 && moveNumber >= _gameJournal.Count) return;

            _nextMoveByA = true;
            
            UpdateBoatsOnBoard();
            _placeBoatsByA = !_placeBoatsByA;
            UpdateBoatsOnBoard();

            if (moveNumber == -1)
            {
                _gameJournal.Clear();
                return;
            }

            for (var i = 0; i <= moveNumber; i++)
            {
                MakeAMove(_gameJournal[i].X, _gameJournal[i].Y);
            }
            
            _gameJournal.RemoveRange(moveNumber + 1, _gameJournal.Count - moveNumber - 1);
        }

        public bool CheckIfBoatLimitIsViolated(GameBoat[] boats)
        {
            var counter = boats.Count(boat => boat.CoordX > -1 && boat.CoordY > -1);
            if (counter == 0) return true;
            
            if (_gameOption.BoatLimit == -1) return false;

            return counter != _gameOption.BoatLimit;
        }

        public bool CheckIfBoatsOverlap(GameBoat[] boats)
        {
            var shipCellCount = 0;
            var boardShipCount = 0;
            var tempBoard = new CellState[_boardHeight, _boardWidth];

            foreach (var boat in boats)
            {
                if (boat.CoordX == -1 && boat.CoordY == -1) continue;

                shipCellCount += boat.Size;
                
                UpdateBoat(boat, tempBoard);
            }
            
            for (var x = 0; x < _boardWidth; x++)
            {
                for (var y = 0; y < _boardHeight; y++)
                {
                    if (tempBoard[y, x] == CellState.Ship) boardShipCount++;
                }
            }

            return shipCellCount != boardShipCount;
        }

        public bool CheckIfTouchViolated(GameBoat[] boats, CellState[,] board)
        {
            if (_gameOption.CanBoatsTouch == CanBoatsTouch.Yes) return false;

            foreach (var boat in boats)
            {
                if (boat.CoordX == -1 && boat.CoordY == -1) continue;

                if (_gameOption.CanBoatsTouch == CanBoatsTouch.No)
                {
                    if (CheckBoatCorners(boat, board)) return true;
                }
                if (CheckBoatEdges(boat, board)) return true;
            }
            
            return false;
        }

        private bool CheckBoatCorners(GameBoat boat, CellState[,] board)
        {
            if (boat.Horizontal)
            {
                if (!(boat.CoordY - 1 < 0 || boat.CoordX - 1 < 0))
                {
                    if (board[boat.CoordY - 1, boat.CoordX - 1] == CellState.Ship) return true;
                }
                
                if (!(boat.CoordY + 1 > _boardHeight - 1 || boat.CoordX - 1 < 0))
                {
                    if (board[boat.CoordY + 1, boat.CoordX - 1] == CellState.Ship) return true;
                }
                
                if (!(boat.CoordY + 1 > _boardHeight - 1 || boat.CoordX + boat.Size > _boardWidth - 1))
                {
                    if (board[boat.CoordY + 1 , boat.CoordX + boat.Size] == CellState.Ship) return true;
                }
                
                if (!(boat.CoordY - 1 < 0 || boat.CoordX + boat.Size > _boardWidth - 1))
                {
                    if (board[boat.CoordY - 1 , boat.CoordX + boat.Size] == CellState.Ship) return true;
                }

                return false;
            }


            if (!(boat.CoordY - 1 < 0 || boat.CoordX - 1 < 0))
            {
                if (board[boat.CoordY - 1, boat.CoordX - 1] == CellState.Ship) return true;
            }

            if (!(boat.CoordY - 1 < 0 || boat.CoordX + 1 > _boardWidth - 1))
            {
                if (board[boat.CoordY - 1, boat.CoordX + 1] == CellState.Ship) return true;
            }

            if (!(boat.CoordY + boat.Size > _boardHeight - 1 || boat.CoordX - 1 < 0))
            {
                if (board[boat.CoordY + boat.Size , boat.CoordX - 1] == CellState.Ship) return true;
            }

            if (!(boat.CoordY + boat.Size > _boardHeight - 1 || boat.CoordX + 1 > _boardWidth - 1))
            {
                if (board[boat.CoordY + boat.Size, boat.CoordX + 1] == CellState.Ship) return true;
            }

            return false;
        }

        private bool CheckBoatEdges(GameBoat boat, CellState[,] board)
        {
            if (boat.Horizontal)
            {
                if (!(boat.CoordX - 1 < 0))
                {
                    if (board[boat.CoordY, boat.CoordX - 1] == CellState.Ship) return true;   
                }

                for (var i = 0; i < boat.Size; i++)
                {
                    if (!(boat.CoordY - 1 < 0 || boat.CoordX + i >= _boardWidth))
                    {
                        if (board[boat.CoordY - 1, boat.CoordX + i] == CellState.Ship) return true;
                    }
                    
                    if (!(boat.CoordY + 1 >= _boardHeight || boat.CoordX + i >= _boardWidth))
                    {
                        if (board[boat.CoordY + 1, boat.CoordX + i] == CellState.Ship) return true;
                    }
                }

                if (!(boat.CoordX + boat.Size >= _boardWidth))
                {
                    if (board[boat.CoordY, boat.CoordX + boat.Size] == CellState.Ship) return true;
                }

                return false;
            }

            if (!(boat.CoordY - 1 < 0))
            {
                if (board[boat.CoordY - 1, boat.CoordX] == CellState.Ship) return true;
            }

            for (var i = 0; i < boat.Size; i++)
            {
                if (!(boat.CoordY + i >= _boardHeight || boat.CoordX - 1 < 0))
                {
                    if (board[boat.CoordY + i, boat.CoordX - 1] == CellState.Ship) return true;
                }
                
                if (!(boat.CoordY + i >= _boardHeight || boat.CoordX + 1 >= _boardWidth))
                {
                    if (board[boat.CoordY + i, boat.CoordX + 1] == CellState.Ship) return true;
                }
            }
            
            if (!(boat.CoordY + boat.Size + 1 >= _boardHeight))
            {
                if (board[boat.CoordY + boat.Size + 1, boat.CoordX] == CellState.Ship) return true;
            }
            
            
            return false;
        }


        public void PlaceBoat(int boatIndex, int x, int y)
        {
            var boats = _placeBoatsByA ? _playerABoats : _playerBBoats;
            
            if (x == -1 && y == -1)
            {
                boats[boatIndex].CoordX = x;
                boats[boatIndex].CoordY = y;
            }

            if (x < 0 || y < 0) return;

            if (x > _boardWidth - 1 || y > _boardHeight - 1) return;
            
            if (x + boats[boatIndex].Size > _boardWidth && boats[boatIndex].Horizontal) return;
            
            if (y + boats[boatIndex].Size > _boardHeight && !boats[boatIndex].Horizontal) return;

            boats[boatIndex].CoordX = x;
            boats[boatIndex].CoordY = y;
        }

        public void RotateBoat(int boatIndex)
        {
            var boats = _placeBoatsByA ? _playerABoats : _playerBBoats;

            if (boats[boatIndex].Size == 1) return;

            if (boats[boatIndex].CoordX + boats[boatIndex].Size - 1 > _boardWidth - 1 
                && !boats[boatIndex].Horizontal) return;
            
            if (boats[boatIndex].CoordY + boats[boatIndex].Size - 1 > _boardHeight - 1 
                && boats[boatIndex].Horizontal) return;

            boats[boatIndex].Horizontal = !boats[boatIndex].Horizontal;
        }

        public void UpdateBoatsOnBoard()
        {
            var board = new CellState[_boardHeight, _boardWidth];
            var boats = _placeBoatsByA ? _playerABoats : _playerBBoats;

            foreach (var boat in boats)
            {
                if (boat.CoordX >= 0 && boat.CoordY >= 0)
                {
                    UpdateBoat(boat, board);
                }

            }

            if (_placeBoatsByA)
            {
                _boardA = board;
                return;
            }
            
            _boardB = board;
            
        }

        private void UpdateBoat(GameBoat boat, CellState[,] board)
        {
            for (var i = 0; i < boat.Size; i++)
            {
                if (boat.Horizontal)
                {
                    board[boat.CoordY, boat.CoordX + i] = CellState.Ship;
                    continue;
                }
                
                board[boat.CoordY + i, boat.CoordX] = CellState.Ship;
                
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
        
        public void AutoShipSetupForOneBoard(GameBoat[] boatArray)
        {
            var r = new Random();
            
            do
            {
                for (var i = 0; i < boatArray.Length; i++)
                {
                    var boatIndex = r.Next(0, boatArray.Length);
                    var boat = boatArray[boatIndex];

                    if (boat.CoordX == -1 && boat.CoordY == -1)
                    {
                        var isHorizontal = (r.Next(0, 2) == 1);
                        boat.Horizontal = isHorizontal;
                    }

                    if (r.Next(0, 100) < 50)
                    {
                        var x = r.Next(0, _boardWidth);
                        var y = r.Next(0, _boardHeight);

                        if (boat.Horizontal && x + boat.Size - 1 < _boardWidth)
                        {
                            boat.CoordX = x;
                            boat.CoordY = y;
                        }

                        if (!boat.Horizontal && y + boat.Size - 1 < _boardHeight)
                        {
                            boat.CoordX = x;
                            boat.CoordY = y;
                        }

                    }
                    else
                    {
                        boat.CoordX = -1;
                        boat.CoordY = -1;
                    }
                    
                    UpdateBoatsOnBoard();
                }

                var board = _placeBoatsByA ? _boardA : _boardB;
                if (!(CheckIfBoatsOverlap(boatArray) ||
                      CheckIfTouchViolated(boatArray, board) ||
                      CheckIfBoatLimitIsViolated(boatArray))
                ) break;

            } while (true);
        }

        public int CountBoatsFromOptions()
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
            var resA = _playerABoats;

            var resB = _playerBBoats;

            return (resA, resB);
        }

        public void MakeAMove(int x, int y)
        {
            if (_winnerString != "") return;
            
            var board = _nextMoveByA ? _boardA : _boardB;

            switch (board[y, x])
            {
                case CellState.Empty:
                    board[y, x] = CellState.Miss;
                    _nextMoveByA = !_nextMoveByA;
                    break;
                case CellState.Ship:
                    board[y, x] = CellState.HitShip;
                    switch (_gameOption.MoveOnHit)
                    {
                        case MoveOnHit.OtherPlayer:
                            _nextMoveByA = !_nextMoveByA;
                            break;
                        case MoveOnHit.SamePlayer:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case CellState.Miss:
                    break;
                case CellState.HitShip:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CheckForWinner(board);

        }

        private int CountSpecificCell(CellState[,] board, CellState cellState)
        {
            var counter = 0;
            for (var x = 0; x < _boardWidth; x++)
            {
                for (var y = 0; y < _boardHeight; y++)
                {
                    if (board[y, x] == cellState) counter++;
                }
            }

            return counter;
        }

        private void CheckForWinner(CellState[,] board)
        {
            if (CountSpecificCell(board, CellState.Ship) == 0)
            {
                _winnerString = board == _boardA ? "Winner is player A" : "Winner is player B";
            }

            //Less demanding solution 
            /*for (var x = 0; x < _boardWidth; x++)
            {
                for (var y = 0; y < _boardHeight; y++)
                {
                    if (board[x, y] == CellState.Ship) return;
                }
            }

            _winnerString = board == _boardA ? "Winner is player A" : "Winner is player B";
            
            */
        }

        public string GetSerializedGameState()
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            var state = new GameState
            {
                NextMoveByX = _nextMoveByA, 
                Width = _boardA.GetLength(1), 
                Height = _boardA.GetLength(0),
                //GameOption = _gameOption,
                PlayerABoats = _playerABoats,
                PlayerBBoats = _playerBBoats,
                GameJournal = _gameJournal.ToArray(),
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
            
            return JsonSerializer.Serialize(state, jsonOptions);
            
        }

        public void SetGameStateFromJsonString(string jsonString, GameOption option)
        {
            var state = JsonSerializer.Deserialize<GameState>(jsonString);
            
            // restore actual state from deserialized state
            _nextMoveByA = state!.NextMoveByX;
            _boardA =  new CellState[state.Width, state.Height];
            _boardB =  new CellState[state.Width, state.Height];
            _boardHeight = state.Height;
            _boardWidth = state.Width;
            _gameOption = option;
            _playerABoats = state.PlayerABoats;
            _playerBBoats = state.PlayerBBoats;
            _gameJournal = state.GameJournal.ToList();
            
            
            for (var x = 0; x < state.Width; x++)
            {
                for (var y = 0; y < state.Height; y++)
                {
                    _boardA[x, y] = state.BoardA[x][y];
                    _boardB[x, y] = state.BoardB[x][y];
                }
            }
            
        }

        public void RecordMove(int x, int y)
        {
            _gameJournal.Add(new JournalEntry(x, y));
        }

        public void ChangeWhoPlacesBoats()
        {
            _placeBoatsByA = !_placeBoatsByA;
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

        public bool GetPlaceBoatsByA()
        {
            return _placeBoatsByA;
        }

        public string GetWinnerString()
        {
            return _winnerString;
        }

        public void SetWinnerString(string winnerString)
        {
            _winnerString = winnerString;
        }
        
        public int GetJournalCount()
        {
            return _gameJournal.Count;
        }

        public List<JournalEntry> GetJournal()
        {
            return _gameJournal;
        }

        public GameOption GetGameOptions()
        {
            return _gameOption;
        }
    }

}