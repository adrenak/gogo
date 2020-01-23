using UnityEngine;

namespace Adrenak.GoGo {
    public static class GoInput {
        public enum Direction {
            North,
            South,
            West,
            East,
            None
        }

        public enum Diagonal {
            NorthEast,
            NorthWest,
            SouthWest,
            SouthEast,
            None
        }

        // GYRO
        public static Quaternion Orientation {
            get {
                return OVRInput.GetLocalControllerRotation(OVRInput.GetActiveController());

            }
        }

        public static Vector3 GetAngles() {
            float MaxAngle = 60f;
            float DeadZone = 0.2f;
            float PitchAngleOffset = 45f;

            var rotation = Orientation;
            Vector3 forward = rotation * Vector3.forward;

            var pitchProj = Vector3.ProjectOnPlane(forward, Vector3.right);

            if (pitchProj.sqrMagnitude < 0.001f) {
                forward = rotation * (forward.x > 0 ? Vector3.left : Vector3.right);
                pitchProj = Vector3.ProjectOnPlane(forward, Vector3.right);
            }

            float pitchAngle = Mathf.Atan2(pitchProj.y, pitchProj.z);

            // cancel out pitch
            rotation = Quaternion.Euler(pitchAngle * Mathf.Rad2Deg, 0, 0) * rotation;

            forward = rotation * Vector3.forward;

            var rollProj = Vector3.ProjectOnPlane(forward, Vector3.up);

            float rollAngle = Mathf.Atan2(rollProj.x, rollProj.z);

            // cancel out roll
            rotation = Quaternion.Euler(0, -rollAngle * Mathf.Rad2Deg, 0) * rotation;

            Vector3 up = rotation * Vector3.up;

            float yawAngle = Mathf.Atan2(up.x, up.y);

            const float MaxYawAngle = 150f;
            // if yaw is greater than MaxYawAngle degrees, it means this is 
            // actually probably a roll > 90 degrees.
            // go back and fix our numbers for yaw, pitch and roll.
            if (Mathf.Abs(yawAngle) > MaxYawAngle * Mathf.Deg2Rad) {
                pitchAngle += (pitchAngle < 0 ? Mathf.PI : -Mathf.PI);
                rollAngle = (Mathf.Sign(rollAngle) * Mathf.PI - rollAngle);
                yawAngle = (Mathf.Sign(yawAngle) * Mathf.PI - yawAngle);
            }

            // convert angles into -1, 1 axis range, apply dead zone
            var pitch = Mathf.Clamp((PitchAngleOffset - pitchAngle * Mathf.Rad2Deg) / MaxAngle, -1, 1);
            if (Mathf.Abs(pitch) < DeadZone)
                pitch = 0;

            var roll = Mathf.Clamp(rollAngle * Mathf.Rad2Deg / MaxAngle, -1, 1);
            if (Mathf.Abs(roll) < DeadZone)
                roll = 0;

            var yaw = Mathf.Clamp(-yawAngle * Mathf.Rad2Deg / MaxAngle, -1, 1);
            if (Mathf.Abs(yaw) < DeadZone)
                yaw = 0;

            return new Vector3(-yaw, pitch, roll);
        }

        // AXIS
        public static float Horizontal {
            get { return Axis.x; }
        }

        public static float Vertical {
            get { return Axis.y; }
        }

        public static Vector2 Axis {
            get { return OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad); }
        }

        public static float Strength {
            get { return Axis.magnitude; }
        }

        /// <summary>
        /// Returns the angle that the thumb makes on the touchpad.
        /// From 0 to 360 starting from the right
        /// </summary>
        public static float Degrees {
            get {
                var radians = Mathf.Atan2(Vertical, Horizontal);
                var degrees = Mathf.Rad2Deg * radians;
                if (degrees < 0)
                    degrees = 180 + (180 + degrees);
                return degrees;
            }
        }

        public static Direction GetDirection() {
            if (!Touch) return Direction.None;

            var degrees = Degrees;
            if (degrees > 45 && degrees < 135)
                return Direction.North;
            else if (degrees > 135 && degrees < 225)
                return Direction.West;
            else if (degrees > 225 && degrees < 315)
                return Direction.South;
            else
                return Direction.East;
        }

        public static Diagonal GetDiagonal() {
            if (!Touch) return Diagonal.None;

            var degrees = Degrees;
            if (degrees > 90)
                return Diagonal.NorthEast;
            else if (degrees > 180)
                return Diagonal.NorthWest;
            else if (degrees > 270)
                return Diagonal.SouthWest;
            else
                return Diagonal.SouthEast;
        }

        // TOUCH
        public static bool Touch {
            get { return OVRInput.Get(OVRInput.Touch.PrimaryTouchpad); }
        }

        public static bool TouchUp {
            get { return OVRInput.GetUp(OVRInput.Touch.PrimaryTouchpad); }
        }

        public static bool TouchDown {
            get { return OVRInput.GetDown(OVRInput.Touch.PrimaryTouchpad); }
        }

        // CLICK
        public static bool Click {
            get { return OVRInput.Get(OVRInput.Button.PrimaryTouchpad); }
        }

        public static bool ClickUp {
            get { return OVRInput.GetUp(OVRInput.Button.PrimaryTouchpad); }
        }

        public static bool ClickDown {
            get { return OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad); }
        }

        // TRIGGER
        public static bool Trigger {
            get { return OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger); }
        }

        public static bool TriggerUp {
            get { return OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger); }
        }

        public static bool TriggerDown {
            get { return OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger); }
        }

        // BACK
        public static bool Back {
            get { return OVRInput.Get(OVRInput.RawButton.Back); }
        }

        public static bool BackUp {
            get { return OVRInput.GetUp(OVRInput.RawButton.Back); }
        }

        public static bool BackDown {
            get { return OVRInput.GetDown(OVRInput.RawButton.Back); }
        }
    }
}
