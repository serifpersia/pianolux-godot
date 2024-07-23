using Godot;
using System;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing; // For System.Drawing.Bitmap
using System.IO; // For MemoryStream

public partial class WebcamStreaming : TextureRect
{
	private VideoCapture _videoCapture;
	private float _frameInterval = 1.0f / 60.0f;
	private double _timeSinceLastFrame = 0.0f;

	public override void _Ready()
	{
		_videoCapture = new VideoCapture(0);
		_videoCapture.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, 640);
		_videoCapture.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, 480);
		_videoCapture.Set(Emgu.CV.CvEnum.CapProp.Fps, 60);
	}

	public override void _Process(double delta)
	{
		// Accumulate elapsed time
		_timeSinceLastFrame += delta;

		// Check if the time since the last frame is greater than or equal to the frame interval
		if (_timeSinceLastFrame >= _frameInterval)
		{
			_timeSinceLastFrame -= _frameInterval; // Reset time accumulator
			CaptureAndSetFrame();
		}
	}

	private void CaptureAndSetFrame()
	{
		Mat frame = new Mat();
		_videoCapture.Read(frame);

		// Check if the frame is not empty
		if (!frame.IsEmpty)
		{
			try
			{
				// Convert Mat to Bitmap
				System.Drawing.Bitmap bitmap = frame.ToBitmap();

				// Convert Bitmap to Godot Image
				Godot.Image img = ConvertBitmapToGodotImage(bitmap);

				// Create an ImageTexture from the Godot Image
				Texture = ImageTexture.CreateFromImage(img);
			}
			catch (Exception ex)
			{
				GD.PrintErr($"Error processing frame: {ex.Message}");
			}
		}
		else
		{
			GD.Print("Failed to capture frame or frame is empty.");
		}
	}

	private Godot.Image ConvertBitmapToGodotImage(System.Drawing.Bitmap bitmap)
	{
		// Convert Bitmap to byte array
		byte[] imageBytes;
		using (var memoryStream = new MemoryStream())
		{
			bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
			imageBytes = memoryStream.ToArray();
		}

		// Convert byte array to Godot Image
		Godot.Image img = new Godot.Image();
		img.LoadPngFromBuffer(imageBytes);
		return img;
	}

	public override void _ExitTree()
	{
		// Cleanup
		_videoCapture?.Dispose();
	}
}
