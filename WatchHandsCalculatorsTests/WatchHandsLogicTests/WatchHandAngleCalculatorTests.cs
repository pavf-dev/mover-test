using System;
using MoverCandidateTest.Application.WatchHands;
using NUnit.Framework;

namespace WatchHandsCalculatorsTests.WatchHandsLogicTests;

public class WatchHandAngleCalculatorTests
{
    [Test]
    public void Seconds_are_ignored_in_hours_hand_angle_calculations()
    {
        var calculator = new WatchHandsAngleCalculator();

        var result = calculator.GetHoursHandAngle(new TimeOnly(13, 15, 42));
        
        Assert.AreEqual(37.5m, result);
    }
    
    [TestCaseSource(nameof(_angleCalculatorCasesForHourHand))]
    public void Calculates_right_angle_for_hours_hand(TimeOnly time, decimal expectedAngle)
    {
        var calculator = new WatchHandsAngleCalculator();

        var result = calculator.GetHoursHandAngle(time);
        
        Assert.AreEqual(expectedAngle, result);
    }
    
    private static object[] _angleCalculatorCasesForHourHand =
    {
        new object[] { new TimeOnly(0, 0, 0), 0m },
        new object[] { new TimeOnly(0, 1, 0), 0.5m },
        new object[] { new TimeOnly(0, 2, 0), 1m },
        new object[] { new TimeOnly(1, 0, 0), 30m },
        new object[] { new TimeOnly(3, 0, 0), 90m },
        new object[] { new TimeOnly(3, 30, 0), 105m },
        new object[] { new TimeOnly(11, 0, 0), 330m },
        new object[] { new TimeOnly(11, 59, 0), 359.5m },
        new object[] { new TimeOnly(12, 0, 0), 0m },
        new object[] { new TimeOnly(12, 1, 0), 0.5m },
        new object[] { new TimeOnly(15, 11, 0), 95.5m },
        new object[] { new TimeOnly(23, 0, 0), 330m },
        new object[] { new TimeOnly(23, 59, 0), 359.5m },
    };
    
    [Test]
    public void Hours_are_ignored_in_minutes_hand_angle_calculations()
    {
        var calculator = new WatchHandsAngleCalculator();

        var result = calculator.GetMinutesHandAngle(new TimeOnly(11, 15, 42));
        
        Assert.AreEqual(94.2m, result);
    }
    
    [TestCaseSource(nameof(_angleCalculatorCasesForMinutesHand))]
    public void Calculates_right_angle_for_minutes_hand(TimeOnly time, decimal expectedAngle)
    {
        var calculator = new WatchHandsAngleCalculator();

        var result = calculator.GetMinutesHandAngle(time);
        
        Assert.AreEqual(expectedAngle, result);
    }
    
    private static object[] _angleCalculatorCasesForMinutesHand =
    {
        new object[] { new TimeOnly(0, 0, 0), 0m },
        new object[] { new TimeOnly(0, 0, 1), 0.1m },
        new object[] { new TimeOnly(0, 0, 59), 5.9m },
        new object[] { new TimeOnly(0, 1, 0), 6m },
        new object[] { new TimeOnly(0, 1, 1), 6.1m },
        new object[] { new TimeOnly(0, 42, 37), 255.7m },
        new object[] { new TimeOnly(0, 59, 59), 359.9m },
    };
}