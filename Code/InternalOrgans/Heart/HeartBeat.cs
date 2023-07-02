using System;
using System.Collections.Generic;
using System.Text;

namespace Get_Text
{
    interface Heart
    {
        /// <summary>
        /// 一次心跳
        /// </summary>
        void OneBeat();
    }

    class HeartBeat:Heart
    {
        int status = 0;
        public void OneBeat()
        {

        }
    }
}
