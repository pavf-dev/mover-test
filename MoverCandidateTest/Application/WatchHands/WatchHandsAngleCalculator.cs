using System;

namespace MoverCandidateTest.Application.WatchHands;

public interface IWatchHandsAngleCalculator
{
    decimal GetHoursHandAngle(TimeOnly time);
    decimal GetMinutesHandAngle(TimeOnly time);
}

public class WatchHandsAngleCalculator : IWatchHandsAngleCalculator
{
    private const byte Hours = 12;

    private const byte OneHourAngle = 30;
    private const byte OneMinuteAngle = 6;
    private const decimal OneMinuteAsPartOfHourAngle = 0.5m;
    private const decimal OneSecondPartOfMinuteAngle = 0.1m;
    
    public decimal GetHoursHandAngle(TimeOnly time)
    {
        var watchHours = time.Hour % Hours;
        
        return watchHours * OneHourAngle + time.Minute * OneMinuteAsPartOfHourAngle;
    }

    public decimal GetMinutesHandAngle(TimeOnly time)
    {
        return time.Minute * OneMinuteAngle + time.Second * OneSecondPartOfMinuteAngle;
    }
}