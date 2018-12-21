namespace GameControllers.Core.Controller
{
    public abstract class GameViewControllerBase : ViewControllerBase
    {
        public virtual bool IsEnabled
        {
            get { return gameObject.activeSelf; }
        }
        
        public virtual void Enable()
        {
            if (!IsEnabled)
            {
                gameObject.SetActive(true);
            }
        }

        public virtual void Disable()
        {
            if (IsEnabled)
            {
                gameObject.SetActive(false);
                ClearData();
            }
        }
        //<summary>Called when Component is Disabled</summary>
        public virtual void ClearData() { }
    }
}
