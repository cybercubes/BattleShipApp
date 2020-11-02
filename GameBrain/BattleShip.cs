using System;
using System.Text.Json;
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

        public BattleShip(int width, int height)
        {
            _boardA = new CellState[height, width];
            _boardB = new CellState[height, width];

            _boardWidth = width;
            _boardHeight = height;
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

            return false;
        }


        public string GetSerializedGameState()
        {
            var state = new GameState
            {
                NextMoveByX = _nextMoveByA, 
                Width = _boardA.GetLength(1), 
                Height = _boardA.GetLength(0)
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
    }

}