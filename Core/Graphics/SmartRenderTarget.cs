namespace Cascade.Core.Graphics
{
    public class SmartRenderTarget : IDisposable
    {
        public RenderTarget2D target;

        public delegate RenderTarget2D TargetCreationDelegate(int width, int height);

        public int AutoDisposalTimer
        {
            get;
            internal set;
        }

        public bool IsDisposed
        {
            get;
            private set;
        }

        public bool ShouldBeRecreatedUponScreenResize
        {
            get;
            private set;
        }

        public TargetCreationDelegate TargetCreation
        {
            get;
            private set;
        }

        public RenderTarget2D RenderTarget
        {
            get
            {
                // Reset the auto-disposal timer.
                AutoDisposalTimer = 0;

                // Create the new RenderTarget once if necessary each time we access this.
                if (target is null || IsDisposed)
                {
                    // Automatically create a screen-sized target.
                    target = TargetCreation(Main.screenWidth, Main.screenHeight);
                    AwaitingInitialization = false;
                }

                return target;
            }
            private set => target = value;
        }

        public int Width => RenderTarget.Width;

        public int Height => RenderTarget.Height;

        public bool AwaitingInitialization = true;

        public SmartRenderTarget(TargetCreationDelegate targetCreationFunction, bool shouldBeRecreatedUponScreenResize)
        {
            TargetCreation = targetCreationFunction;
            ShouldBeRecreatedUponScreenResize = shouldBeRecreatedUponScreenResize;
            SmartTargetManager.SmartTargets.Add(this);
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;
            target?.Dispose();
            AutoDisposalTimer = 0;
            GC.SuppressFinalize(this);
        }

        // For auto recreation.
        public void RecreateTarget(int width, int height)
        {
            Dispose();
            IsDisposed = false;
            AutoDisposalTimer = 0;
            target = TargetCreation(width, height);
        }
    }
}
