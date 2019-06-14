//
// Active Forums - http://activeforums.org/
// Copyright (c) 2019
// by Active Forums Community
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;
//ORIGINAL LINE: Imports System.Web.HttpContext
using System.Drawing;
namespace DotNetNuke.Modules.ActiveForums
{
	public abstract class Images
	{
		public static int imgHeight;
		public static int imgWidth;
		private static int GetHeight(string sPath)
		{
			var g = Image.FromFile(sPath);
			//Dim thisFormat = g.RawFormat
			return g.Height;
		}
		private static int GetWidth(string sPath)
		{
			var g = Image.FromFile(sPath);
			//Dim thisFormat = g.RawFormat
			return g.Width;
		}
		public static int GetHeightFromStream(Stream sFile)
		{
			var g = Image.FromStream(sFile, true);
			//Dim thisFormat = g.RawFormat
			return g.Height;
		}
		public static int GetWidthFromStream(Stream sFile)
		{
			var g = Image.FromStream(sFile, true);
			//Dim thisFormat = g.RawFormat
			return g.Width;
		}
		public static void CreateImage(string sFile, int intHeight, int intWidth)
		{
			string tmp;
			if (sFile.ToUpper().Contains(".JPG"))
			{
				tmp = sFile.ToUpper().Replace(".JPG", "_TEMP.JPG");
			}
			else if (sFile.ToUpper().Contains(".PNG"))
			{
				tmp = sFile.ToUpper().Replace(".PNG", "_TEMP.PNG");
			}
			else if (sFile.ToUpper().Contains(".GIF"))
			{
				tmp = sFile.ToUpper().Replace(".GIF", "_TEMP.GIF");
			}
			else if (sFile.ToUpper().Contains(".JPEG"))
			{
				tmp = sFile.ToUpper().Replace(".JPEG", "_TEMP.JPEG");
			}
			else
			{
				tmp = sFile;
			}
			File.Copy(sFile, tmp);
			var g = Image.FromFile(tmp);
			//Dim thisFormat = g.RawFormat
			int newHeight;
			int newWidth;
			newHeight = intHeight;
			newWidth = intWidth;
			Size imgSize;
			if (g.Width > newWidth | g.Height > newHeight)
			{
				imgSize = NewImageSize(g.Width, g.Height, newWidth, newHeight);
				imgHeight = imgSize.Height;
				imgWidth = imgSize.Width;
			}
			else
			{
				imgHeight = g.Height;
				imgWidth = g.Width;
			}

			var imgOutput1 = new Bitmap(g, imgWidth, imgHeight);
			Graphics bmpOutput = Graphics.FromImage(imgOutput1);
			bmpOutput.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			bmpOutput.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			var compressionRectange = new Rectangle(0, 0, imgWidth, imgHeight);
			bmpOutput.DrawImage(g, compressionRectange);

			System.Drawing.Imaging.ImageCodecInfo myImageCodecInfo;
			System.Drawing.Imaging.Encoder myEncoder;
			System.Drawing.Imaging.EncoderParameter myEncoderParameter;
			System.Drawing.Imaging.EncoderParameters myEncoderParameters;
			myImageCodecInfo = GetEncoderInfo("image/jpeg");
			myEncoder = System.Drawing.Imaging.Encoder.Quality;
			myEncoderParameters = new System.Drawing.Imaging.EncoderParameters(1);
			myEncoderParameter = new System.Drawing.Imaging.EncoderParameter(myEncoder, 90);
			myEncoderParameters.Param[0] = myEncoderParameter;
			imgOutput1.Save(sFile, myImageCodecInfo, myEncoderParameters);
			g.Dispose();
			imgOutput1.Dispose();
			File.Delete(tmp);
		}


		public static MemoryStream CreateImageForDB(Stream sFile, int intHeight, int intWidth)
		{
			var newStream = new MemoryStream();
			var g = Image.FromStream(sFile);
			//Dim thisFormat = g.RawFormat
			if (intHeight > 0 & intWidth > 0)
			{
				int newHeight;
				int newWidth;
				newHeight = intHeight;
				newWidth = intWidth;
				Size imgSize;
				if (g.Width > newWidth | g.Height > newHeight)
				{
					imgSize = NewImageSize(g.Width, g.Height, newWidth, newHeight);
					imgHeight = imgSize.Height;
					imgWidth = imgSize.Width;
				}
				else
				{
					imgHeight = g.Height;
					imgWidth = g.Width;
				}
			}
			else
			{
				imgWidth = g.Width;
				imgHeight = g.Height;
			}

			var imgOutput1 = new Bitmap(g, imgWidth, imgHeight);
			Graphics bmpOutput = Graphics.FromImage(imgOutput1);
			bmpOutput.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			bmpOutput.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			var compressionRectange = new Rectangle(0, 0, imgWidth, imgHeight);
			bmpOutput.DrawImage(g, compressionRectange);
			System.Drawing.Imaging.ImageCodecInfo myImageCodecInfo;
			System.Drawing.Imaging.Encoder myEncoder;
			System.Drawing.Imaging.EncoderParameter myEncoderParameter;
			System.Drawing.Imaging.EncoderParameters myEncoderParameters;
			myImageCodecInfo = GetEncoderInfo("image/jpeg");
			myEncoder = System.Drawing.Imaging.Encoder.Quality;
			myEncoderParameters = new System.Drawing.Imaging.EncoderParameters(1);
			myEncoderParameter = new System.Drawing.Imaging.EncoderParameter(myEncoder, 90);
			myEncoderParameters.Param[0] = myEncoderParameter;
			imgOutput1.Save(newStream, myImageCodecInfo, myEncoderParameters);
			g.Dispose();
			imgOutput1.Dispose();
			bmpOutput.Dispose();
			return newStream;


		}
		public static System.Drawing.Imaging.ImageCodecInfo GetEncoderInfo(string MYmimeType)
		{
			try
			{
				int i;
				System.Drawing.Imaging.ImageCodecInfo[] encoders = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
				for (i = 0; i <= (encoders.Length - 1); i++)
				{
					if (encoders[i].MimeType == MYmimeType)
					{
						return encoders[i];
					}
				}
				return null;
			}
			catch
			{
				return null;
			}
		}
		public static Size NewImageSize(int currentWidth, int currentHeight, int newWidth, int newHeight)
		{
			double tempMultiplier;

			if (currentHeight > currentWidth)
			{
				tempMultiplier = newHeight / (double)currentHeight;
			}
			else
			{
				tempMultiplier = newWidth / (double)currentWidth;
			}

			var NewSize = new Size(Convert.ToInt32(currentWidth * tempMultiplier), Convert.ToInt32(currentHeight * tempMultiplier));

			return NewSize;

		}
	}
}

