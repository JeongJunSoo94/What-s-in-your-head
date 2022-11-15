using System.Collections;
using System.Collections.Generic;
using System.Text;
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
        public GameObject canvas;
        public GameObject panel;
        readonly StringBuilder maxStr = new(20);
        readonly StringBuilder curStr = new(20);
        readonly StringBuilder minStr = new(20);
        private void Awake()
        {
            curfps = 0;
            minfps = 400f;
            maxfps = 0;
            CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();
            canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
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
            maxStr.Clear();
            maxStr.AppendFormat("{0} MAXFPS", maxfps.ToString());

            curStr.Clear();
            curStr.AppendFormat("{0} FPS", curfps.ToString());

            minStr.Clear();
            minStr.AppendFormat("{0} MINFPS", minfps.ToString());

            textMax.text = maxStr.ToString();
            textCur.text = curStr.ToString();
            textMin.text = minStr.ToString();
        }
    }
}
