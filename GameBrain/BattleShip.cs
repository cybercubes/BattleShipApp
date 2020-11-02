using System;
using System.Text.Json;
using GameBrain.Enums;
using GameBrain.GameBrain;

namespace GameBrain
{
    public class BattleShip
    {
        private CellState[,] _boardA;
        private CellState[,] _boardB;
        private int _boardWidth;
        private int _boardHeight;
        private bool _nextMoveByA = true;
        private GameOption _gameOption;

        public BattleShip(GameOption option)
        {
            _boardA = new CellState[option.BoardHeight, option.BoardWidth];
            _boardB = new CellState[option.BoardHeight, option.BoardWidth];

            _boardWidth = option.BoardWidth;
            _boardHeight = option.BoardHeight;

            _gameOption = option;

        }

        public (CellState[,], CellState[,]) GetBoards()
        {
            var resA = new CellState[_boardHeight,_boardWidth];
            Array.Copy(_boardA, resA, _boardA.Length );
            
            var resB = new CellState[_boardHeight,_boardWidth];
            Array.Copy(_boardB, resB, _boardA.Length );

            return (resA, resB);
        }

        public bool MakeAMove(int x, int y)
        {
            CellState[,] board = _nextMoveByA ? _boardA : _boardB;
            
            if (board[y, x] == CellState.Empty)
            {
                board[y, x] = CellState.Miss;
                _nextMoveByA = !_nextMoveByA;
                return true;
            }

            if (board[y, x] == CellState.Ship)
            {
                board[y, x] = CellState.HitShip;
                switch (_gameOption.MoveOnHit)
                {
                    case MoveOnHit.OtherPlayer:
                        _nextMoveByA = !_nextMoveByA;
                        return true;
                    case MoveOnHit.SamePlayer:
                        return true;
                }
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