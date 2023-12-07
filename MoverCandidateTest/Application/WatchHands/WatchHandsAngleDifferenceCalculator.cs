using System;

namespace MoverCandidateTest.Application.WatchHands;

public class WatchHandsAngleDifferenceCalculator
{
    private const int FullCircleAngle = 360;
    private const int HalfCircleAngle = 180;
    
    private readonly IWatchHandsAngleCalculator _watchHandsAngleCalculator;

    public WatchHandsAngleDifferenceCalculator(IWatchHandsAngleCalculator watchHandsAngleCalculator)
    {
        _watchHandsAngleCalculator = watchHandsAngleCalculator;
    }
    
    public decimal GetLeastAnglesDifference(TimeOnly time)
    {
        var minutesHandAngle = _watchHandsAngleCalculator.GetMinutesHandAngle(time);
        var hoursHandAngle = _watchHandsAngleCalculator.GetHoursHandAngle(time);
        var anglesDifference = Math.Abs(hoursHandAngle - minutesHandAngle);

        if (anglesDifference <= HalfCircleAngle) return anglesDifference;

        return FullCircleAngle - anglesDifference;
    }
}