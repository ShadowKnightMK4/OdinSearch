using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace FileInventoryGUI
{
    public class ImageCheckedListBox: CheckedListBox
    {
        ImageList IndexImages;
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            Image DrawThis = null;
            Rectangle Bounds = e.Bounds;
            Rectangle ImageBounds;
            string index_str = this.Items[e.Index].ToString();
            int IndexImage = -1;
            e.DrawBackground();
            base.OnDrawItem(e);

            IndexImage = IndexImages.Images.IndexOfKey(index_str);
            if (IndexImage != -1)
            {
                DrawThis= IndexImages.Images[IndexImage];
                ImageBounds = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
                Bounds.X += DrawThis.Width;
                Bounds.Width -= DrawThis.Width;

                e.Graphics.DrawImage(DrawThis, ImageBounds);
            }
            
            {
                e.Graphics.DrawString(index_str,  e.Font, Brushes.Black, Bounds, StringFormat.GenericDefault);
            }

            e.DrawFocusRectangle();
        }
    }
}
