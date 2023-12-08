namespace Cascade.Content.Events
{
    public abstract class EventHandler : ModSystem
    {
        /// <summary>
        /// The condition which determins whether or not the event is active.
        /// </summary>
        public abstract bool EventIsActive { get; }

        public override void OnWorldLoad() => ResetEventStuff();

        public override void OnWorldUnload() => ResetEventStuff();

        public override void PostUpdateNPCs()
        {
            if (EventIsActive)
                UpdateEvent();
        }

        /// <summary>
        /// Override to run any code related to your custom event.
        /// </summary>
        public virtual void UpdateEvent() { }

        /// <summary>
        /// Override to place any code related to your custom event that should be reset upon loading and unloading a world. 
        /// </summary>
        public virtual void ResetEventStuff() { }
    }
}
