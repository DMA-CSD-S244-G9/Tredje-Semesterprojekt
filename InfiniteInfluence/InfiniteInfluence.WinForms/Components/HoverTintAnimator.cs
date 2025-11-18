using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace InfiniteInfluence.WinFormsApp.Components
{
    /// <summary>
    /// Provides a hover tint animation for PictureBox controls.
    /// When the mouse enters a PictureBox, the image is smoothly tinted towards a target color.
    /// When the mouse leaves, the tint is smoothly removed.
    /// </summary>
    public static class HoverTintAnimator
    {
        /// <summary>
        /// Holds animation state for a single PictureBox.
        /// </summary>
        private class PictureBoxAnimationState
        {
            /// <summary>
            /// The original, untinted image of the PictureBox.
            /// </summary>
            public Image OriginalImage;

            /// <summary>
            /// Current animation progress in the range [0..1].
            /// 0 = no tint, 1 = fully tinted.
            /// </summary>
            public float CurrentProgress;

            /// <summary>
            /// Target animation progress in the range [0..1].
            /// The animation moves CurrentProgress towards this value over time.
            /// </summary>
            public float TargetProgress;

            /// <summary>
            /// Timer that drives the animation by updating CurrentProgress at a fixed interval.
            /// </summary>
            public System.Windows.Forms.Timer AnimationTimer;

            /// <summary>
            /// Total duration of a full animation from 0 to 1 (or 1 to 0) in milliseconds.
            /// </summary>
            public int DurationInMilliseconds;

            /// <summary>
            /// The color used as the tint target when hovering.
            /// </summary>
            public Color TargetTintColor;
        }


        /// <summary>
        /// Keeps track of all PictureBox controls that have hover tint animation attached.
        /// Each PictureBox maps to its animation state.
        /// </summary>
        private static readonly Dictionary<PictureBox, PictureBoxAnimationState> _pictureBoxAnimationStates = new();


        /// <summary>
        /// Attaches hover tint animation behavior to a PictureBox.
        /// When the mouse enters, the image is smoothly tinted towards the specified target color.
        /// When the mouse leaves, the tint is smoothly removed.
        /// </summary>
        /// <param name="pictureBox">The PictureBox to animate.</param>
        /// <param name="targetTintColor">The color to tint towards on hover.</param>
        /// <param name="durationInMilliseconds">The duration of the animation in milliseconds.</param>
        public static void Attach(PictureBox pictureBox, Color targetTintColor, int durationInMilliseconds = 1000)
        {
            // Do nothing if the PictureBox is null.
            if (pictureBox == null)
            {
                return;
            }

            // If the PictureBox is already tracked, just update its target color and duration.
            if (_pictureBoxAnimationStates.ContainsKey(pictureBox))
            {
                UpdateExistingState(pictureBox, targetTintColor, durationInMilliseconds);
                return;
            }

            // Create a new animation state for this PictureBox.
            var animationState = new PictureBoxAnimationState
            {
                // Store the original image so we can re-draw it with different color tints.
                OriginalImage = pictureBox.Image,

                // Start with no tint applied.
                CurrentProgress = 0f,

                // Target is also no tint initially.
                TargetProgress = 0f,

                // Ensure the duration is at least 1 ms to avoid division by zero.
                DurationInMilliseconds = Math.Max(1, durationInMilliseconds),

                // Store the desired tint color.
                TargetTintColor = targetTintColor,

                // Create a timer that ticks ~60 times per second (16 ms interval).
                AnimationTimer = new System.Windows.Forms.Timer { Interval = 16 }
            };

            // Disable the built-in PictureBox image drawing, since we will draw the image manually in the Paint event.
            pictureBox.Image = null;

            // Ensure the image is scaled correctly within the PictureBox bounds.
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            // Subscribe to the PictureBox events required for the animation.
            pictureBox.Paint += PictureBox_Paint;
            pictureBox.MouseEnter += PictureBox_MouseEnter;
            pictureBox.MouseLeave += PictureBox_MouseLeave;

            // Subscribe to the timer Tick event to update the animation progress over time.
            animationState.AnimationTimer.Tick += (sender, eventArgs) =>
            {
                // Compute how much CurrentProgress should change per tick, based on total duration.
                float progressStep = animationState.AnimationTimer.Interval / (float)animationState.DurationInMilliseconds;

                // If CurrentProgress is within one step of TargetProgress, snap to TargetProgress and stop the timer.
                if (Math.Abs(animationState.CurrentProgress - animationState.TargetProgress) <= progressStep)
                {
                    animationState.CurrentProgress = animationState.TargetProgress;
                    animationState.AnimationTimer.Stop();
                }
                else
                {
                    // Otherwise move CurrentProgress in the direction of TargetProgress by one step.
                    animationState.CurrentProgress += Math.Sign(animationState.TargetProgress - animationState.CurrentProgress) * progressStep;
                }

                // Request a repaint of the PictureBox to reflect the updated tint.
                pictureBox.Invalidate();
            };

            // Store the new state so we can access it later from event handlers.
            _pictureBoxAnimationStates[pictureBox] = animationState;
        }


        /// <summary>
        /// Detaches hover tint animation behavior from a PictureBox
        /// and restores its original image and event handlers.
        /// </summary>
        /// <param name="pictureBox">The PictureBox to detach from.</param>
        public static void Detach(PictureBox pictureBox)
        {
            // If the PictureBox is null or not being tracked, there is nothing to do.
            if (pictureBox == null || !_pictureBoxAnimationStates.TryGetValue(pictureBox, out var animationState))
            {
                return;
            }

            // Unsubscribe from the custom paint and mouse events.
            pictureBox.Paint -= PictureBox_Paint;
            pictureBox.MouseEnter -= PictureBox_MouseEnter;
            pictureBox.MouseLeave -= PictureBox_MouseLeave;

            // Stop and dispose of the animation timer.
            animationState.AnimationTimer?.Stop();
            animationState.AnimationTimer?.Dispose();

            // Restore the original image to the PictureBox.
            pictureBox.Image = animationState.OriginalImage;

            // Remove the PictureBox from the tracking dictionary.
            _pictureBoxAnimationStates.Remove(pictureBox);
        }


        /// <summary>
        /// Updates an existing animation state for a PictureBox with a new
        /// target color and animation duration.
        /// </summary>
        /// <param name="pictureBox">The PictureBox that already has an attached state.</param>
        /// <param name="targetTintColor">The new target tint color.</param>
        /// <param name="durationInMilliseconds">The new animation duration in milliseconds.</param>
        private static void UpdateExistingState(PictureBox pictureBox, Color targetTintColor, int durationInMilliseconds)
        {
            // Retrieve the current state for this PictureBox.
            var animationState = _pictureBoxAnimationStates[pictureBox];

            // Update the tint color and animation duration.
            animationState.TargetTintColor = targetTintColor;
            animationState.DurationInMilliseconds = Math.Max(1, durationInMilliseconds);
        }


        /// <summary>
        /// MouseEnter event handler for a PictureBox.
        /// Starts the animation towards a fully tinted state.
        /// </summary>
        private static void PictureBox_MouseEnter(object sender, EventArgs eventArgs)
        {
            // Cast the sender back to a PictureBox.
            var pictureBox = (PictureBox)sender;

            // Retrieve the animation state for this PictureBox.
            var animationState = _pictureBoxAnimationStates[pictureBox];

            // Set the target progress to 1 (fully tinted).
            animationState.TargetProgress = 1f;

            // Start the timer so the animation begins.
            animationState.AnimationTimer.Start();
        }


        /// <summary>
        /// MouseLeave event handler for a PictureBox.
        /// Starts the animation back towards the original, non-tinted state.
        /// </summary>
        private static void PictureBox_MouseLeave(object sender, EventArgs eventArgs)
        {
            // Cast the sender back to a PictureBox.
            var pictureBox = (PictureBox)sender;

            // Retrieve the animation state for this PictureBox.
            var animationState = _pictureBoxAnimationStates[pictureBox];

            // Set the target progress to 0 (no tint).
            animationState.TargetProgress = 0f;

            // Start the timer so the animation begins.
            animationState.AnimationTimer.Start();
        }


        /// <summary>
        /// Paint event handler for a PictureBox.
        /// Draws the original image with a color matrix applied to create the tint effect.
        /// </summary>
        private static void PictureBox_Paint(object sender, PaintEventArgs paintEventArgs)
        {
            // Cast the sender back to a PictureBox.
            var pictureBox = (PictureBox)sender;

            // If we cannot find a state or there is no original image, do nothing.
            if (!_pictureBoxAnimationStates.TryGetValue(pictureBox, out var animationState) || animationState.OriginalImage == null)
            {
                return;
            }

            // Compute the target color components as normalized values (0..1).
            // We assume the original image is white (1,1,1) and multiply towards the target color.
            float targetRed = animationState.TargetTintColor.R / 255f;
            float targetGreen = animationState.TargetTintColor.G / 255f;
            float targetBlue = animationState.TargetTintColor.B / 255f;

            // Linearly interpolate (lerp) between white (1) and the target color
            // based on the current animation progress.
            float redScale = Lerp(1f, targetRed, animationState.CurrentProgress);
            float greenScale = Lerp(1f, targetGreen, animationState.CurrentProgress);
            float blueScale = Lerp(1f, targetBlue, animationState.CurrentProgress);

            // Build a color matrix that scales the RGB channels based on the calculated factors.
            var colorMatrix = new ColorMatrix(new[]
            {
                new float[] { redScale,   0,          0,          0, 0 },
                new float[] { 0,          greenScale, 0,          0, 0 },
                new float[] { 0,          0,          blueScale,  0, 0 },
                new float[] { 0,          0,          0,          1, 0 },
                new float[] { 0,          0,          0,          0, 1 }
            });

            // Create an ImageAttributes object and apply the color matrix.
            using var imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            // Use high-quality interpolation for scaling the image.
            paintEventArgs.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Destination rectangle is the full client area of the PictureBox.
            var destinationRectangle = pictureBox.ClientRectangle;

            // Draw the original image into the destination rectangle,
            // applying the color matrix to create the tint.
            paintEventArgs.Graphics.DrawImage(
                animationState.OriginalImage,
                destinationRectangle,
                0,
                0,
                animationState.OriginalImage.Width,
                animationState.OriginalImage.Height,
                GraphicsUnit.Pixel,
                imageAttributes
            );
        }


        /// <summary>
        /// Linearly interpolates between two values startValue and endValue by factor interpolationFactor.
        /// When interpolationFactor = 0, returns startValue. When interpolationFactor = 1, returns endValue.
        /// </summary>
        /// <param name="startValue">Start value.</param>
        /// <param name="endValue">End value.</param>
        /// <param name="interpolationFactor">Interpolation factor in the range [0..1].</param>
        private static float Lerp(float startValue, float endValue, float interpolationFactor)
        {
            return startValue + (endValue - startValue) * interpolationFactor;
        }
    }
}
