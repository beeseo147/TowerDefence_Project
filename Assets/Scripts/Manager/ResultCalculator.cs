public static class ResultCalculator
{
    public static string GetBoldnessRank(int kills)
    {
        if (kills >= 50) return "S";
        if (kills >= 30) return "A";
        if (kills >= 10) return "B";
        if (kills >= 5) return "C";
        return "D";
    }

    public static string GetTimeTakenRank(float time)
    {
        if (time <= 200) return "D";
        if (time <= 220) return "S";
        if (time <= 240) return "A";
        if (time <= 260) return "B";
        if (time <= 280) return "C";
        return "D";
    }

    public static string GetItemCollectedRank(int count)
    {
        if (count >= 20) return "S";
        if (count >= 10) return "A";
        if (count >= 5) return "B";
        if (count >= 2) return "C";
        return "D";
    }

    public static string GetTotalRank(string boldness, string time, string item)
    {
        int sCount = 0;
        if (boldness == "S") sCount++;
        if (time == "S") sCount++;
        if (item == "S") sCount++;
        if (sCount >= 3) return "S";
        if (sCount == 2) return "A";
        if (sCount == 1) return "B";
        return "C";
    }

    public static int GetStarCount(string totalRank)
    {
        switch (totalRank)
        {
            case "S": return 5;
            case "A": return 4;
            case "B": return 3;
            case "C": return 2;
            case "D": return 1;
            default: return 0;
        }
    }
} 