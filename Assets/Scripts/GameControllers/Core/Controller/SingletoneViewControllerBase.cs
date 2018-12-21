namespace GameControllers.Core.Controller
{
    public class SingletoneViewControllerBase : ViewControllerBase
    {
        public override void Init()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
