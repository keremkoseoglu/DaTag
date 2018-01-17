using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace DaTag
{
    public interface kant2
    {
        string dummy { get; }
    }

    [ComVisible(true), Guid("BCE04B16-A7BC-4c39-9AF6-547467BD8F0C")]
    public partial class DaTagMulti : UserControl, kant2
    {
        private ArrayList datags;
        private ArrayList labels;

        public string dummy
        {
            get { return ""; }
        }

        public DaTagMulti()
        {
            InitializeComponent();

            clear();
        }

        public void addImage(string Path, string Text)
        {
            DaTagView dtv = new DaTagView();
            dtv.image = Path;
            datags.Add(dtv);

            Label l = new Label();
            l.Text = Text;
            labels.Add(l);
        }

        public void addTag(int X1, int Y1, int X2, int Y2, string Text, DaTag.Tag.STATUS Status)
        {
            int n = datags.Count;
            if (n <= 0) return; 

            DaTagView dtv = (DaTagView)datags[n - 1];
            dtv.addTag(X1, Y1, X2, Y2, Text, Status);
        }

        public void clear()
        {
            datags = new ArrayList();
            labels = new ArrayList();

            foreach (Control c in this.Controls)
            {
                c.Visible = false;
                c.Dispose();
            }

            this.PerformLayout();
            this.Refresh();
            Application.DoEvents();
        }

        public void repaint()
        {
            int top = 1;

            for (int n = 0; n < datags.Count; n++)
            {
                DaTagView dtv = (DaTagView)datags[n];
                dtv.Parent = this;
                dtv.Top = top;
                dtv.Left = 1;
                dtv.Width = 150;
                dtv.Height = 100;
                dtv.zoomRate = 25;
                dtv.showTags();
                dtv.Show();
                dtv.Visible = true;

                Label l = (Label)labels[n];
                l.Parent = this;
                l.Top = top;
                l.Left = 151;
                l.Show();
                l.Visible = true;

                top += 150;
            }
        }
    }
}
