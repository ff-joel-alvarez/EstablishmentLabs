using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerService
{
    

    public class ServiceConfig
    {

        public ServiceConfig(int screenshotsToTakePerSecond=1, int tilesSize=1, int tilesNumber=1, string imageResolution="") {
            ScreenshotsToTakePerSecond = screenshotsToTakePerSecond;
            TilesSize = tilesSize;
            TilesNumber = tilesNumber;
            ImageResolution = imageResolution;
        }



        public int ScreenshotsToTakePerSecond { get; set; }

        public int TilesSize { get; set; }

        public int TilesNumber { get; set; }

        public String ImageResolution { get; set; }


    }
}
