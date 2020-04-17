using System;
using Ultraviolet.Content;
using Ultraviolet.UI;

namespace upfgame.UI.Screens
{
    public class GamePlayScreen : UIScreen
    {
        public GamePlayScreen(ContentManager globalContent)
            : base("Content/UI/Screens/GamePlayScreen", "GamePlayScreen", globalContent)
        { }

        protected override Object CreateViewModel(UIView view) => new GamePlayViewModel();
    }
}
