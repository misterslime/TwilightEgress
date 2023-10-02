namespace Cascade.Core.Graphics
{
    public struct VertexPosition2DColor : IVertexType
    {
        public Vector2 Position;
        public Color Color;
        public Vector2 TextureCoordinates;
        public VertexDeclaration VertexDeclaration => vertexDeclaration;

        private static readonly VertexDeclaration vertexDeclaration = new VertexDeclaration(new VertexElement[]
        {
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
            new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        });

        public VertexPosition2DColor(Vector2 position, Color color, Vector2 textureCoordinates)
        {
            Position = position;
            Color = color;
            TextureCoordinates = textureCoordinates;
        }
    }

    public class PrimitiveDrawingSystem
    {
        public delegate float VertexWidthFunction(float completionRatio);

        public delegate Color VertexColorFunction(float completionRatio);

        public VertexWidthFunction WidthFunction;

        public VertexColorFunction ColorFunction;

        public BasicEffect BasicEffect;

        public MiscShaderData Shader;

        public bool UseSmoothening;

        /// <summary>
        /// The constructor. Call this to initialize a new instance of a <see cref="PrimitiveDrawingSystem"/>. Cache this as a field.
        /// </summary>
        /// <param name="widthFunction">The delegate method for setting the width of a primitive.</param>
        /// <param name="colorFunction">The delegate method for setting the color of a primitive.</param>
        /// <param name="useeSmoothening">Whether or not vertex points should be smoothly connected or not. Typically you'd always want this enabled.</param>
        /// <param name="shader">A special shader that will be drawn on top of the Primitive being drawn.</param>
        public PrimitiveDrawingSystem(VertexWidthFunction widthFunction, VertexColorFunction colorFunction, bool useeSmoothening = false, MiscShaderData shader = null)
        {
            if (widthFunction is null || colorFunction is null)
                throw new NullReferenceException($"In order to create a new instance of the Primitive Drawing System, a non-null {(widthFunction is null ? "width" : "color")} must be given.");

            WidthFunction = widthFunction;
            ColorFunction = colorFunction;
            UseSmoothening = useeSmoothening;
            Shader = shader;

            BasicEffect = new BasicEffect(Main.instance.GraphicsDevice)
            {
                VertexColorEnabled = true,
                TextureEnabled = false,
            };

            UpdateBasicEffect(out _, out _);
        }

        private void UpdateBasicEffect(out Matrix effectProjection, out Matrix effectView)
        {
            // Screeen bounds.
            int height = Main.instance.GraphicsDevice.Viewport.Height;

            Vector2 zoom = Main.GameViewMatrix.Zoom;
            Matrix zoomScaleMatrix = Matrix.CreateScale(zoom.X, zoom.Y, 1f);

            // Get a matrix that aims towards thee Z axis.
            // These calculations are relative to a 2D world.
            effectView = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up);

            // Offset the matrix to the appropriate position.
            effectView *= Matrix.CreateTranslation(0, -height, 0);

            // Rotate the matrix by 180 degrees.
            effectView *= Matrix.CreateRotationZ(Pi);

            // Accounting for inverted gravity.
            if (Main.LocalPlayer.gravDir == -1)
                effectView *= Matrix.CreateScale(1f, -1f, 1f) * Matrix.CreateTranslation(0, height, 0);

            // And finally, account for the current level of zoom.
            effectView *= zoomScaleMatrix;

            effectProjection = Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth * zoom.X, 0f, Main.screenHeight * zoom.Y, 0f, 1f) * zoomScaleMatrix;
            BasicEffect.View = effectView;
            BasicEffect.Projection = effectProjection;
        }

        private VertexPosition2DColor[] CreatePrimitiveVertices(List<Vector2> points)
        {
            List<VertexPosition2DColor> rectPoints = new List<VertexPosition2DColor>();

            // Loop throught all points, excluding the final one because it doesn't need to connect to anything.
            for (int i = 0; i < points.Count - 1; i++)
            {
                float completionRatio = i / (float)points.Count;

                // Get the current width and color.
                float width = WidthFunction(completionRatio);
                Color color = ColorFunction(completionRatio);

                // Get the current point and point ahead.
                Vector2 point = points[i];
                Vector2 pointAhead = points[i + 1];

                // Get the direction ahead.
                Vector2 directionAhead = (pointAhead - points[i]).SafeNormalize(Vector2.Zero);

                // Get the left and right coordinates, with the current completion for the X value. 
                Vector2 leftCurrentTextureCoordiante = new Vector2(completionRatio, 0f);
                Vector2 rightCurrentTextureCoordinate = new Vector2(completionRatio, 1f);

                // Point 90 degrees away from the direction towards the next point, and use it to mark the edges of a rectangle.
                Vector2 sideDirection = new Vector2(-directionAhead.Y, directionAhead.X);

                // What this is doing, at its core, is making a rectangle out of two triangles.
                // See https://cdn.discordapp.com/attachments/770382926545813515/1050185533780934766/a.png for a visual of this.
                // The two triangles can be imagined as the point being the the tip and the sides being the opposite side.
                // How to connect it all is defined in CreatePrimitiveIndices().
                rectPoints.Add(new VertexPosition2DColor(point - sideDirection * width, color, leftCurrentTextureCoordiante));
                rectPoints.Add(new VertexPosition2DColor(point + sideDirection * width, color, rightCurrentTextureCoordinate));
            }

            return rectPoints.ToArray();
        }

        private static short[] CreatePrimitiveIndices(int totalPoints)
        {
            // What this method does is represents each point on the vertices list as indices.
            // These indices should come together to create a tiny rectangle, which then acts as a segment
            // on a full primitive drawing, say a trail for example. This is achieved by splitting the indices
            // (or points, if you rather) into 2 triangles, which requires 6 points. This is the aformentioned connecting
            // of triangles using indices.

            // Get the total number of indices, -1 because the last indice doesn't have to connect to anything
            // and * 6 becausee we need 6 indices for each point.
            int totalIndices = (totalPoints - 1) * 6;

            // Create an array to hold them with the correct size.
            short[] indices = new short[totalIndices];

            // Loop through the points, creating each indice
            for (int i = 0; i < totalPoints - 2; i++)
            {
                // This will probably look confusing, but to break it down, its basically going around the rectangle and adding points
                // in the appropriate place.
                // Use this as a visual aid. https://cdn.discordapp.com/attachments/864078125657751555/1050218596623716413/image.png
                int startingTriangleIndex = i * 6;
                int connectToIndex = i * 2;
                indices[startingTriangleIndex] = (short)connectToIndex;
                indices[startingTriangleIndex + 1] = (short)(connectToIndex + 1);
                indices[startingTriangleIndex + 2] = (short)(connectToIndex + 2);
                indices[startingTriangleIndex + 3] = (short)(connectToIndex + 2);
                indices[startingTriangleIndex + 4] = (short)(connectToIndex + 1);
                indices[startingTriangleIndex + 5] = (short)(connectToIndex + 3);
            }

            return indices;
        }

        private List<Vector2> CorrectlyOffsetPoints(List<Vector2> basePoints, Vector2 baseOffset, int totalPoints)
        {
            // If smoothening isn't being used...
            if (!UseSmoothening)
            {
                List<Vector2> points = new List<Vector2>();
                if (basePoints.Count > 0)
                {
                    // Only try to if there's any in the given list.
                    for (int i = 0; i < basePoints.Count; i++)
                    {
                        // Get the current point, add the offset to it and add it to the current list.
                        points.Add(basePoints[i] + baseOffset);
                    }
                }

                return points;
            }
            else
            {
                List<Vector2> newList = new List<Vector2>();
                for (int i = 0; i < basePoints.Count; i++)
                {
                    // Do not incorperate points that are zeroed out.
                    // They are almost certainly a result of incomplete oldPos arrays.
                    if (basePoints.ElementAt(i) == Vector2.Zero)
                        continue;

                    newList.Add(basePoints.ElementAt(i) + baseOffset);
                }

                // Avoding any index errors.
                if (newList.Count <= 1)
                    return newList;

                List<Vector2> points = new();

                // Round up the point count to the nearest multiple of the position count, to ensure that the interpolant works.
                int splineIterations = (int)Math.Ceiling(totalPoints / (double)newList.Count);
                totalPoints = splineIterations * totalPoints;

                for (int i = 1; i < newList.Count - 2; i++)
                {
                    for (int j = 0; j < splineIterations; j++)
                    {
                        float splineInterpolant = j / (float)splineIterations;
                        if (splineIterations <= 1f)
                            splineInterpolant = 0.5f;

                        points.Add(Vector2.CatmullRom(newList[i - 1], newList[i], newList[i + 1], newList[i + 2], splineInterpolant));
                    }
                }

                // Manually insert the front and end points.
                points.Insert(0, newList.First());
                points.Add(newList.Last());

                return points;
            }
        }

        /// <summary>
        /// Creates and draws primitives from the given points.
        /// </summary>
        /// <param name="basePoints">The base positions.</param>
        /// <param name="baseOffset">The base offset. Should commonly be set to -<see cref="Main.screenPosition"/>.</param>
        /// <param name="totalPoints">The total number of points in the primitive. Should only be used if smoothening is.</param>
        public void DrawPrimitives(List<Vector2> basePoints, Vector2 baseOffset, int totalPoints)
        {
            // Set the corrct Rasterizer State.
            Main.instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            // First, we offfset the points by the base offset. This is almost always going to be -Main.screenPosition, but it is changeable for flexability.
            List<Vector2> drawPoints = CorrectlyOffsetPoints(basePoints, baseOffset, totalPoints);

            // If the list is too short, any points in it are NaNs, or they are all the same point, return.
            if (drawPoints.Count < 2f || drawPoints.Any((drawPoint) => drawPoint.HasNaNs()) || drawPoints.All(point => point == drawPoints[0]))
                return;

            // Updating the basic effect.
            UpdateBasicEffect(out Matrix projection, out Matrix view);

            // Get an array of primitive triangles to pass through. Color data etc is stored in the struct.
            VertexPosition2DColor[] pointVertices = CreatePrimitiveVertices(drawPoints);
            // Get an array of indices for each primitive triangle.
            short[] triangleIndices = CreatePrimitiveIndices(drawPoints.Count);

            // If these are too short, or the indices aren't completed, return.
            if (triangleIndices.Length % 6 != 0 || pointVertices.Length <= 3)
                return;

            // If there is a shader being used, we set the correct view and apply it.
            if (Shader != null)
            {
                Shader.Shader.Parameters["uWorldViewProjection"].SetValue(view * projection);
                Shader.Apply();
            }
            // Otherwise, we apply the basic effect.
            else
                BasicEffect.CurrentTechnique.Passes[0].Apply();

            // The fun part! The part where we draw the prims.
            // We specify the type of PrimitiveType this should be expecting, and pass through an array of the struct using the correct interface.
            // We also apply the main pixel shader.
            Main.instance.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, pointVertices, 0, pointVertices.Length, triangleIndices, 0, triangleIndices.Length / 3);
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }
    }
}
