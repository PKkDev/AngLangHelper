using CommunityToolkit.Mvvm.ComponentModel;

namespace WinUITestParser.MVVM.Model
{
    public class SubPanel : ObservableObject
    {
        private int _height;
        public int Height { get => _height; set => SetProperty(ref _height, value); }

        private string _glyph;
        public string Glyph { get => _glyph; set => SetProperty(ref _glyph, value); }

        private bool IsOpen { get; set; }

        public SubPanel()
        {
            Close();
        }

        public void Open()
        {
            Height = 150;
            Glyph = "\uE972";
            IsOpen = true;
        }

        public void Close()
        {
            Height = 30;
            Glyph = "\uE971";
            IsOpen = false;
        }

        public void ChangeState()
        {
            if (IsOpen)
                Close();
            else
                Open();
        }
    }
}
