using System;
using Ultraviolet.Presentation;

namespace UpfGame.UI.Screens
{
    public sealed class GamePlayViewModel
    {
        public void HandleButtonClicked(DependencyObject element, RoutedEventData data) => Counter++;
        public Int32 Counter { get; set; }
    }
}
