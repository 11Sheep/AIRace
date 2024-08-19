using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Utils;
using Utils.Singleton;
using mixpanel;

namespace Utils
{
    public class AnalyticUtils : Singleton<AnalyticUtils>
    {
        public void Initialize()
        {
            Mixpanel.Identify(SystemInfo.deviceUniqueIdentifier);
            Mixpanel.People.Increment("num_of_sessions", 1);
        }

        public void ReportExpStep(int step)
        {
            /*
            if (!DataCollection.Instance.isAdminMode)
            {
                var props = new Value();
                props["subjectId"] = DataCollection.Instance.subjectId;
                props["atime"] = Time.time;
                props["step"] = step;
                props["experimentId"] = DataCollection.Instance.experimentId;

                Mixpanel.Track("reportStep", props);
            }
            */
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                Mixpanel.Flush();
            }
        }

        private void OnApplicationQuit()
        {
            Mixpanel.Flush();
        }
    }
}