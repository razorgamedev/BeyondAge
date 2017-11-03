﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondAge.Graphics
{
    class Camera
    {
        // Construct a new Camera class with standard zoom (no scaling)
        public Camera()
        {
            Zoom = 1.0f;
        }

        // Centered Position of the Camera in pixels.
        public Vector2 Position { get; set; }
        // Current Zoom level with 1.0f being standard
        public float Zoom { get; set; }
        // Current Rotation amount with 0.0f being standard orientation
        public float Rotation { get; set; }

        // Height and width of the viewport window which we need to adjust
        // any time the player resizes the game window.
        public int ViewportWidth { get; set; }
        public int ViewportHeight { get; set; }

        public Rectangle Bounds { get; set; } = Rectangle.Empty;

        public float Width { 
            get {
                if (Zoom == 0) return ViewportWidth;
                return ViewportWidth / Zoom; 
            } 
        }

        public float Height
        {
            get {
                if (Zoom == 0) return ViewportHeight;
                return ViewportHeight / Zoom;
            }
        }

        // Center of the Viewport which does not account for scale
        public Vector2 ViewportCenter
        {
            get {
                return new Vector2(ViewportWidth * 0.5f, ViewportHeight * 0.5f);
            }
        }

        // Create a matrix for the camera to offset everything we draw,
        // the map and our objects. since the camera coordinates are where
        // the camera is, we offset everything by the negative of that to simulate
        // a camera moving. We also cast to integers to avoid filtering artifacts.
        public Matrix TranslationMatrix
        {
            get {
                return Matrix.CreateTranslation(-(int)Position.X,
                   -(int)Position.Y, 0) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                   Matrix.CreateTranslation(new Vector3(ViewportCenter, 0));
            }
        }

        // Call this method with negative values to zoom out
        // or positive values to zoom in. It looks at the current zoom
        // and adjusts it by the specified amount. If we were at a 1.0f
        // zoom level and specified -0.5f amount it would leave us with
        // 1.0f - 0.5f = 0.5f so everything would be drawn at half size.
        public void AdjustZoom(float amount)
        {
            Zoom += amount;
            if (Zoom < 0.25f)
            {
                Zoom = 0.25f;
            }
        }

        // Move the camera in an X and Y amount based on the cameraMovement param.
        // if clampToMap is true the camera will try not to pan outside of the
        // bounds of the map.
        public void MoveCamera(Vector2 cameraMovement)
        {
            Vector2 newPosition = Position + cameraMovement;

            Position = ClampToBounds(newPosition);
        }

        public Rectangle ViewportWorldBoundry()
        {
            Vector2 viewPortCorner = ScreenToWorld(new Vector2(0, 0));
            Vector2 viewPortBottomCorner =
               ScreenToWorld(new Vector2(ViewportWidth, ViewportHeight));

            return new Rectangle((int)viewPortCorner.X,
               (int)viewPortCorner.Y,
               (int)(viewPortBottomCorner.X - viewPortCorner.X),
               (int)(viewPortBottomCorner.Y - viewPortCorner.Y));
        }

        private Vector2 ClampToBounds(Vector2 newPosition)
        {
            if (Bounds != Rectangle.Empty)
            {
                if (newPosition.X - Width / 2 < Bounds.X) newPosition.X = Bounds.X + Width / 2;
                if (newPosition.Y - Height / 2 < Bounds.Y) newPosition.Y = Bounds.Y + Height / 2;
                
                if (newPosition.X + Width / 2 > Bounds.X + Bounds.Width)
                    newPosition.X = Bounds.X + Bounds.Width - Width / 2;

                if (newPosition.Y + Height / 2 > Bounds.Y + Bounds.Height)
                    newPosition.Y = Bounds.Y + Bounds.Height - Height / 2;

                //Console.WriteLine(ViewportWidth);
            }

            return newPosition;
        }

        // Center the camera on specific pixel coordinates
        public void CenterOn(Vector2 position)
        {
            Position = ClampToBounds(position);
        }
        
        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, TranslationMatrix);
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition,
                Matrix.Invert(TranslationMatrix));
        }
    }
}
