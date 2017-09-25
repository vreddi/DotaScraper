namespace MetadataScraper
{
    public enum LaneType {
        OffLane,
        SafeLane,
        MidLane,
        Jungle
    }

    public class LanePresenceStats
    {
        public LaneType Lane { get; set; }

        public float Presence { get; set; }

        public float WinRate { get; set; }

        public float KdaRatio { get; set; }

        public float Gpm { get; set; }

        public float Xpm { get; set; }

        /// <summary>
        /// Provides the correct lane type based on the text
        /// </summary>
        /// <param name="text">Client text</param>
        /// <returns></returns>
        public static LaneType? GetLaneTypeFromString(string text)
        {
            text = text.ToLower();

            switch (text)
            {
                case "off lane":
                    return LaneType.OffLane;

                case "safe lane":
                    return LaneType.SafeLane;

                case "mid lane":
                    return LaneType.MidLane;

                case "jungle":
                    return LaneType.Jungle;

                default:
                    return null;
            }
        }
    }
}
