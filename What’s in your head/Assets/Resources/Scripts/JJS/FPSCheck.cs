using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace JJS
{ 
    public class FPSCheck : MonoBehaviour
    {
        float maxfps;
        float curfps;
        float minfps;
        float maxfpsText;
        float minfpsText;
        float secDelay =1f;
        float secsDelay = 10f;
        int fpsCount=0;
        public Text textMax;
        public Text textCur;
        public Text textMin;
        private void Awake()
        {
            curfps = 0;
            minfps = 400f;
            maxfps = 0;
        }
        // Update is called once per frame
        void Update()
        {
           float delta = Time.unscaledDeltaTime;
            secDelay -= delta;
            fpsCount++;
            int fps = (int)(1f/delta);


            if (secDelay <= 0)
            {
                secsDelay += -1f + secDelay;
                if (secsDelay <= 0)
                {
                    minfpsText = 500;
                    maxfpsText = 0;
                    secsDelay = 10f;
                }
                minfps = 200;
                maxfps = 0;
                curfps = fpsCount;
                fpsCount = 0;
                secDelay = 1f;
            }
            if (fps < minfps)
            {
                minfps = fps;
            }
            if (fps > maxfps)
            {
                maxfps = fps;
            }
            if (curfps < minfpsText)
            {
                minfpsText = curfps;
            }

            if (curfps > maxfpsText)
            {
                maxfpsText = curfps;
            }

            textMax.text = maxfps.ToString() + "MAXFPS";
            textCur.text = curfps.ToString() + "FPS";
            textMin.text = minfps.ToString() + "MINFPS";
        }
    }
}
