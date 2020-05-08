using System.Windows.Forms;

namespace Andrique.Utils.NatureSound
{
    internal partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public WebBrowser Browser
        {
            get { return _webBrowser; }
        }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            _webBrowser.ScriptErrorsSuppressed = true;

            _webBrowser.Navigate("ecosounds.net/dozhd/zvuk-i-shum-dozhdya/");
        }
    }
}
