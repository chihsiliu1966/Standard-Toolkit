﻿#region BSD License
/*
 * 
 * Original BSD 3-Clause License (https://github.com/ComponentFactory/Krypton/blob/master/LICENSE)
 *  © Component Factory Pty Ltd, 2006 - 2016, All rights reserved.
 * 
 *  New BSD 3-Clause License (https://github.com/Krypton-Suite/Standard-Toolkit/blob/master/LICENSE)
 *  Modifications by Peter Wagner(aka Wagnerp) & Simon Coghlan(aka Smurf-IV), et al. 2017 - 2024. All rights reserved. 
 *  
 *  Modified: Monday 12th April, 2021 @ 18:00 GMT
 *
 */
#endregion

namespace Krypton.Ribbon
{
    /// <summary>
    /// Draws either a large or small image from a group color button.
    /// </summary>
    internal class ViewDrawRibbonGroupColorButtonImage : ViewDrawRibbonGroupImageBase

    {
        #region Instance Fields
        private readonly Size _smallSize; //new Size(16, 16);
        private readonly Size _largeSize;//new Size(32, 32);
        private readonly KryptonRibbonGroupColorButton _ribbonColorButton;
        private readonly bool _large;
        private Image? _compositeImage;
        private Color _selectedColor;
        private Color _emptyBorderColor;
        private Rectangle _selectedRectSmall;
        private Rectangle _selectedRectLarge;
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the ViewDrawRibbonGroupColorButtonImage class.
        /// </summary>
        /// <param name="ribbon">Reference to owning ribbon control.</param>
        /// <param name="ribbonColorButton">Reference to ribbon group color button definition.</param>
        /// <param name="large">Show the large image.</param>
        public ViewDrawRibbonGroupColorButtonImage([DisallowNull] KryptonRibbon? ribbon,
                                                   [DisallowNull] KryptonRibbonGroupColorButton? ribbonColorButton,
                                                   bool large)
            : base(ribbon)
        {
            Debug.Assert(ribbonColorButton is not null);

            _ribbonColorButton = ribbonColorButton ?? throw new NullReferenceException(GlobalStaticValues.VariableCannotBeNull(nameof(ribbonColorButton)));
            _selectedColor = ribbonColorButton.SelectedColor;
            _emptyBorderColor = ribbonColorButton.EmptyBorderColor;
            _selectedRectSmall = ribbonColorButton.SelectedRectSmall;
            _selectedRectLarge = ribbonColorButton.SelectedRectLarge;
            _large = large;

            _smallSize = new Size((int)(16 * FactorDpiX), (int)(16 * FactorDpiY));
            _largeSize = new Size((int)(32 * FactorDpiX), (int)(32 * FactorDpiY));
        }

        /// <summary>
        /// Obtains the String representation of this instance.
        /// </summary>
        /// <returns>User readable name of the instance.</returns>
        public override string ToString() =>
            // Return the class name and instance identifier
            $@"ViewDrawRibbonGroupColorButtonImage:{Id}";

        #endregion

        #region Public
        /// <summary>
        /// Notification that the selected color has changed.
        /// </summary>
        public void SelectedColorRectChanged()
        {
            // If we have a cache image we need to release it
            if (_compositeImage != null)
            {
                _compositeImage.Dispose();
                _compositeImage = null;
            }

            _emptyBorderColor = _ribbonColorButton.EmptyBorderColor;
            _selectedColor = _ribbonColorButton.SelectedColor;
            _selectedRectSmall = _ribbonColorButton.SelectedRectSmall;
            _selectedRectLarge = _ribbonColorButton.SelectedRectLarge;
        }
        #endregion

        #region Protected
        /// <summary>
        /// Gets the size to draw the image.
        /// </summary>
        protected override Size DrawSize => _large ? _largeSize : _smallSize;

        /// <summary>
        /// Gets the image to be drawn.
        /// </summary>
        protected override Image? DrawImage
        {
            get
            {
                Image? newImage;
                if (_ribbonColorButton.KryptonCommand != null)
                {
                    newImage = _large ? _ribbonColorButton.KryptonCommand.ImageLarge : _ribbonColorButton.KryptonCommand.ImageSmall;
                }
                else
                {
                    newImage = _large ? _ribbonColorButton.ImageLarge : _ribbonColorButton.ImageSmall;
                }

                // Do we need to create another composite image?
                if ((newImage != null) && (_compositeImage == null))
                {
                    // Create a copy of the source image
                    var copyBitmap = new Bitmap(newImage);

                    // Paint over the image with a color indicator
                    using (Graphics g = Graphics.FromImage(copyBitmap))
                    {
                        Rectangle selectedRect = _large ? _selectedRectLarge : _selectedRectSmall;

                        // If the color is not defined, i.e. it is empty then...
                        if (_selectedColor.Equals(Color.Empty))
                        {
                            // Indicate the absence of a color by drawing a border around 
                            // the selected color area, thus indicating the area inside the
                            // block is blank/empty.
                            using var borderPen = new Pen(_emptyBorderColor);
                            g.DrawRectangle(borderPen, selectedRect with { Width = selectedRect.Width - 1, Height = selectedRect.Height - 1 });
                        }
                        else
                        {
                            // We have a valid selected color so draw a solid block of color
                            using var colorBrush = new SolidBrush(_selectedColor);
                            g.FillRectangle(colorBrush, selectedRect);
                        }
                    }

                    // Cache it for future use
                    _compositeImage = copyBitmap;
                }

                return _compositeImage;
            }
        }
        #endregion
    }
}

