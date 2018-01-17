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
    public interface kant
    {
        string image { set;}
    }

    [ComVisible(true), Guid("DBB8E560-98B9-4b2a-8552-4DB5159B0B33")]
    public partial class DaTagView : UserControl, kant
    {
        private const int SELECTION_BUFFER = 17;
        private Tag currentTag;
        private bool isTagging;
        private bool tagsVisible;
        private bool iconsVisible;
        private bool isVerbose;
        private ArrayList tags;
        private Image originalImage;

        private int zoom;

        private static Color COLOR_EMPTY = (Color)(new ColorConverter().ConvertFromString("#ff0000"));
        private static Color COLOR_RESERVED = (Color)(new ColorConverter().ConvertFromString("#555555"));
        private static Color COLOR_PREPAID = (Color)(new ColorConverter().ConvertFromString("#00ff00"));
        private static Color COLOR_AGREED = (Color)(new ColorConverter().ConvertFromString("#ff0000"));

        private static Brush FBRUSH_EMPTY = Brushes.Transparent;
        private static Brush FBRUSH_RESERVED = new SolidBrush(COLOR_RESERVED);
        private static Brush FBRUSH_PREPAID = new SolidBrush(COLOR_PREPAID);
        private static Brush FBRUSH_AGREED = new SolidBrush(COLOR_AGREED);

        private static Brush BRUSH_EMPTY = Brushes.Red;
        private static Brush BRUSH_RESERVED = Brushes.White;
        private static Brush BRUSH_PREPAID = Brushes.Black;
        private static Brush BRUSH_AGREED = Brushes.White;

        public DaTagView()
        {
            InitializeComponent();
            clearTags();
            zoom = 100;
        }

        public string verbose
        {
            set
            {
                isVerbose = value == "X";
            }
            get
            {
                return isVerbose ? "X" : "";
            }
        }

        public int zoomRate
        {
            set 
            { 
                zoom = value;
                paintOriginalImage();
                if (tagsVisible) showTags();
            }
            get { return zoom; }
        }

        #region TAG_STUFF

        public void addTag(int X1, int Y1, int X2, int Y2, string Text, DaTag.Tag.STATUS Status)
        {
            currentTag = new Tag();
            currentX1 = X1;
            currentY1 = Y1;
            currentX2 = X2;
            currentY2 = Y2;
            currentText = Text;
            currentStatus = Status;
            tags.Add(currentTag);
            currentTag = (Tag) tags[tags.Count - 1];
        }

        public void showTags()
        {
            if (tags == null) return;
            if (originalImage == null) return;

            hideTags();

            for (int n = 0; n < tags.Count; n++)
            {
                Tag t = (Tag)tags[n];
                paintTag(t);
            }

            tagsVisible = true;
        }

        public void hideTags()
        {
            if (originalImage == null) return;
            paintOriginalImage();
            tagsVisible = false;
        }

        public void clearTags()
        {
            tags = new ArrayList();
            hideTags();
        }

        public void removeCurrentTag()
        {
            if (tags == null) return;
            if (currentTag == null) return;

            tags.Remove(currentTag);
            if (tagsVisible) showTags();
        }

        public void removeTagByCoords(int X1, int Y1, int X2, int Y2)
        {
            if (tags == null) return;

            if (isVerbose) MessageBox.Show(" Coords sent: " + X1.ToString() + ":" + Y1.ToString() + " - " + X2.ToString() + ":" + Y2.ToString());

            for (int n = 0; n < tags.Count; n++)
            {
                Tag t = (Tag)tags[n];
                if (isVerbose) MessageBox.Show("removeTagByCoords Current Tag: " + t.xStart.ToString() + ":" + t.yStart.ToString() + " - " + t.xEnd.ToString() + ":" + t.yEnd.ToString());

                if (t.xStart == X1 && t.yStart == Y1 && t.xEnd == X2 && t.yEnd == Y2)
                {
                    if (isVerbose) MessageBox.Show("removeTagByCoords Equality found!");
                    tags.Remove(t);
                    if (tagsVisible) showTags();
                    return;
                }
            }
        }

        public void selectTagByCoords(int X1, int Y1, int X2, int Y2)
        {
            if (tags == null) return;

            if (isVerbose) MessageBox.Show(" Coords sent: " + X1.ToString() + ":" + Y1.ToString() + " - " + X2.ToString() + ":" + Y2.ToString());

            for (int n = 0; n < tags.Count; n++)
            {
                Tag t = (Tag)tags[n];
                if (isVerbose) MessageBox.Show("selectTagByCoords Current Tag: " + t.xStart.ToString() + ":" + t.yStart.ToString() + " - " + t.xEnd.ToString() + ":" + t.yEnd.ToString());

                if (t.xStart == X1 && t.yStart == Y1 && t.xEnd == X2 && t.yEnd == Y2)
                {
                    if (isVerbose) MessageBox.Show("selectTagByCoords Equality found!");
                    currentTag = t;
                    showTags();
                    paintTag(currentTag, true);
                    picMain.Refresh();
                    return;
                }
            }
        }

        public string tagsAreVisible
        {
            get
            {
                return tagsVisible ? "X" : "";
            }
        }

        public DaTag.Tag.STATUS currentStatus 
        { 
            set 
            {
                if (currentTag == null) return;
                currentTag.status = value;
            } 
        }
        public int currentX1 
        {
            set
            {
                if (currentTag == null) return;
                currentTag.xStart = value;
                if (tagsVisible) showTags();
                if (isVerbose) MessageBox.Show("set currentX1 = " + value.ToString());
            }  
            get 
            { 
                if (isTagging || currentTag == null) return 0;
                if (isVerbose) MessageBox.Show("get currentX1 = " + currentTag.xStart.ToString());
                return currentTag.xStart; 
            } 
        }
        public int currentY1 
        {
            set
            {
                if (currentTag == null) return;
                currentTag.yStart = value;
                if (tagsVisible) showTags();
                if (isVerbose) MessageBox.Show("set currentY1 = " + value.ToString());
            }  
            get 
            { 
                if (isTagging || currentTag == null) return 0;
                if (isVerbose) MessageBox.Show("get currentX1 = " + currentTag.yStart.ToString());
                return currentTag.yStart; 
            } 
        }
        public int currentX2 
        {
            set
            {
                if (currentTag == null) return;
                currentTag.xEnd = value;
                if (tagsVisible) showTags();
                if (isVerbose) MessageBox.Show("set currentX2 = " + value.ToString());
            }  
            get 
            { 
                if (isTagging || currentTag == null) return 0;
                if (isVerbose) MessageBox.Show("get currentX2 = " + currentTag.xEnd.ToString());
                return currentTag.xEnd; 
            } 
        }
        public int currentY2 
        {
            set
            {
                if (currentTag == null) return;
                currentTag.yEnd = value;
                if (tagsVisible) showTags();
                if (isVerbose) MessageBox.Show("set currentY2 = " + value.ToString());
            }  
            get 
            { 
                if (isTagging || currentTag == null) return 0;
                if (isVerbose) MessageBox.Show("get currentY2 = " + currentTag.yEnd.ToString());
                return currentTag.yEnd; 
            } 
        }
        public string currentText 
        { 
            set 
            { 
                if (currentTag == null) return;  
                currentTag.text = value; 
                if (tagsVisible) showTags(); 
            } 
            get 
            { 
                if (isTagging || currentTag == null) return ""; 
                return currentTag.text; 
            } 
        }
        public string isTagSelected { get { return (currentTag == null ? "" : "X"); } }

        public void showIcons()
        {
            iconsVisible = true;
            showTags();
        }

        public void hideIcons()
        {
            iconsVisible = false;
            showTags();
        }

        private Color getTagColor(Tag T)
        {
            switch (T.status)
            {
                case DaTag.Tag.STATUS.AGREED:
                    return COLOR_AGREED;
                    break;
                case DaTag.Tag.STATUS.EMPTY:
                    return COLOR_EMPTY;
                    break;
                case DaTag.Tag.STATUS.PREPAID:
                    return COLOR_PREPAID;
                    break;
                case DaTag.Tag.STATUS.RESERVED:
                    return COLOR_RESERVED;
                    break;
            }

            return Color.Black;
        }

        private Image getTagIcon(Tag T)
        {
            switch (T.status)
            {
                case DaTag.Tag.STATUS.AGREED:
                    return ilMain.Images[3];
                    break;
                case DaTag.Tag.STATUS.EMPTY:
                    return ilMain.Images[0];
                    break;
                case DaTag.Tag.STATUS.PREPAID:
                    return ilMain.Images[2];
                    break;
                case DaTag.Tag.STATUS.RESERVED:
                    return ilMain.Images[1];
                    break;
            }

            return null;
        }

        private Brush getTagBrush(Tag T)
        {
            switch (T.status)
            {
                case DaTag.Tag.STATUS.AGREED:
                    return BRUSH_AGREED;
                    break;
                case DaTag.Tag.STATUS.EMPTY:
                    return BRUSH_EMPTY;
                    break;
                case DaTag.Tag.STATUS.PREPAID:
                    return BRUSH_PREPAID;
                    break;
                case DaTag.Tag.STATUS.RESERVED:
                    return BRUSH_RESERVED;
                    break;
            }

            return Brushes.Black;
        }

        private Brush getTagFillBrush(Tag T)
        {
            switch (T.status)
            {
                case DaTag.Tag.STATUS.AGREED:
                    return FBRUSH_AGREED;
                    break;
                case DaTag.Tag.STATUS.EMPTY:
                    return FBRUSH_EMPTY;
                    break;
                case DaTag.Tag.STATUS.PREPAID:
                    return FBRUSH_PREPAID;
                    break;
                case DaTag.Tag.STATUS.RESERVED:
                    return FBRUSH_RESERVED;
                    break;
            }

            return Brushes.Black;
        }

        #endregion

        public string image
        {
            set
            {
                originalImage = Image.FromFile(value);
                paintOriginalImage();
                picMain.Refresh();
            }
        }

        private void paintOriginalImage()
        {
            if (zoomRate != 100)
            {
                picMain.Width = 600 * zoomRate / 100;
                picMain.Height = 400 * zoomRate / 100;
                picMain.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            else
            {
                picMain.Width = 600;
                picMain.Height = 400;
                picMain.SizeMode = PictureBoxSizeMode.Normal;
            }

            if (originalImage == null) return;

            picMain.Image = (Image)originalImage.Clone();
            /// SCROLLBAR ///
            //picMain.Width = picMain.Image.Width;
            //picMain.Height = picMain.Image.Height;
            /// SCROLLBAR ///
        }

        private void picMain_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    currentTag = new Tag();
                    currentTag.xStart = e.X;
                    currentTag.yStart = e.Y;
                    isTagging = true;
                    break;
                case MouseButtons.Right:
                    // Nothing for now
                    break;
            }
        }

        private void picMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isTagging) return;
            if (currentTag == null) return;
            if (originalImage == null) return;
            if (zoomRate != 100) return;

            currentTag.xEnd = e.X;
            currentTag.yEnd = e.Y;

            if (currentTag.xEnd - currentTag.xStart < SELECTION_BUFFER || currentTag.yEnd - currentTag.yStart < SELECTION_BUFFER)
            {
                isTagging = false;
                if (tagsVisible) showTags(); else hideTags();
                currentTag = detectTag(e);
                if (currentTag == null) return;
                paintTag(currentTag, true);
                picMain.Refresh();

                if (isVerbose) MessageBox.Show("picMain_MouseUp Current Tag: " + currentTag.xStart.ToString() + ":" + currentTag.yStart.ToString() + " - " + currentTag.xEnd.ToString() + ":" + currentTag.yEnd.ToString());

                return;
            }

            addTag(currentTag.xStart, currentTag.yStart, currentTag.xEnd, currentTag.yEnd, "?", DaTag.Tag.STATUS.EMPTY);
            currentTag = (Tag) tags[tags.Count - 1];

            isTagging = false;
            paintOriginalImage();
            showTags();
            paintTag(currentTag, true);
            picMain.Refresh();
        }

        private void picMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isTagging) return;
            if (picMain.Image == null) return;
            if (originalImage == null) return;
            if (zoomRate != 100) return;

            currentTag.xEnd = e.X;
            currentTag.yEnd = e.Y;
            currentTag.text = "";

            /// SCROLLBAR ///
            paintOriginalImage();
            paintTag(currentTag);
            picMain.Refresh();
        }

        private void paintTag(Tag T)
        {
            paintTag(T, false);
        }

        private void paintTag(Tag T, bool Fill)
        {
            float xRect, yRect, wRect, hRect;
            float xFill, yFill, wFill, hFill;
            float xText, yText;

            // Henüz görsel yoksa çýk, varsa görseli ata
            if (picMain.Image == null) return;
            Graphics g = Graphics.FromImage(picMain.Image);

            // Zoom oranýna baðlý olarak koordinatlarý ayarla
            xRect = T.xStart;
            yRect = T.yStart;
            wRect = T.xEnd - T.xStart;
            hRect = T.yEnd - T.yStart;

            xFill = xRect + 1;
            yFill = yRect + 1;
            wFill = T.xEnd - T.xStart - 1;
            hFill = T.yEnd - T.yStart - 1;

            xText = xRect + 3;
            yText = yRect + 3;

            // Çiz
            g.DrawRectangle(new Pen(getTagColor(T), 5), xRect, yRect, wRect, hRect);
            if (Fill)
            {
                g.FillRectangle(Brushes.Yellow, xFill, yFill, wFill, hFill);
            }
            else
            {
                g.FillRectangle(getTagFillBrush(T), xFill, yFill, wFill, hFill);
            }

            if (!isTagging && iconsVisible && zoomRate == 100)
            {
                Image st = getTagIcon(T);
                if (st != null)
                {
                    g.DrawImage(st, T.xEnd - st.Width - 1, T.yEnd - st.Height - 1);
                }
            }

            if (zoomRate == 100)
            {
                if (Fill)
                {
                    g.DrawString(T.text, new Font("Arial", 8), Brushes.Black, xText, yText);
                    //g.DrawString(T.status, new Font("Arial", 8), Brushes.Red, T.xStart, T.yStart + 10);
                }
                else
                {
                    g.DrawString(T.text, new Font("Arial", 8), getTagBrush(T), xText, yText);
                    //g.DrawString(T.status, new Font("Arial", 8), Brushes.Red, T.xStart, T.yStart + 10);
                }
            }
            
            g.Flush();
        }

        private Tag detectTag(MouseEventArgs e)
        {
            if (tags == null) return null;

            for (int n = 0; n < tags.Count; n++)
            {
                Tag t = (Tag)tags[n];

                if (t.xStart <= e.X && t.yStart <= e.Y && t.xEnd >= e.X && t.yEnd >= e.Y) return t;
            }

            return null;
        }
    }
}
