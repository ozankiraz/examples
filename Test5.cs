using System;
using System.Drawing;
using System.Linq;
using Xunit;

namespace CheckCheck
{
    public class Test5
    {
        [Fact]
        public void ConsecutiveFibonacci()
        {
            var series = new[] {2, 3, 5, 8, 13};
            var result = Grid.IsConsecutiveFibonacciSeries(series);
            Assert.Equal(result, true);
        }

        [Fact]
        public void ConsecutiveFibonacci_Fail()
        {
            var series = new[] { 1, 1, 2, 3, 13 };
            var result = Grid.IsConsecutiveFibonacciSeries(series);
            Assert.Equal(result, false);
        }

        [Fact]
        public void ConsecutiveFibonacci_Fail_2()
        {
            var series = new[] { 0, 1, 9, 7, 3 };
            var result = Grid.IsConsecutiveFibonacciSeries(series);
            Assert.Equal(result, false);
        }

        [Fact]
        public void Test_Preset_Grid()
        {
            var grid = new Grid(10)
            {
                GridData =
                {
                    [0, 0] = new GridCell {Color = Color.White, Value = 2},
                    [1, 0] = new GridCell {Color = Color.White, Value = 3},
                    [2, 0] = new GridCell {Color = Color.White, Value = 5},
                    [3, 0] = new GridCell {Color = Color.White, Value = 8},
                    [4, 0] = new GridCell {Color = Color.White, Value = 12}
                }
            };


            grid.Click(4, 3);
            grid.Click(4, 3);

            var actual = Enumerable.Range(0, 5).Select(i => grid.GridData[i, 0]).ToList();

            actual.ForEach(a => Assert.Equal(a.Value, 0));
        }
        
        [Fact]
        public void Test_ActualClicks()
        {
            var grid = new Grid(10);

            grid.Click(0, 0);
            grid.Click(0, 1);
            grid.Click(0, 2);
            grid.Click(0, 3);
            grid.Click(0, 4);

            grid.Click(1, 2);
            grid.Click(1, 2);
            
            grid.Click(1, 3);
            grid.Click(1, 3);
            grid.Click(1, 3);

            grid.Click(1, 4);
            grid.Click(1, 4);
            grid.Click(1, 4);
            grid.Click(1, 4);
            grid.Click(1, 4);


            var actual = Enumerable.Range(0, 5).Select(i => grid.GridData[0, i]).ToList();

            actual.ForEach(a => Assert.Equal(a.Value, 0));
        }
    }

    public class GridCell
    {
        public int Value { get; set; }

        public Color Color { get; set; }

        public override string ToString()
        {
            return string.Concat(Value, " ", Color.Name);
        }

        public void SetColor(Color color)
        {
            Color = color;
        }

        public void ToggleYellow()
        {
            SetColor(Color.Yellow);
            SetColor(Color.White);
        }

        public void ToggleGreen()
        {
            SetColor(Color.Green);
            SetColor(Color.White);
        }

        public void Reset()
        {
            Value = 0;
        }

        public void Increment()
        {
            Value++;
        }

        public void Decrement()
        {
            Value--;
        }
    }

    public class Grid
    {
        private readonly int _size;
        
        public GridCell[,] GridData;

        public Grid(int size)
        {
            _size = size;
            GridData = new GridCell[size, size];

            InitializeGridData();
        }

        public void Click(int x, int y)
        {
            if (x >= _size || y >= _size)
            {
                throw new Exception(string.Format("Cannot Access Grid Element {0} {1}", x, y));
            }

            IncreaseValues(x, y);

            for (var row = 0; row < _size; row++)
            {
                for (var column = 0; column < _size - 5; column++)
                {
                    var currentCell = GridData[row, column];
                    
                    var sequenceGridData = new []
                    {
                        currentCell,
                        GridData[row, column + 1],
                        GridData[row, column + 2],
                        GridData[row, column + 3],
                        GridData[row, column + 4]
                    };

                    var hasFiveConsecutiveFibonacciSeries = IsConsecutiveFibonacciSeries(sequenceGridData.Select(s => s.Value).ToArray());

                    if (hasFiveConsecutiveFibonacciSeries)
                    {
                        sequenceGridData.ToList().ForEach(s => { s.ToggleGreen(); s.Reset(); });
                    }
                }
            }

            for (var column = 0; column < _size ; column++)
            {
                for (var row = 0; row < _size - 5; row++)
                {
                    var currentCell = GridData[row, column];

                    var sequenceGridData = new[]
                    {
                        currentCell,
                        GridData[row + 1, column],
                        GridData[row + 2, column],
                        GridData[row + 3, column],
                        GridData[row + 4, column]
                    };

                    var hasFiveConsecutiveFibonacciSeries = IsConsecutiveFibonacciSeries(sequenceGridData.Select(s => s.Value).ToArray());

                    if (hasFiveConsecutiveFibonacciSeries)
                    {
                        sequenceGridData.ToList().ForEach(s => { s.ToggleGreen(); s.Reset(); });
                    }
                }
            }

        }

        private void InitializeGridData()
        {
            for (var i = 0; i < _size; i++)
            {
                for (var j = 0; j < _size; j++)
                {
                    GridData[i, j] = new GridCell { Value = 0, Color = Color.White };
                }
            }
        }

        private void IncreaseValues(int x, int y)
        {
            var clickedCell = GridData[x, y];

            if (clickedCell.Value == 0)
            {
                clickedCell.Increment();
                return;
            }

            for (var row = 0; row < _size; row++)
            {
                var current = GridData[x, row];

                if (current.Value != 0)
                {
                    current.Increment();
                    current.ToggleYellow();
                }
            }

            for (var column = 0; column < _size; column++)
            {
                var current = GridData[column, y];

                if (current.Value != 0)
                {
                    current.Increment();
                    current.ToggleYellow();
                }
            }

            // Small Hack to Avoid Increasing Twice, otherwise had to use if statement
            clickedCell.Decrement();
        }
        
        public static bool IsConsecutiveFibonacciSeries(int[] series)
        {
            if (series.Length != 5)
            {
                return false;
            }

            var isConsecutive = false;
            for (var i = 0; i < series.Length - 2; i++)
            {
                var prev = series[i];
                var next = series[i + 1];
                var sum = series[i + 2];

                if (prev == 0 && next == 0)
                {
                    return false;
                }

                isConsecutive = IsFibonacci(prev) && IsFibonacci(next) && IsFibonacci(sum) && (prev + next == sum);
            }

            return isConsecutive;
        }

        //Found those logic online, saves time
        private static bool IsPerfectSquare(int x)
        {
            var s = (int)Math.Sqrt(x);
            return (s * s == x);
        }

        //Found those logic online, saves time
        private static bool IsFibonacci(int n)
        {
            return (IsPerfectSquare(5 * n * n + 4)
                    || IsPerfectSquare(5 * n * n - 4));
        }
    }
}
