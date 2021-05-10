using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    public class Button : AnimationSprite
    {
        public bool active = true;

        public Button(float x, float y, string texture = "square.png", float scale = 0.5f):base(texture, 1, 3)
        {
            SetXY(x, y);
            SetOrigin(width / 2, height / 2);
            this.scale = scale;
        }

        void Update()
        {
            if (active)
            {
                if (HitTestPoint(Input.mouseX, Input.mouseY))
                {
                    if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) SetFrame(2);
                    else SetFrame(1);
                }
                else SetFrame(0);
            }
        }

        public bool Click()
        {
            if (active)
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
            else return false;
        }
    }
}
