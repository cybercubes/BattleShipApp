using System;
using System.Text.Json;
using GameBrain.GameBrain;

namespace GameBrain
{
    public class BattleShip
    {
        private CellState[,] _board;
        private int _boardSize;
        private bool _nextMoveByX = true;
        
        public BattleShip(int size)
        {
            _board = new CellState[size,size];
            _boardSize = size;
        }

        public CellState[,] GetBoard()
        {
            var res = new CellState[_boardSize,_boardSize];
            Array.Copy(_board, res, _board.Length );
            return res;
        }

        public bool MakeAMove(int x, int y)
        {
            if (_board[x, y] == CellState.Empty)
            {
                _board[x, y] = _nextMoveByX ? CellState.X : CellState.O;
                _nextMoveByX = !_nextMoveByX;
                return true;
            }

            return false;
        }


        public string GetSerializedGameState()
        {
            var state = new GameState
            {
                NextMoveByX = _nextMoveByX, 
                Width = _board.GetLength(0), 
                Height = _board.GetLength(1)
            };
            
            state.Board = new CellState[state.Width ][];
            
            for (var i = 0; i < state.Board.Length; i++)
            {
                state.Board[i] = new CellState[state.Height];
            }

            for (var x = 0; x < state.Width; x++)
            {
                for (var y = 0; y < state.Height; y++)
                {
                    state.Board[x][y] = _board[x, y];
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
            _nextMoveByX = state.NextMoveByX;
            _board =  new CellState[state.Width, state.Height];
            
            for (var x = 0; x < state.Width; x++)
            {
                for (var y = 0; y < state.Height; y++)
                {
                    _board[x, y] = state.Board[x][y];
                }
            }
            
        }

        public int GetBoardSize()
        {
            return _boardSize;
        }
    }

}