using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    public class Button : Sprite
    {
        public Button(float x, float y):base("square.png")
        {
            SetXY(x, y);
        }

        void Update()
        {
            
        }

        public bool Click()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (HitTestPoint(Input.mouseX, Input.mouseY))
                {
                    return true;
                }
                else return false;
            }
            else return false;
        }
    }
}
