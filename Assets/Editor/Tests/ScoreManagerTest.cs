using NUnit.Framework;
using TapRythm.Enums;
using TapRythm.Managers;

public class ScoreManagerTests
{
    private ScoreManager _scoreManager;
    
    [SetUp]
    public void Setup()
    {
        _scoreManager = new ScoreManager();
        _scoreManager.ResetScore();
    }
    
    [Test]
    public void AddScore_Perfect_Adds100Points()
    {
        _scoreManager.AddScore(HitResult.Perfect);
        Assert.AreEqual(100, _scoreManager.CurrentScore);
    }
    
    [Test]
    public void AddScore_Great_Adds80Points()
    {
        _scoreManager.AddScore(HitResult.Great);
        Assert.AreEqual(80, _scoreManager.CurrentScore);
    }
    
    [Test]
    public void AddScore_Good_Adds50Points()
    {
        _scoreManager.AddScore(HitResult.Good);
        Assert.AreEqual(50, _scoreManager.CurrentScore);
    }
    
    [Test]
    public void AddScore_Miss_Adds0Points()
    {
        _scoreManager.AddScore(HitResult.Miss);
        Assert.AreEqual(0, _scoreManager.CurrentScore);
    }
    
    [Test]
    public void AddScore_MultipleHits_SumCorrectly()
    {
        _scoreManager.AddScore(HitResult.Perfect);
        _scoreManager.AddScore(HitResult.Great);
        _scoreManager.AddScore(HitResult.Good);
        
        Assert.AreEqual(100 + 80 + 50, _scoreManager.CurrentScore);
    }
    
    [Test]
    public void ResetScore_ClearsAllPoints()
    {
        _scoreManager.AddScore(HitResult.Perfect);
        _scoreManager.AddScore(HitResult.Great);
        
        _scoreManager.ResetScore();
        
        Assert.AreEqual(0, _scoreManager.CurrentScore);
    }
    
    [Test]
    public void Combo_ResetsToZeroOnMiss()
    {
        _scoreManager.AddScore(HitResult.Perfect);
        _scoreManager.AddScore(HitResult.Perfect);
        
        _scoreManager.AddScore(HitResult.Miss);
        
        Assert.AreEqual(0, _scoreManager.Combo);
    }
    
    [Test]
    public void Multiplier_At10Perfect()
    {
        for (int i = 0; i < 10; i++)
        {
            _scoreManager.AddScore(HitResult.Perfect);
        }
        
        Assert.AreEqual(2, _scoreManager.ComboMultiplier);
    }
    
    [Test]
    public void Multiplier_At25Perfect()
    {
        for (int i = 0; i < 25; i++)
        {
            _scoreManager.AddScore(HitResult.Perfect);
        }
        
        Assert.AreEqual(3, _scoreManager.ComboMultiplier);
    }
    
    [Test]
    public void Multiplier_At50Perfect()
    {
        for (int i = 0; i < 50; i++)
        {
            _scoreManager.AddScore(HitResult.Perfect);
        }
        
        Assert.AreEqual(4, _scoreManager.ComboMultiplier);
    }
}