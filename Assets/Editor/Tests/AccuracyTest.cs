using NUnit.Framework;
using TapRythm.Utils;
using TapRythm.Enums;

public class AccuracyHelperTest
{
    [Test]
    public void CalculateAccuracy_PerfectHit_ReturnsOne()
    {
        float accuracy = AccuracyHelper.CalculateAccuracy(2f, 2f);
        Assert.AreEqual(1f, accuracy, 0.01f);
    }
    
    [Test]
    public void GetHitResult_Accuracy90_ReturnsPerfect()
    {
        HitResult result = AccuracyHelper.GetHitResultFromAccuracy(0.90f);
        Assert.AreEqual(HitResult.Perfect, result);
    }
    
    [Test]
    public void GetHitResult_Accuracy70_ReturnsGreat()
    {
        HitResult result = AccuracyHelper.GetHitResultFromAccuracy(0.70f);
        Assert.AreEqual(HitResult.Great, result);
    }
    
    [Test]
    public void GetHitResult_Accuracy50_ReturnsGood()
    {
        HitResult result = AccuracyHelper.GetHitResultFromAccuracy(0.50f);
        Assert.AreEqual(HitResult.Good, result);
    }
    
    [Test]
    public void GetHitResult_Accuracy10_ReturnsMiss()
    {
        HitResult result = AccuracyHelper.GetHitResultFromAccuracy(0.10f);
        Assert.AreEqual(HitResult.Miss, result);
    }
}