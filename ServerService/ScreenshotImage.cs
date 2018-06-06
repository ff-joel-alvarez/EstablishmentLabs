using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Server
{
    public class ScreenshotImage
    {

        ///TODO: Stop using  ref and out, so it instead returns a struct.
        /// <summary>
        /// This method specifies the tiles size. 
        /// Sometimes we cant make all tiles to be of size 'size' because of the entire  width/height length (There could be a leftover or surplus). 
        /// </summary>
        /// <returns>
        /// Nothing
        /// </returns>    
        /// <param name="tileDimension">Height or width of a title</param>
        /// <param name="dimensionLeft">A fullscreen screenshot image.</param>
        /// <param name="size">Size of a tile</param>
        public void UpdateDimensions(ref int tileDimension, ref int dimensionLeft, int size)
        {

            if (tileDimension - size >= 0)
            {
                tileDimension = size;
                dimensionLeft -= size;
            }
            else
            {
                tileDimension = dimensionLeft;
            }
        }

        /// <summary>
        /// Given an image, make tiles of size "size"
        /// </summary>
        /// <returns>
        /// An array with all the created tiles
        /// </returns>    
        /// <param name="size">Size of a tile.</param>
        /// <param name="srcImage">A fullscreen screenshot image.</param>
        public Image[] MakeTiles(int size, Image srcImage)
        {

            //Given the fullscreen iamge width and size, calculate how many tiles horizontally and vertically given the tile size.
            int amountOfTilesHorizontal = (int)Math.Ceiling((double)srcImage.Width / size);
            int amountOfTitlesVertical = (int)Math.Ceiling((double)srcImage.Height / size);

            //Array to store all tiles
            var imgarray = new Image[amountOfTilesHorizontal * amountOfTitlesVertical];

            var heightLeft = srcImage.Height;
            var widthLeft = srcImage.Width;

            var tileHeight = size;
            var tileWidth = size;

            for (int i = 0; i < amountOfTilesHorizontal; i++)
            {
                //Reset height, we are starting on anew column
                heightLeft = srcImage.Height;
                UpdateDimensions(ref tileWidth, ref widthLeft, size);

                for (int j = 0; j < amountOfTitlesVertical; j++)
                {
                    var index = i * 3 + j;
                    UpdateDimensions(ref tileHeight, ref heightLeft, size);

                    imgarray[index] = new Bitmap(tileWidth, tileHeight);
                    var graphics = Graphics.FromImage(imgarray[index]);
                    graphics.DrawImage(srcImage, new Rectangle(0, 0, tileWidth, tileHeight), new Rectangle(i * size, j * size, tileWidth, tileHeight), GraphicsUnit.Pixel);
                    graphics.Dispose();

                }
            }

            return imgarray;
        }


        /// <summary>
        /// Take a screenshot of the active window on PC
        /// </summary>
        /// <returns>
        ///  A fullscreen screenshot image 
        /// </returns>    
        /// <param name="xdpi">X resolution</param>
        /// <param name="ydpi">Y resolution</param>
        /// <param name="filename">Filename</param>
        public Image TakeScreenshot(int xdpi = 1920, int ydpi = 1080, string filename = "Screenshot")
        {
            Image rtnImage = null;
            int screenWidth = Screen.GetBounds(new Point(0, 0)).Width;
            int screenHeight = Screen.GetBounds(new Point(0, 0)).Height;
            using (Bitmap bmpScreenShot = new Bitmap(screenWidth, screenHeight))
            {

                using (Graphics gfx = Graphics.FromImage((Image)bmpScreenShot))
                {
                    gfx.CopyFromScreen(0, 0, 0, 0, new Size(screenWidth, screenHeight));
                    gfx.CompositingMode = CompositingMode.SourceCopy;
                    gfx.CompositingQuality = CompositingQuality.HighQuality;
                    gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gfx.SmoothingMode = SmoothingMode.HighQuality;
                    gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    Bitmap bmpRet = new Bitmap(xdpi, ydpi);
                    Graphics bmpRetG = Graphics.FromImage(bmpRet);
                    bmpRetG.DrawImage(bmpScreenShot, 0, 0, xdpi, ydpi);
                    rtnImage = bmpRet;
                    pHash(rtnImage);
                }
            }
            return rtnImage;
        }


        /// <summary>
        /// Take a screenshot of the active window on PC
        /// </summary>
        /// <returns>
        ///  A fullscreen screenshot image 
        /// </returns>    
        /// <param name="xdpi">X resolution</param>
        /// <param name="ydpi">Y resolution</param>
        /// <param name="filename">Filename</param>
        private static Bitmap ToGrayscale(Bitmap original)
        {
            //Bitmap to 'load' new gray scaled image.
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            Graphics g = Graphics.FromImage(newBitmap);

            /*Create the grayscale ColorMatrix, these coefficients are based on the human perception of color,
            this is the standard conversion from RGB to gray.*/

            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
                       {
                 new float[] {.3f, .3f, .3f, 0, 0},
                 new float[] {.59f, .59f, .59f, 0, 0},
                 new float[] {.11f, .11f, .11f, 0, 0},
                 new float[] {0, 0, 0, 1, 0},
                 new float[] {0, 0, 0, 0, 1}
                       });

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute in order to change color scale.
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }

        public static bool pHash(Image firstImage)
        {
            //make image small
            int width = 64;
            int height = 64;

            Bitmap firstImageSmall = ResizeImage(firstImage, width,height);
            firstImageSmall = ToGrayscale(firstImageSmall);

            firstImageSmall.Save("TESTSTDF.png");

            return false;
        }

        /// <summary>
        /// Resize an image giving width and height.
        /// code based on https://stackoverflow.com/questions/1922040/resize-an-image-c-sharp
        /// </summary>
        /// <returns>
        ///  The resized image
        /// </returns>    
        /// <param name="image">Image to resize</param>
        /// <param name="width">Width of new image</param>
        /// <param name="height">Height of new image</param>

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
