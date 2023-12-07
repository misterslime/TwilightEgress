namespace Cascade.Core.Graphics
{
    public class SmartTargetManager : ModSystem
    {
        public static List<SmartRenderTarget> SmartTargets;

        public const int TimeOfTargetDisposal = 300;

        public override void OnModLoad()
        {
            SmartTargets = new();
            Main.OnPreDraw += HandleAutoDisposal;
            On_Main.SetDisplayMode += RecreateRenderTargets;
        }

        public override void OnModUnload()
        {
            DisposeAllTargets();
            Main.OnPreDraw -= HandleAutoDisposal;
        }

        // You may use the below method as an acceptable TargetCreationDelegate parameter when creating a new SmartRenderTarget.

        /// <summary>
        /// Simple method for creating a custom RenderTarget2D easily.
        /// </summary>
        public static RenderTarget2D CreateCustomRenderTarget(int width, int height)
            => new(Main.graphics.GraphicsDevice, width, height);

        private void RecreateRenderTargets(On_Main.orig_SetDisplayMode orig, int width, int height, bool fullscreen)
        {
            if (SmartTargets is null)
                return;

            foreach (SmartRenderTarget renderTarget in SmartTargets)
            {
                // Determine if a render target is eligible to be recreated upon screen resizes or not.
                if (renderTarget is null || renderTarget.IsDisposed || !renderTarget.ShouldBeRecreatedUponScreenResize || !renderTarget.AwaitingInitialization)
                    continue;

                Main.QueueMainThreadAction(() =>
                {
                    renderTarget.RecreateTarget(width, height);
                });
            }
            orig.Invoke(width, height, fullscreen);
        }

        private void DisposeAllTargets()
        {
            if (SmartTargets is null)
                return;

            // Dispose every SmartRenderTarget and clear the list in case some targets aren't disposed of properly beforehand.
            Main.QueueMainThreadAction(() =>
            {
                foreach (SmartRenderTarget renderTarget in SmartTargets)
                    renderTarget.Dispose();
                SmartTargets.Clear();

            });
        }

        private void HandleAutoDisposal(GameTime gameTime)
        {
            if (SmartTargets is null)
                return;

            // Automatically dispose of targets that go unused for a period of time (5 seconds in our case).
            // This is done so that unused targets do not linger and take up unnecessary memeory in the GPU.
            foreach (SmartRenderTarget target in SmartTargets.ToList())
            {
                if (target is null || target.IsDisposed)
                    continue;

                target.AutoDisposalTimer++;
                if (target.AutoDisposalTimer >= TimeOfTargetDisposal)
                    target.Dispose();
            }
        }
    }
}
