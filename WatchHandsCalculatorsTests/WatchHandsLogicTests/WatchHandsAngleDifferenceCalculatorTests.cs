using System;
using MoverCandidateTest.Application.WatchHands;
using NSubstitute;
using NUnit.Framework;

namespace WatchHandsCalculatorsTests.WatchHandsLogicTests;

public class WatchHandsAngleDifferenceCalculatorTests
{
    [TestCase(0, 180, 180)]
    [TestCase(180, 0, 180)]
    [TestCase(90, 270, 180)]
    [TestCase(270, 90, 180)]
    [TestCase(42, 222, 180)]
    [TestCase(289, 109, 180)]
    public void Flat_angle_doesnt_break_the_logic(decimal hoursHandAngle, decimal minutesHandAngle, decimal expectedAnglesDifference)
    {
        var angleCalculator = Substitute.For<IWatchHandsAngleCalculator>();
        angleCalculator.GetHoursHandAngle(Arg.Any<TimeOnly>()).Returns(hoursHandAngle);
        angleCalculator.GetMinutesHandAngle(Arg.Any<TimeOnly>()).Returns(minutesHandAngle);
        var calculator = new WatchHandsAngleDifferenceCalculator(angleCalculator);

        var anglesDifference = calculator.GetLeastAnglesDifference(TimeOnly.MaxValue);
        
        Assert.AreEqual(expectedAnglesDifference, anglesDifference);
    }
    
    [TestCase(0, 0, 0)]
    [TestCase(359, 0, 1)]
    [TestCase(359, 1, 2)]
    [TestCase(0, 359, 1)]
    [TestCase(1, 359, 2)]
    [TestCase(42, 260, 142)]
    [TestCase(300, 15, 75)]
    [TestCase(201, 95, 106)]
    [TestCase(195, 350, 155)]
    public void Angle_difference_calculated_right(decimal hoursHandAngle, decimal minutesHandAngle, decimal expectedAnglesDifference)
    {
        var angleCalculator = Substitute.For<IWatchHandsAngleCalculator>();
        angleCalculator.GetHoursHandAngle(Arg.Any<TimeOnly>()).Returns(hoursHandAngle);
        angleCalculator.GetMinutesHandAngle(Arg.Any<TimeOnly>()).Returns(minutesHandAngle);
        var calculator = new WatchHandsAngleDifferenceCalculator(angleCalculator);

        var anglesDifference = calculator.GetLeastAnglesDifference(TimeOnly.MaxValue);
        
        Assert.AreEqual(expectedAnglesDifference, anglesDifference);
    }
}