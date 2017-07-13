using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Server
{
    class ResultRenderRows
    {
        public Bitmap image;
        public int startPos;
        public CurrentState state;
        public ResultRenderRows( Bitmap B, int y, CurrentState S)
        {
            image = B;
            startPos = y;
            state = S;
        }
        public void DrawRow()
        {
           // gr.DrawImage(image, 0,startPos);
            this.state = CurrentState.Draw;
        }
        public enum CurrentState
        {
            notDraw,
            Draw,
        };
    }
}


