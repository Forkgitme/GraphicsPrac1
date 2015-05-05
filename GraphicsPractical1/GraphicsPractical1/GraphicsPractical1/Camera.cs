using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GraphicsPractical1
{
    class Camera
    {
        private Matrix viewMatrix;
        private Matrix projectionMatrix;

        private Vector3 eye;
        private Vector3 rotation;

        public Camera(Vector3 camEye, Vector3 rotation, float aspectRatio = 4.0f / 3.0f)
        {
            // Set the vectors used to create the view matrix.
            this.eye = camEye;
            this.rotation = rotation;

            // Calculate the view and the projection matrix.
            this.updateViewMatrix();
            this.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1.0f, 300.0f);
        }

        private void updateViewMatrix()
        {
            // Make the camera look from 'eye' to 'focus', with 'up' being which way up is.

            // Take a point 1 away from the origin in the Z axis, rotate it by the camera's rotation and add the camera's position.
            // That's where the camera should look; where 'focus' should be.
            Vector3 focus = Vector3.Transform(-Vector3.UnitZ, this.RotationMatrix) + this.eye;

            // The same logic applies to 'up', but without adding the camera's position.
            Vector3 up = Vector3.Transform(Vector3.UnitY, this.RotationMatrix);

            // Finally, create the look-at matrix.
            this.viewMatrix = Matrix.CreateLookAt(this.eye, focus, up);
        }

        public Matrix ViewMatrix
        {
            get { return this.viewMatrix; }
        }

        public Matrix ProjectionMatrix
        {
            get { return this.projectionMatrix; }
        }

        public Matrix RotationMatrix
        {   // This turns the camera's rotation into a matrix and returns it.
            get { return Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationY(rotation.Y) * Matrix.CreateRotationZ(rotation.Z); }
        }

        public Vector3 Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Vector3 Eye
        {
            get { return this.eye; }
            set { this.eye = value; this.updateViewMatrix(); }
        }
    }
}
