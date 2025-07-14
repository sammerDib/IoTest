using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.Shared.DieCutUpUI.Common
{
    /// <summary>
    ///     Utility class for GridVM to give access to a cell and modify it.
    /// </summary>
    public class CellIndex
    {
        private readonly GridVM _parentGrid;

        public CellIndex(GridVM parentGrid, int x, int y)
        {
            _parentGrid = parentGrid;
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

        public bool IsSelected()
        {
            return _parentGrid.SelectedCellsSet.Contains((X, Y));
        }

        public void Select()
        {
            _parentGrid.SelectedCellsSet.Add((X, Y));
        }

        public void UnSelect()
        {
            _parentGrid.SelectedCellsSet.Remove((X, Y));
        }

        public void SetAsReference()
        {
            _parentGrid.ReferenceReticle = (X, Y);
        }

        public Rect GetRect()
        {
            int x = _parentGrid.OffsetX + _parentGrid.BoxWidth * X;
            int y = _parentGrid.OffsetY + _parentGrid.BoxHeight * Y;
            return new Rect(x, y, _parentGrid.BoxWidth, _parentGrid.BoxHeight);
        }
    }

    /// <summary>
    ///     A class to define and get the properties of a reticule grid on an area in pixels.
    ///     This class does not take in account things like scale or scroll value, that should
    ///     be accounted for before giving a value to and after recieving a value from the GridVM.
    /// </summary>
    public class GridVM : ObservableRecipient
    {
        private const double GridColorSaturation = 1.0;
        private const double GridColorLightness = 0.5;

        private int _boxHeight;

        private int _boxWidth;

        private Color _color;

        private bool _isReticleVisible;

        private bool _isSelectingReferenceReticle;

        private string _name;

        private int _offsetX;

        private int _offsetY;

        private (int, int) _referenceReticle;

        private HashSet<(int, int)> _selectedCells;

        public GridVM(string name, Color color, int offsetX = 0, int offsetY = 0, int boxWidth = 0, int boxHeight = 0)
        {
            _name = name;
            OffsetX = offsetX;
            OffsetY = offsetY;
            BoxWidth = boxWidth;
            BoxHeight = boxHeight;
            _selectedCells = new HashSet<(int, int)>();
            _isSelectingReferenceReticle = false;
            _isReticleVisible = true;
            _color = color;
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public int OffsetX
        {
            get => _offsetX;
            set => SetProperty(ref _offsetX, value);
        }

        public int OffsetY
        {
            get => _offsetY;
            set => SetProperty(ref _offsetY, value);
        }

        public int BoxWidth
        {
            get => _boxWidth;
            set => SetProperty(ref _boxWidth, value);
        }

        public int BoxHeight
        {
            get => _boxHeight;
            set => SetProperty(ref _boxHeight, value);
        }

        public (int, int) ReferenceReticle
        {
            get => _referenceReticle;
            set => SetProperty(ref _referenceReticle, value);
        }

        public bool IsSelectingReferenceReticle
        {
            get => _isSelectingReferenceReticle;
            set => SetProperty(ref _isSelectingReferenceReticle, value);
        }

        public bool IsReticleVisible
        {
            get => _isReticleVisible;
            set => SetProperty(ref _isReticleVisible, value);
        }

        public Color Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        public Brush ColorBrush
        {
            get => new SolidColorBrush(Color.FromArgb(255, Color.R, Color.G, Color.B));
        }

        internal HashSet<(int, int)> SelectedCellsSet
        {
            get => _selectedCells;
            set => SetProperty(ref _selectedCells, value);
        }

        public CellIndex CellAtPosition(Point pos)
        {
            int x = (int)Math.Floor((pos.X - OffsetX) / BoxWidth);
            int y = (int)Math.Floor((pos.Y - OffsetY) / BoxHeight);
            return new CellIndex(this, x, y);
        }

        public IEnumerable<double> VerticalLinesBetween(double xStart, double xEnd)
        {
            if (BoxWidth <= 0)
            {
                yield break;
            }

            // Math.Ceiling because the result of Math.Floor will be lower than xStart
            double x = Math.Ceiling((xStart - OffsetX) / BoxWidth) * BoxWidth + OffsetX;
            for (double i = x; i < xEnd; i += BoxWidth)
            {
                yield return i;
            }
        }

        public IEnumerable<double> HorizontalLinesBetween(double yStart, double yEnd)
        {
            if (BoxHeight <= 0)
            {
                yield break;
            }

            // Math.Ceiling because the result of Math.Floor will be lower than yStart
            double y = Math.Ceiling((yStart - OffsetY) / BoxHeight) * BoxHeight + OffsetY;
            for (double i = y; i < yEnd; i += BoxHeight)
            {
                yield return i;
            }
        }

        public IEnumerable<CellIndex> SelectedCells()
        {
            foreach (var pos in _selectedCells)
            {
                yield return new CellIndex(this, pos.Item1, pos.Item2);
            }
        }

        public IEnumerable<CellIndex> CellsInBetween(CellIndex cell1, CellIndex cell2)
        {
            int minX = Math.Min(cell1.X, cell2.X);
            int minY = Math.Min(cell1.Y, cell2.Y);

            int maxX = Math.Max(cell1.X, cell2.X);
            int maxY = Math.Max(cell1.Y, cell2.Y);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    yield return new CellIndex(this, x, y);
                }
            }
        }

        public void ClearSelectedCells()
        {
            _selectedCells.Clear();
        }


        public override string ToString()
        {
            return _name;
        }
    }
}
