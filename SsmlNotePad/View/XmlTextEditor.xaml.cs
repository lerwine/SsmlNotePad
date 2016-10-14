using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Erwine.Leonard.T.SsmlNotePad.View
{
    /// <summary>
    /// Interaction logic for XmlTextEditor.xaml
    /// </summary>
    public partial class XmlTextEditor : UserControl
    {
        public XmlTextEditor()
        {
            InitializeComponent();
        }

        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            base.OnTextInput(e);
            textBlock.Text += String.Format("\r\nOnTextInput: ControlText={0}; SystemText={1}; Text={2}",
                (e.ControlText == null) ? "null" : "\"" + e.ControlText + "\"",
                (e.SystemText == null) ? "null" : "\"" + e.SystemText + "\"",
                (e.Text == null) ? "null" : "\"" + e.Text + "\"");
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            textBlock.Text += String.Format("\r\nOnKeyDown: DeadCharProcessedKey={0}; ImeProcessedKey={1}; IsDown={2}; IsRepeat={3}; IsToggled={4}; IsUp={5}; Key={6}; KeyStates={7}; SystemKey={8}",
                e.DeadCharProcessedKey.ToString("F"), e.ImeProcessedKey.ToString("F"), e.IsDown, e.IsRepeat, e.IsToggled, e.IsUp, e.Key.ToString("F"),
                e.KeyStates.ToString("F"), e.SystemKey.ToString("F"));
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            textBlock.Text += String.Format("\r\nOnKeyUp: DeadCharProcessedKey={0}; ImeProcessedKey={1}; IsDown={2}; IsRepeat={3}; IsToggled={4}; IsUp={5}; Key={6}; KeyStates={7}; SystemKey={8}",
                e.DeadCharProcessedKey.ToString("F"), e.ImeProcessedKey.ToString("F"), e.IsDown, e.IsRepeat, e.IsToggled, e.IsUp, e.Key.ToString("F"),
                e.KeyStates.ToString("F"), e.SystemKey.ToString("F"));
        }
    }
}
